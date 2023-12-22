using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Dialogue.Characters;
using Precisamento.MonoGame.Dialogue.Options;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.MathHelpers;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Linq;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue
{
    enum DialogueBoxMode
    {
        Text,
        Option
    }

    /// <summary>
    /// A Dialogue Box that uses YarnSpinner for the text processing and MonoGame
    /// for the display.
    /// </summary>
    /// <remarks>
    /// This class also contains all of the methods for displaying dialogue options
    /// so that the formatting can be shared and also so that options can be displayed
    /// inline easier.
    /// </remarks>
    public partial class DialogueBox
    {
        private const float MINUTE_IN_MS = 60000;
                         
        private Func<bool>? _continuePressed;
        private Func<bool>? _fastForwardPressed;
        private Func<int>? _scroll;
        private Action? _dismiss;
        private TextureRegion2D? _background;
        private Rectangle _bounds;
        private DialogueState _state;
        private bool _running = false;
        private int _startingLineIndex = 0;
        private List<DialogueFrame> _frames = new();
        private int _line = 0;
        private int _column = 0;
        private int _lineAtStart = 0;
        private DialogueRunner? _runner;
        private float _ticks = 0;
        private bool _hasCustomProcessors = false;
        private RenderTarget2D _texture;
        private SpriteBatchState _spriteBatchState;
        private DialogueProcessorFactory _processorFactory;
        private LineTransitionBehavior _defaultTransition = LineTransitionBehavior.NewLine;
        private bool _defaultWaitForInput = true;
        private ISentenceSplitter _sentenceSplitter;
        private DialogueBoxMode _mode;
        private Thickness _padding;
        private DialogueBox _optionWindow;
        private Game _game;

        #region Option Stuff

        /// <summary>
        /// Determines if the dialogue box continues to display if another window is currently
        /// displaying options.
        /// </summary>
        private bool _displayDialogueBoxWhileOptionsAreShowing = true;

        private bool _optionRemoveIgnoredEntriesOnSelect = true;
        private bool _optionShowSelectedOption = true;

        private int _optionSelectedIndex;

        private List<DialogueOptionDisplay> _options = new();

        /// <summary>
        /// When a non-null value is returned, selects the option with the specified index without
        /// having to scroll to it.
        /// </summary>
        private Func<int?>? _optionQuickSelect;

        /// <summary>
        /// A function that should return a value indicating how much the selected option index has
        /// been requested to change. Negative numbers will go up, positive numbers down. If this
        /// returns the same sign for multiple frames, it will act as if the button has been held down,
        /// and will utilize the auto scroll fields to determine if the cursor should actuall move or
        /// not. If this is not the desired behavior, set the initial/secondary wait values to zero and
        /// perform the custom behavior in the implementor of this function.
        /// </summary>
        private Func<int>? _optionSelectMove;

        private Func<bool>? _optionSelected;

        private Func<bool>? _optionCanceled;

        // === Option Auto Scroll ===

        /// <summary>
        /// Determines if the selected option index is allowed to wrap. 
        /// For example, if the last option is selected and the user pressed down, if this is true,
        /// the selected index will move to the first option.
        /// </summary>
        private bool _optionAllowScrollWrap;

        /// <summary>
        /// Determines the initial amount of time needed for the user to hold down up or down before
        /// the selected option index moves again.
        /// </summary>
        private float _optionAutoScrollInitialWait;

        /// <summary>
        /// Determines the amount of time needed for the user to hold up or down before the selected
        /// option index moves after the first auto scroll.
        /// </summary>
        private float _optionAutoScrollSecondaryWait;

        /// <summary>
        /// Keeps track of the amount of time the user has to continue holding up or down before the
        /// selected option index changes.
        /// </summary>
        private float _optionAutoScrollTimer;

        /// <summary>
        /// Keeps track of the option select direction the user has been holding.
        /// </summary>
        private int _optionAutoScrollDirection;

        //  === Option Selection Indicators ===

        /// <summary>
        /// An animation that draws behind the currently selected option.
        /// </summary>
        private SpriteAnimationPlayer _optionSelectedBackgroundPlayer = new();

        /// <summary>
        /// The padding between the selected option background and the option text.
        /// </summary>
        private Thickness _optionSelectedBackgroundPadding;

        /// <summary>
        /// An animation that draws next to the currrently selected icon.
        /// </summary>
        private SpriteAnimationPlayer _optionSelectedIconPlayer = new();

        /// <summary>
        /// Determines where the selected icon is drawn in relation to the currently selected option.
        /// </summary>
        private SelectIconLocation _optionSelectedIconLocation;

        /// <summary>
        /// Moves the selected icon a specified distance from its defalt draw location.
        /// </summary>
        private Point _optionSelectedIconOffset;

        // === Option Window ===

        /// <summary>
        /// Determines the way and position that options are rendered.
        /// </summary>
        private DialogueOptionRenderLocation _optionWindowLocation;

        private Size _optionWindowMinBounds;

        private Size _optionWindowMaxBounds;

        private Point _optionWindowOffset;

        private Thickness _optionWindowPadding;

        private bool _optionWindowAlwaysUseMaxBounds;

        private bool _lastPaddingWasOption = true;

        private bool _waitingOnOptionSelect = false;

        private Action<int> _onOptionSelected;

        private bool _isOptionWindow;

        #endregion

        #region Characters

        private ICharacterProcessorFactory _characterFactory;
        private DialogueCharacterState _characterState = new();
        private SpriteAnimationPlayer _characterBackgroundPlayer = new();
        private SpriteAnimationPlayer _characterFacePlayer = new();

        #endregion

        private bool Finished
        {
            get
            {
                var lastFrame = _frames[^1];
                var lines = lastFrame.Lines;
                return _running && _line == lines.Count - 1 && _column >= lines[_line].Line.Length;
            }
        }

        public DialogueBox(Game game, DialogueBoxOptions options)
        {
            var optionSettings = options.OptionBoxOptions;

            _game = game;
            _continuePressed = options.ContinuePressed;
            _fastForwardPressed = options.FastForwardPressed;
            _dismiss = options.Dismiss;
            _scroll = options.Scroll;
            _background = options.IsOptionWindow ? optionSettings.WindowBackground : options.Background;
            _bounds = options.Bounds;
            _padding = options.Padding;
            _sentenceSplitter = options.SentenceSplitter;
            _isOptionWindow = options.IsOptionWindow;

            _displayDialogueBoxWhileOptionsAreShowing = options.DisplayDialogueBoxWhileOptionsAreShowing;
            _optionQuickSelect = optionSettings.QuickSelect;
            _optionSelectMove = optionSettings.MoveSelection;
            _optionSelected = optionSettings.Selected;
            _optionCanceled = optionSettings.Canceled;

            _optionAllowScrollWrap = optionSettings.AllowScrollWrap;
            _optionAutoScrollInitialWait = optionSettings.AutoScrollInitialWait;
            _optionAutoScrollSecondaryWait = optionSettings.AutoScrollSecondaryWait;
            _optionSelectedBackgroundPlayer.Animation = optionSettings.OptionBackground;
            _optionSelectedBackgroundPadding = optionSettings.OptionBackgroundPadding;
            _optionSelectedIconPlayer.Animation = optionSettings.SelectIcon;
            _optionSelectedIconLocation = optionSettings.SelectIconLocation;
            _optionSelectedIconOffset = optionSettings.SelectIconOffset;
            _optionWindowLocation = optionSettings.RenderLocation;
            _optionWindowMinBounds = optionSettings.MinBounds;
            _optionWindowMaxBounds = optionSettings.MaxBounds;
            _optionWindowOffset = optionSettings.Offset;
            _optionWindowPadding = optionSettings.Padding;
            _optionWindowAlwaysUseMaxBounds = optionSettings.AlwaysUseMaxBounds;

            _state = new DialogueState(options.TextColor, options.Font)
            {
                TimePerLetter = options.TextSpeed / MINUTE_IN_MS
            };

            SetDrawArea(false);

            _state.CurrentPosition = _state.DrawArea.Location.ToVector2();

            if (_state.DrawArea.Width < 0 || _state.DrawArea.Height < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(options.Padding),
                    "The text render bounds had a negative width or height.");
            }

            _spriteBatchState = game.Services.GetService<SpriteBatchState>();
            _processorFactory = new DialogueProcessorFactory(game);
            _characterFactory = new CharacterProfileProcessorFactory(game, _characterState, options.ProfileOptions.Characters ?? new());
            if(!options.IsOptionWindow)
                _texture = new RenderTarget2D(_spriteBatchState.GraphicsDevice, _bounds.Width, _bounds.Height);

            if (_optionWindowLocation == DialogueOptionRenderLocation.Inline)
            {
                _onOptionSelected = (index) =>
                {
                    if (!_optionShowSelectedOption)
                    {
                        _frames.RemoveRange(_frames.Count - _options.Count, _options.Count);
                        SetStartingDrawLine();
                    }
                    else if (_optionRemoveIgnoredEntriesOnSelect)
                    {
                        _frames.RemoveRange(_frames.Count - _options.Count, _options.Count);
                        var frame = _options[index].Line;
                        frame.LineStart = _frames[^1].LineStart + _frames[^1].Lines.Count;
                        _frames.Add(_options[index].Line);
                        SetStartingDrawLine();
                        FastForward();
                    }

                    _mode = DialogueBoxMode.Text;
                    _runner!.SetSelectedOption(index);
                };
            }
            else if(!_isOptionWindow
                && _optionWindowLocation != DialogueOptionRenderLocation.List 
                && _optionWindowLocation != DialogueOptionRenderLocation.CustomRenderMethod)
            {
                options.IsOptionWindow = true;
                _optionWindow = new DialogueBox(game, options);
            }
        }

        private void SetDrawArea(bool isOption)
        {
            if (_lastPaddingWasOption == isOption)
                return;

            _lastPaddingWasOption = isOption;

            var padding = isOption ? _optionWindowPadding : _padding;

            _state.DrawArea = new Rectangle(
                padding.Left,
                padding.Top,
                _bounds.Width - padding.Width,
                _bounds.Height - padding.Height);
        }

        public void AttachToRunner(DialogueRunner runner)
        {
            _runner = runner;
            _runner.DialogueStarted += OnDialogueStarted;
            _runner.LineNeedsPresented += OnLineNeedsPresented;
            _runner.DialogueCompleted += OnDialogueCompleted;
            _runner.OptionsNeedPresented += OnOptionsNeedPresenting;
            _runner.CommandTriggered += ProcessCommand;
        }

        public void DetachFromRunner()
        {
            _runner!.DialogueStarted -= OnDialogueStarted;
            _runner!.LineNeedsPresented -= OnLineNeedsPresented;
            _runner!.DialogueCompleted -= OnDialogueCompleted;
            _runner!.OptionsNeedPresented -= OnOptionsNeedPresenting;
            _runner!.CommandTriggered -= ProcessCommand;
        }

        private void ProcessCommand(object? sender, CommandTriggeredArgs e)
        {
            if (e.Handled)
                return;

            switch(e.CommandElements[0])
            {
                case "show":
                    HandleShowCommand(e.CommandElements);
                    break;
                case "hide":
                    HandleHideCommand(e.CommandElements);
                    break;
            }
        }

        private void OnOptionsNeedPresenting(object? sender, DialogueOption[] options)
        {
            switch (_optionWindowLocation)
            {
                case DialogueOptionRenderLocation.Inline:
                    PresentInlineOptions(options);
                    break;
                case DialogueOptionRenderLocation.List:
                case DialogueOptionRenderLocation.CustomRenderMethod:
                    throw new NotImplementedException();
                default:
                    _optionWindow._running = true;
                    _waitingOnOptionSelect = true;
                    _optionWindow.SetOptionsForOptionsWindow(options, _bounds, index =>
                    {
                        _waitingOnOptionSelect = false;
                        _optionWindow._running = false;
                        if (_optionShowSelectedOption)
                        {
                            PresentLine(options[index].Line, true);
                        }
                        _runner!.SetSelectedOption(index);
                    });
                    break;
            }
        }

        private void PresentInlineOptions(DialogueOption[] options)
        {
            _state.LineTransition = LineTransitionBehavior.NewLine;
            _state.WaitForInput = false;
            _line = 0;
            _column = 0;
            _optionSelectedIndex = 0;

            _options.Clear();

            foreach (var option in options)
            {
                var frame = CreateFrame(option.Line, _state, true);
                var optionDisplay = new DialogueOptionDisplay(frame, option.DialogueOptionId, option.IsAvailable);

                _options.Add(optionDisplay);
            }

            JumpToOption();

            _mode = DialogueBoxMode.Option;

            var lastOption = _options[^1].Line;

            _line = lastOption.Lines.Count - 1;
            _column = lastOption.Lines[_line].Line.Length;

            if(!_isOptionWindow)
                Redraw();
        }

        private void OnDialogueStarted(object? sender, EventArgs e)
        {
            _running = true;
            _lineAtStart = _frames.Count == 0 ? 0 : _frames[^1].LineStart + _frames[^1].Lines.Count;
            _spriteBatchState.SetRenderTarget(_texture);
            _spriteBatchState.GraphicsDevice.Clear(Color.Transparent);
            _spriteBatchState.SetRenderTarget(null);
        }

        private void OnDialogueCompleted(object? sender, EventArgs e)
        {
            _running = false;
            _dismiss?.Invoke();
        }

        private void OnLineNeedsPresented(object? sender, LocalizedLine line)
        {
            PresentLine(line, false);
        }

        private void PresentLine(LocalizedLine line, bool isOption)
        {
            _state.LineTransition = _defaultTransition;
            _state.WaitForInput = _defaultWaitForInput;
            _line = 0;
            _column = 0;
            _waitingOnOptionSelect = false;

            var frame = CreateFrame(line, _state, isOption);

            if (_lineAtStart == frame.LineStart || _state.LineTransition != LineTransitionBehavior.NewLine)
            {
                _startingLineIndex = frame.LineStart;
            }
            else
            {
                SetStartingDrawLine();
            }
        }

        private DialogueFrame CreateFrame(LocalizedLine line, DialogueState state, bool isOption)
        {
            var startingFrameLine = _frames.Count == 0
                ? 0
                : _frames[^1].LineStart + _frames[^1].Lines.Count;

            SetDrawArea(isOption);

            var frame = new DialogueFrame(
                line,
                _processorFactory,
                _characterFactory,
                state,
                _sentenceSplitter,
                startingFrameLine);

            frame.IsOption = isOption;

            _frames.Add(frame);

            return frame;
        }

        public void Update(float delta)
        {
            if (_optionWindow?._running ?? false)
            {
                _optionWindow.Update(delta);
                return;
            }

            if (!_running || _frames.Count == 0)
                return;

            switch(_mode)
            {
                case DialogueBoxMode.Text:
                    UpdateText(delta);
                    break;
                case DialogueBoxMode.Option:
                    _optionSelectedBackgroundPlayer.Update(delta);
                    _optionSelectedIconPlayer.Update(delta);
                    UpdateOption(delta);
                    break;
            }
        }

        private void UpdateText(float delta)
        {
            var continuePressed = _continuePressed?.Invoke() ?? false;
            var fastForward = _fastForwardPressed?.Invoke() ?? false;
            var needsRedraw = _hasCustomProcessors;

            if (Finished)
            {
                UpdateCharacters(delta);

                if (continuePressed || fastForward || !_state.WaitForInput)
                {
                    _characterState.Removing.AddRange(_characterState.Characters);
                    _ticks = 0;
                    _runner!.Continue();
                    return;
                }
                else
                {
                    needsRedraw = CheckScroll();
                }
            }
            else if (fastForward)
            {
                FastForward();
                needsRedraw = true;
            }
            else
            {
                _ticks += delta;
                while (_ticks >= _state.TimePerLetter && !Finished)
                {
                    if (_column == _frames[^1].Lines[_line].Line.Length)
                    {
                        _line++;
                        _column = 0;
                        SetStartingDrawLine();
                    }
                    else
                    {
                        _column++;
                    }
                    _ticks -= _state.TimePerLetter;
                    needsRedraw = true;
                }
            }

            UpdateCharacters(delta);

            if (needsRedraw)
            {
                Redraw();
            }
        }

        private void FastForward()
        {
            var frame = _frames[^1];
            _line = frame.Lines.Count - 1;
            _column = frame.Lines[_line].Line.Length;
            SetStartingDrawLine();
        }

        private void Redraw()
        {
            _spriteBatchState.SetRenderTarget(_texture);
            _spriteBatchState.GraphicsDevice.Clear(Color.Transparent);
            _spriteBatchState.Begin();
            _state.CurrentPosition = new Vector2(0, _state.DrawArea.Top);
            DrawText(_spriteBatchState);
            _spriteBatchState.End();
            _spriteBatchState.SetRenderTarget(null);
        }

        private void SetStartingDrawLine()
        {
            var lastFrame = _frames[^1];
            var startingLine = lastFrame.LineStart + _line;
            var totalHeight = 0;

            for(var frame = _frames.Count - 1; frame >= 0; frame--)
            {
                for(var line = frame == _frames.Count - 1 ? _line : _frames[frame].Lines.Count - 1; line >= 0; line--)
                {
                    totalHeight += _frames[frame].Lines[line].Height;
                    if(totalHeight <= _state.DrawArea.Height)
                    {
                        startingLine = _frames[frame].LineStart + line;
                    }
                    else
                    {
                        if (_startingLineIndex == _lineAtStart && startingLine < _lineAtStart)
                            return;
                        _startingLineIndex = startingLine;
                        return;
                    }
                }
            }
        }

        private bool CheckScroll()
        {
            var delta = _scroll?.Invoke() ?? 0;
            var originalStartingLine = _startingLineIndex;
            if(delta != 0)
            {
                _startingLineIndex = MathExt.Clamp(
                    _startingLineIndex + delta, 
                    0, 
                    _frames[^1].LineStart + _line);
            }

            // Don't allow the user to scroll below the last option
            // inside of option windows.
            if (_isOptionWindow)
            {
                ClampScroll();
            }

            return originalStartingLine != _startingLineIndex;
        }

        /// <summary>
        /// Ensures that the dialogue window can't scroll past the last line.
        /// </summary>
        private void ClampScroll()
        {
            if (_startingLineIndex == 0)
                return;

            var startingFrame = FindStartingFrame();
            var height = 0;
            var maxHeight = _bounds.Height - _padding.Height;

            // This for loop measures the height of the currently displayed lines.
            for (var i = startingFrame; i < _frames.Count; i++)
            {
                var lastLine = i == _frames.Count - 1 ? _line : _frames[i].Lines.Count - 1;
                if (i == startingFrame)
                {
                    lastLine = Math.Max(lastLine, _startingLineIndex - _frames[i].LineStart);
                }

                for(var line = 0; line < lastLine; line++)
                {
                    height += _frames[i].Lines[line].Height;
                    if (height >= maxHeight)
                        return;
                }
            }

            var currentFrame = startingFrame;
            var currentLine = _startingLineIndex - _frames[currentFrame].LineStart;

            // Continue moving the starting line up until we've reached the top
            // or the lines have taken up the whole screen.
            while (height < maxHeight && _startingLineIndex > 0)
            {
                if (currentLine == 0)
                {
                    currentFrame--;
                    currentLine = _frames[currentFrame].Lines.Count - 1;
                }
                else
                {
                    currentLine--;
                }

                height += _frames[currentFrame].Lines[currentLine].Height;
                if (height > maxHeight)
                    break;

                _startingLineIndex--;
            }
        }

        public void Draw(SpriteBatchState state)
        {
            if (!_running)
                return;

            DrawCharacters(state);

            if(_optionWindow?._running ?? false)
            {
                _optionWindow.Draw(state);
                if(_waitingOnOptionSelect && !_displayDialogueBoxWhileOptionsAreShowing)
                {
                    return;
                }
            }

            state.SpriteBatch.Draw(_background, _bounds, Color.White);

            if(_mode == DialogueBoxMode.Option)
            {
                var optionBounds = CalculateSelectedOptionBounds();
                if(!optionBounds.IsEmpty)
                {
                    var backgroundBounds = CalculateSelectedBackgroundBounds(optionBounds);
                    _optionSelectedBackgroundPlayer.Draw(state, backgroundBounds);

                    var iconBounds = CalculateSelectIconBounds(optionBounds);
                    _optionSelectedIconPlayer.Draw(state, iconBounds);
                }
            }

            state.SpriteBatch.Draw(_texture, _bounds, Color.White);
        }

        private void DrawText(SpriteBatchState state)
        {
            var startingFrame = FindStartingFrame();

            for(var i = startingFrame; i < _frames.Count; i++)
            {
                var line = i == _frames.Count - 1 ? _line : _frames[i].Lines.Count - 1;
                if(i == startingFrame)
                {
                    line = Math.Max(line, _startingLineIndex - _frames[i].LineStart);
                }
                var column = i == _frames.Count - 1 ? _column : _frames[i].Lines[^1].Line.Length;

                var doneDrawing = DrawFrame(state, i, line, column);

                if (doneDrawing)
                    break;
            }
        }

        private int FindStartingFrame()
        {
            var min = 0;
            var max = _frames.Count - 1;

            while(min <= max)
            {
                var mid = (min + max) / 2;
                var frame = _frames[mid];
                var frameEnd = frame.LineStart + frame.Lines.Count;
                if (frame.LineStart <= _startingLineIndex && frameEnd > _startingLineIndex)
                {
                    return mid;
                }
                else if (_startingLineIndex < frame.LineStart)
                {
                    max = mid - 1;
                }
                else
                {
                    min = mid + 1;
                }
            }

            return 0;
        }

        private bool DrawFrame(SpriteBatchState state, int frameIndex, int line, int column)
        {
            var frame = _frames[frameIndex];
            SetDrawArea(frame.IsOption);
            _state.CurrentPosition = new Vector2(_state.DrawArea.X, _state.CurrentPosition.Y);
            return frame.Draw(state, _state, _startingLineIndex, line, column);
        }

        private Rectangle CalculateSelectedOptionBounds()
        {
            var startingFrame = FindStartingFrame();
            var totalHeight = 0;
            var y = 0;
            var size = new Size();
            var selectedFrame = _options[_optionSelectedIndex].Line;
            for(var i = startingFrame; i < _frames.Count; i++)
            {
                var frame = _frames[i];
                if(frame == selectedFrame)
                {
                    y = totalHeight;
                }

                for(var line = i == startingFrame ? _startingLineIndex - frame.LineStart : 0; line < frame.Lines.Count; line++)
                {
                    totalHeight += frame.Lines[line].Height;
                    if (totalHeight > _state.DrawArea.Height)
                    {
                        goto Finished;
                    }
                    else if(frame == selectedFrame)
                    {
                        size.Height += frame.Lines[line].Height;
                        size.Width = Math.Max(size.Width, frame.Lines[line].Width);
                    }
                }
            }

            Finished:

            if(!size.IsEmpty)
            {
                SetDrawArea(true);
                return new Rectangle(
                    _bounds.X + _state.DrawArea.X, 
                    _bounds.Y + _state.DrawArea.Y + y, size.Width, size.Height);
            }
            else
            {
                return Rectangle.Empty;
            }
        }
    }
}
