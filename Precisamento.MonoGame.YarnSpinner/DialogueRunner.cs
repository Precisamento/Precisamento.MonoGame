using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Yarn;
using Yarn.Markup;

namespace Precisamento.MonoGame.YarnSpinner
{
    public class DialogueRunner
    {
        /// <summary>
        /// The last set of options presented to the user.
        /// Necessary to keep around in order for <see cref="SetSelectedOption(int)"/>
        /// to know which option was selected.
        /// </summary>
        private DialogueOption[]? _options;

        /// <summary>
        /// When a method called by <see cref="CommandHandler"/> needs to run
        /// for a long period of time, it can return a <see cref="CommandResult"/>
        /// which will be stored in this variable to be watched in the update event
        /// for the command to complete. After <see cref="CommandResult.Complete"/> is set
        /// to true, automatically calls <see cref="Continue"/>.
        /// <para>
        /// Also used when <see cref="Continue"/> gets called from another thread in order to
        /// wait for the main thread to be able to process it.
        /// </para>
        /// </summary>
        private CommandResult? _waitForCommand;

        /// <summary>
        /// Used when <see cref="SetSelectedOption(int)"/> is called from a background thread
        /// in order to make sure it's processed on the main thread.
        /// </summary>
        private SetSelectedIndexSync _setSelectedIndexSync = new SetSelectedIndexSync();

        /// <summary>
        /// The thread that called <see cref="Start(string)"/>.
        /// </summary>
        private int _runningThreadId;

        /// <summary>
        /// Determines if the dialogue is actively running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// An adapter that can be used to capture log events.
        /// </summary>
        public ILogger? Logger { get; private set; }

        /// <summary>
        /// Gets the name of the current node that is being run.
        /// </summary>
        public string? CurrentNode => Dialogue.CurrentNode;

        /// <summary>
        /// Determines if an exception is thrown when a warning is issued.
        /// The warning message can be captured by the <see cref="Logger"/> field otherwise.
        /// </summary>
        public bool ThrowOnWarning { get; set; }

        /// <summary>
        /// Determines if an exception is thrown when an error is issued.
        /// The error message can be captured by the <see cref="Logger"/> field otherwise.
        /// </summary>
        public bool ThrowOnError { get; set; }

        /// <summary>
        /// Determines if a <see cref="LogAdapter.LogDebug"/> event will be triggered
        /// every time it enters a node, and other frequent events.
        /// </summary>
        public bool VerboseLogging { get; set; }

        /// <summary>
        /// When an option is selected, determines if its handled as a line.
        /// </summary>
        public bool RunSelectedOptionAsLine { get; set; }


        /// <summary>
        /// Determines if the <see cref="CommandTriggered"/> event is triggered
        /// even if the command was already handled via the <see cref="CommandHandler"/>.
        /// </summary>
        public bool RunCommandTriggeredAfterCommandHandler { get; set; }

        /// <summary>
        /// Gets the locale to use when retrieving strings from <see cref="Localization"/>.
        /// </summary>
        public string? Locale { get; set; }

        /// <summary>
        /// Gets the underlying dialogue object that runs the yarn code.
        /// </summary>
        public Dialogue Dialogue { get; private set; }

        /// <summary>
        /// A localization object that is used to retrieve string values
        /// based on the current <see cref="Locale"/>, falling back to
        /// <see cref="YarnLocalization.BaseLocale"/> if the string isn't
        /// loaded in the specified locale.
        /// </summary>
        public YarnLocalization Localization { get; private set; }

        /// <summary>
        /// Used to dispatch commands to the registered functions.
        /// Any commands not registered in this will need to be handled by
        /// <see cref="CommandTriggered"/>.
        /// </summary>
        public CommandHandler CommandHandler { get; private set; }

        /// <summary>
        /// The variable storage object.
        /// </summary>
        public IVariableStorage VariableStorage => Dialogue.VariableStorage;

        /// <summary>
        /// Triggered when a set of options are ready to be displayed to the user.
        /// One of the handlers must call <see cref="SetSelectedOption(int)"/> in order
        /// for the dialogue to contine.
        /// </summary>
        public event EventHandler<DialogueOption[]>? OptionsNeedPresented;

        /// <summary>
        /// Triggered when a line is ready to be shown to the user.
        /// One of the handlers must call <see cref="Continue()"/> in order
        /// for the dialogue to continue.
        /// </summary>
        public event EventHandler<LocalizedLine>? LineNeedsPresented;

        /// <summary>
        /// Triggered when a command has been triggered. Set 
        /// <see cref="RunCommandTriggeredAfterCommandHandler"/> to true
        /// to trigger this after the command is handled by the <see cref="CommandHandler"/>.
        /// </summary>
        public event EventHandler<CommandTriggeredArgs>? CommandTriggered;

        /// <summary>
        /// Triggered when the dialogue enters a new node.
        /// </summary>
        public event EventHandler<string>? NodeStarted;

        /// <summary>
        /// Triggered after the dialogue finishes processing a node.
        /// Does not get triggered if <see cref="Stop"/> is called.
        /// </summary>
        public event EventHandler<string>? NodeEnded;

        /// <summary>
        /// Triggered when <see cref="Start(string)"/> is called.
        /// </summary>
        public event EventHandler? DialogueStarted;

        /// <summary>
        /// Triggered when the dialogue has reached its end.
        /// </summary>
        public event EventHandler? DialogueCompleted;
        public event EventHandler<IEnumerable<string>>? PrepareForLines;

        public DialogueRunner(YarnLocalization localization)
            : this(localization, new MemoryVariableStore())
        {
        }

        public DialogueRunner(YarnLocalization localization, IVariableStorage storage)
            : this(localization, storage, null)
        {
        }

        public DialogueRunner(YarnLocalization localization, CommandHandler? handler)
            : this(localization, new MemoryVariableStore(), handler)
        {
        }

        public DialogueRunner(YarnLocalization localization, IVariableStorage storage, CommandHandler? handler)
        {
            Localization = localization;
            Dialogue = new Dialogue(storage)
            {
                LogDebugMessage = message =>
                {
                    if (VerboseLogging)
                        Logger?.LogInformation("{Message}", message);
                },

                LogErrorMessage = Error,

                LineHandler = HandleLine,
                CommandHandler = HandleCommand,
                OptionsHandler = HandleOptions,
                NodeStartHandler = HandleNodeStarted,
                NodeCompleteHandler = HandleNodeEnded,
                DialogueCompleteHandler = HandleDialogueComplete,
                PrepareForLinesHandler = HandlePrepareForLines,
            };
            CommandHandler = handler ?? new CommandHandler();
            CommandHandler.RegisterCommand(Wait);
        }

        public DialogueRunner(YarnLocalization localization, Dialogue dialogue)
            : this(localization, dialogue, null)
        {
        }

        public DialogueRunner(YarnLocalization localization, Dialogue dialogue, CommandHandler? handler)
        {
            Localization = localization;
            Dialogue = dialogue;
            Dialogue.LogDebugMessage = message =>
            {
                if (VerboseLogging)
                    Logger?.LogInformation("{Message}", message);
            };

            Dialogue.LogErrorMessage = Error;

            Dialogue.LineHandler = HandleLine;
            Dialogue.CommandHandler = HandleCommand;
            Dialogue.OptionsHandler = HandleOptions;
            Dialogue.NodeStartHandler = HandleNodeStarted;
            Dialogue.NodeCompleteHandler = HandleNodeEnded;
            Dialogue.DialogueCompleteHandler = HandleDialogueComplete;
            Dialogue.PrepareForLinesHandler = HandlePrepareForLines;

            CommandHandler = handler ?? new CommandHandler();
            CommandHandler.RegisterCommand(Wait);
        }

        private void HandleOptions(OptionSet optionSet)
        {
            _options = new DialogueOption[optionSet.Options.Length];
            for(var i = 0; i < _options.Length; i++)
            {
                var line = GetLine(optionSet.Options[i].Line, optionSet.Options[i].Line.Substitutions);
                _options[i] = new DialogueOption
                {
                    TextId = line.TextId,
                    DialogueOptionId = optionSet.Options[i].ID,
                    Line = line,
                    IsAvailable = optionSet.Options[i].IsAvailable
                };
            }

            OptionsNeedPresented?.Invoke(this, _options);
        }

        private void HandleLine(Line line)
        {
            var localizedLine = GetLine(line, line.Substitutions);
            HandleLine(localizedLine);
        }

        private void HandleLine(LocalizedLine line)
        {
            LineNeedsPresented?.Invoke(this, line);
        }

        private void HandleCommand(Command commandValue)
        {
            var elements = SplitCommandText(commandValue.Text).ToArray();
            TriggerCommand(elements);
        }

        private void HandleNodeStarted(string node)
        {
            NodeStarted?.Invoke(this, node);
        }

        private void HandleNodeEnded(string node)
        {
            NodeEnded?.Invoke(this, node);
        }

        private void HandleDialogueComplete()
        {
            IsRunning = false;
            DialogueCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void HandlePrepareForLines(IEnumerable<string> lineIds)
        {
            PrepareForLines?.Invoke(this, lineIds);
        }

        private LocalizedLine GetLine(Line line, IList<string> substitutions)
        {
            if(!Localization.TryGetString(Locale, line.ID, out var text))
            {
                Error($"Yarn: No string registered for the ID {line.ID}");
                text = string.Empty;
            }

            var finalText = Dialogue.ExpandSubstitutions(text, substitutions);

            MarkupParseResult markup;

            try
            {
                markup = Dialogue.ParseMarkup(finalText);
            }
            catch(MarkupParseException ex)
            {
                Error($"Yarn: Failed to parse markup in \"{finalText}\": {ex.Message}", ex);
                markup = new MarkupParseResult
                {
                    Text = finalText,
                    Attributes = new List<MarkupAttribute>()
                };
            }

            return new LocalizedLine
            {
                TextId = line.ID,
                RawText = text,
                Text = markup,
                Substitutions = line.Substitutions,
                Metadata = Localization.GetMetadata(line.ID)
            };
        }

        [YarnCommand("wait")]
        protected virtual CommandResult Wait(float time)
        {
            var result = CommandResult.Pool.Get();
            Task.Run(async () =>
            {
                await Task.Delay((int)(time * 1000));
                result.Complete = true;
            });
            return result;
        }

        public void TriggerCommand(string[] args)
        {
            var command = args[0];
            if (CommandHandler.TryRunCommand(args, this, out _waitForCommand))
            {
                if (RunCommandTriggeredAfterCommandHandler)
                    CommandTriggered?.Invoke(this, new CommandTriggeredArgs(args, true));

                if (_waitForCommand is null || _waitForCommand.Complete)
                {
                    _waitForCommand = null;
                    Continue();
                }
                else
                {
                    IsRunning = false;
                }
            }
            else if (CommandTriggered != null)
            {
                CommandTriggered.Invoke(this, new CommandTriggeredArgs(args, false));
            }
            else
            {
                Error($"Yarn: No command registered for \"{command}\" and no handler for the {nameof(CommandTriggered)} event subscribed. The dialogue will never continue.");
                if (!ThrowOnError)
                    Continue();
            }
        }

        public void Update()
        {
            if(_waitForCommand != null && _waitForCommand.Complete)
            {
                IsRunning = true;
                CommandResult.Pool.Release(_waitForCommand);
                _waitForCommand = null;
                Continue();
            }
            else if(_setSelectedIndexSync.Synchronizing)
            {
                _setSelectedIndexSync.Synchronizing = false;
                SetSelectedOption(_setSelectedIndexSync.Index);
            }
        }

        public void Start(string startNode)
        {
            if(IsRunning)
            {
                Error("Yarn: Cannot start dialogue: the dialogue is already running.");
                return;
            }

            _runningThreadId = Environment.CurrentManagedThreadId;

            IsRunning = true;

            DialogueStarted?.Invoke(this, EventArgs.Empty);

            Dialogue.SetNode(startNode);

            Continue();
        }

        public void Stop()
        {
            IsRunning = false;
            Dialogue.Stop();
        }

        public void Continue()
        {
            if(!IsRunning)
            {
                Warning("Yarn: Can't continue when the dialogue isn't running.");
                return;
            }

            if(Environment.CurrentManagedThreadId != _runningThreadId)
            {
                _waitForCommand = CommandResult.Pool.Get();
                _waitForCommand.Complete = true;
            }
            else
            {
                Dialogue.Continue();
            }
        }

        public void SetSelectedOption(int optionIndex)
        {
            if(!IsRunning)
            {
                Warning("Yarn: Can't select an option when the dialogue isn't running");
                return;
            }

            if(Environment.CurrentManagedThreadId == _runningThreadId)
            {
                Dialogue.SetSelectedOption(optionIndex);
                if (RunSelectedOptionAsLine && _options != null)
                {
                    HandleLine(_options[optionIndex].Line);
                }
                else
                {
                    Continue();
                }
            }
            else
            {
                _setSelectedIndexSync.Synchronizing = true;
                _setSelectedIndexSync.Index = optionIndex;
            }
        }

        public bool NodeExists(string node) => Dialogue.NodeExists(node);
        public IEnumerable<string> GetTagsForNode(string node) => Dialogue.GetTagsForNode(node);

        public void SetInitialVariables(Program program, bool overrideExistingValues)
        {
            foreach(var pair in program.InitialValues)
            {
                if (!overrideExistingValues && VariableStorage.TryGetValue<object>(pair.Key, out _))
                {
                    continue;
                }
                var value = pair.Value;
                switch (value.ValueCase)
                {
                    case Operand.ValueOneofCase.StringValue:
                        VariableStorage.SetValue(pair.Key, value.StringValue);
                        break;
                    case Operand.ValueOneofCase.BoolValue:
                        VariableStorage.SetValue(pair.Key, value.BoolValue);
                        break;
                    case Operand.ValueOneofCase.FloatValue:
                        VariableStorage.SetValue(pair.Key, value.FloatValue);
                        break;
                    default:
                        Error($"Yarn: {pair.Key} is of an invalid type: {value.ValueCase}");
                        break;
                }
            }
        }

        private void Warning(string message) => Warning(message, null);

        private void Warning(string message, Exception? ex)
        {
            var exception = new InvalidOperationException(message, ex);
            Logger?.LogWarning(exception, "{Message}", message);
            if (ThrowOnWarning)
                throw exception;
        }

        private void Error(string message) => Error(message, null);

        private void Error(string message, Exception? ex)
        {
            var exception = new InvalidOperationException(message, ex);
            Logger?.LogError(exception, "{Message}", message);
            if (ThrowOnError)
                throw exception;
        }

        public static IEnumerable<string> SplitCommandText(string input)
        {
            var reader = new System.IO.StringReader(input.Normalize());

            int c;

            var results = new List<string>();
            var currentComponent = new StringBuilder();

            while ((c = reader.Read()) != -1)
            {
                if (char.IsWhiteSpace((char)c))
                {
                    if (currentComponent.Length > 0)
                    {
                        // We've reached the end of a run of visible
                        // characters. Add this run to the result list and
                        // prepare for the next one.
                        results.Add(currentComponent.ToString());
                        currentComponent.Clear();
                    }
                    else
                    {
                        // We encountered a whitespace character, but
                        // didn't have any characters queued up. Skip this
                        // character.
                    }

                    continue;
                }
                else if (c == '\"')
                {
                    // We've entered a quoted string!
                    while (true)
                    {
                        c = reader.Read();
                        if (c == -1)
                        {
                            // Oops, we ended the input while parsing a
                            // quoted string! Dump our current word
                            // immediately and return.
                            results.Add(currentComponent.ToString());
                            return results;
                        }
                        else if (c == '\\')
                        {
                            // Possibly an escaped character!
                            var next = reader.Peek();
                            if (next == '\\' || next == '\"')
                            {
                                // It is! Skip the \ and use the character after it.
                                reader.Read();
                                currentComponent.Append((char)next);
                            }
                            else
                            {
                                // Oops, an invalid escape. Add the \ and
                                // whatever is after it.
                                currentComponent.Append((char)c);
                            }
                        }
                        else if (c == '\"')
                        {
                            // The end of a string!
                            break;
                        }
                        else
                        {
                            // Any other character. Add it to the buffer.
                            currentComponent.Append((char)c);
                        }
                    }

                    results.Add(currentComponent.ToString());
                    currentComponent.Clear();
                }
                else
                {
                    currentComponent.Append((char)c);
                }
            }

            if (currentComponent.Length > 0)
            {
                results.Add(currentComponent.ToString());
            }

            return results;
        }

        private class SetSelectedIndexSync
        {
            public bool Synchronizing { get; set; }
            public int Index { get; set; }
        }
    }
}
