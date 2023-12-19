using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
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

    public class DialogueBox
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
        private bool _firstLineFromStart = false;
        private int _startingLineIndex = 0;
        private List<DialogueFrame> _frames = new List<DialogueFrame>();
        private int _line = 0;
        private int _column = 0;
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

        private int _optionSelectedIndex;

        private List<DialogueOptionDisplay> _options = new List<DialogueOptionDisplay>();

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
        private SpriteAnimationPlayer _optionSelectedBackgroundPlayer = new SpriteAnimationPlayer();

        /// <summary>
        /// The padding between the selected option background and the option text.
        /// </summary>
        private Thickness _optionSelectedBackgroundPadding;

        /// <summary>
        /// An animation that draws next to the currrently selected icon.
        /// </summary>
        private SpriteAnimationPlayer _optionSelectedIconPlayer = new SpriteAnimationPlayer();

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

        private TextureRegion2D? _optionWindowBackground;

        private bool _lastPaddingWasOption = true;

        private bool _waitingOnOptionSelect = false;

        private Action<int> _onOptionSelected;

        private bool _isOptionWindow;

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
            _game = game;
            _continuePressed = options.ContinuePressed;
            _fastForwardPressed = options.FastForwardPressed;
            _dismiss = options.Dismiss;
            _scroll = options.Scroll;
            _background = options.Background;
            _bounds = options.Bounds;
            _padding = options.Padding;
            _sentenceSplitter = options.SentenceSplitter;

            _displayDialogueBoxWhileOptionsAreShowing = options.DisplayDialogueBoxWhileOptionsAreShowing;
            _optionQuickSelect = options.OptionQuickSelect;
            _optionSelectMove = options.OptionMoveSelection;
            _optionSelected = options.OptionSelected;
            _optionCanceled = options.OptionCanceled;

            _optionAllowScrollWrap = options.OptionAllowScrollWrap;
            _optionAutoScrollInitialWait = options.OptionAutoScrollInitialWait;
            _optionAutoScrollSecondaryWait = options.OptionAutoScrollSecondaryWait;
            _optionSelectedBackgroundPlayer.Animation = options.OptionBackground;
            _optionSelectedBackgroundPadding = options.OptionBackgroundPadding;
            _optionSelectedIconPlayer.Animation = options.OptionSelectIcon;
            _optionSelectedIconLocation = options.OptionSelectIconLocation;
            _optionSelectedIconOffset = options.OptionSelectIconOffset;
            _optionWindowLocation = options.OptionRenderLocation;
            _optionWindowMinBounds = options.OptionBoxMinBounds;
            _optionWindowMaxBounds = options.OptionBoxMaxBounds;
            _optionWindowOffset = options.OptionBoxOffset;
            _optionWindowPadding = options.OptionBoxPadding;
            _optionWindowAlwaysUseMaxBounds = options.AlwaysUseOptionBoxMaxBounds;
            _optionWindowBackground = options.OptionBoxWindowBackground;
            _isOptionWindow = options.IsOptionWindow;

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
            if(!options.IsOptionWindow)
                _texture = new RenderTarget2D(_spriteBatchState.GraphicsDevice, _bounds.Width, _bounds.Height);

            if (_optionWindowLocation == DialogueOptionRenderLocation.Inline)
            {
                _onOptionSelected = (index) =>
                {
                    if (_optionRemoveIgnoredEntriesOnSelect)
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
            if (_lastPaddingWasOption == isOption) return;

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
        }

        public void DetachFromRunner()
        {
            _runner!.DialogueStarted -= OnDialogueStarted;
            _runner!.LineNeedsPresented -= OnLineNeedsPresented;
            _runner!.DialogueCompleted -= OnDialogueCompleted;
            _runner!.OptionsNeedPresented -= OnOptionsNeedPresenting;
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
                var optionDisplay = new DialogueOptionDisplay()
                {
                    Line = frame,
                    DialogueOptionId = option.DialogueOptionId,
                    IsAvailable = option.IsAvailable
                };

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
            _firstLineFromStart = true;
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
            _state.LineTransition = _defaultTransition;
            _state.WaitForInput = _defaultWaitForInput;
            _line = 0;
            _column = 0;
            _waitingOnOptionSelect = false;

            var frame = CreateFrame(line, _state, false);

            if (_firstLineFromStart || _state.LineTransition != LineTransitionBehavior.NewLine)
            {
                _startingLineIndex = frame.LineStart;
                _firstLineFromStart = false;
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

        private void UpdateOption(float delta)
        {
            var movement = _optionSelectMove?.Invoke() ?? 0;
            var direction = Math.Sign(movement);

            if(movement != 0)
            {
                var moved = false;
                if(direction == _optionAutoScrollDirection)
                {
                    _optionAutoScrollTimer -= delta;
                    if(_optionAutoScrollTimer < 0)
                    {
                        moved = UpdateSelectedIndex(movement);
                        _optionAutoScrollTimer = moved ? _optionAutoScrollSecondaryWait : 100000;
                    }
                }
                else
                {
                    moved = UpdateSelectedIndex(movement);
                    _optionAutoScrollTimer = moved ? _optionAutoScrollInitialWait : 100000;
                }

                // Jump the view to the option if it's not visible.

                JumpToOption();

                if (moved)
                    Redraw();
            }

            _optionAutoScrollDirection = direction;

            var quickSelect = _optionQuickSelect?.Invoke() ?? -1;

            if(_optionSelected?.Invoke() ?? false)
            {
                _onOptionSelected?.Invoke(_optionSelectedIndex);
                return;
            }
            else if (_optionCanceled?.Invoke() ?? false)
            {
                var cancelOption = _options.FirstOrDefault(o => o.Line.Metadata.Contains("cancel"));
                if(cancelOption != null)
                {
                    _onOptionSelected?.Invoke(cancelOption.DialogueOptionId);
                    return;
                }
            }
            else if (quickSelect != -1)
            {
                _onOptionSelected?.Invoke(_options[quickSelect].DialogueOptionId);
                return;
            }

            if (_hasCustomProcessors || CheckScroll())
            {
                Redraw();
            }
        }

        private bool UpdateSelectedIndex(int movement)
        {
            var initialIndex = _optionSelectedIndex;

            var direction = Math.Sign(movement);
            var increment = direction * -1;
            for(; Math.Abs(movement) > 0; movement += increment)
            {
                _optionSelectedIndex += direction;
                if (_optionSelectedIndex < 0)
                {
                    if (_optionAllowScrollWrap)
                    {
                        _optionSelectedIndex = _options.Count - 1;
                    }
                    else
                    {
                        _optionSelectedIndex = initialIndex;
                        return false;
                    }
                }
                else if(_optionSelectedIndex >= _options.Count)
                {
                    if(_optionAllowScrollWrap)
                    {
                        _optionSelectedIndex = 0;
                    }
                    else
                    {
                        _optionSelectedIndex = initialIndex;
                        return false;
                    }
                }

                if (!_options[_optionSelectedIndex].IsAvailable)
                {
                    movement -= increment;
                }
            }

            return _optionSelectedIndex != initialIndex;
        }

        private void JumpToOption()
        {
            var frame = _options[_optionSelectedIndex].Line;
            var frameIndex = _frames.IndexOf(frame);

            if(frame.LineStart <= _startingLineIndex)
            {
                _startingLineIndex = frame.LineStart;
                return;
            }

            var startingFrameIndex = FindStartingFrame();
            var endingFrameIndex = startingFrameIndex;
            var totalHeight = 0;

            for(var i = startingFrameIndex + 1; i < _frames.Count; i++)
            {
                totalHeight += _frames[i].TotalSize.Height;
                if(totalHeight <= _state.DrawArea.Height)
                {
                    endingFrameIndex++;
                }
                else
                {
                    break;
                }
            }

            var endingFrame = _frames[endingFrameIndex];

            if(frame.LineStart + frame.Lines.Count > endingFrame.LineStart + endingFrame.Lines.Count)
            {
                _startingLineIndex = frame.LineStart + frame.Lines.Count - 1;
                totalHeight = 0;
                for(var i = frameIndex; i >= 0; i--)
                {
                    for(var line = _frames[i].Lines.Count - 1; line >= 0; line--)
                    {
                        totalHeight += _frames[i].Lines[line].Height;
                        if(totalHeight <= _state.DrawArea.Height)
                        {
                            _startingLineIndex = _frames[i].LineStart + line;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }

        private void SetOptionsForOptionsWindow(DialogueOption[] options, Rectangle parentBounds, Action<int> onOptionSelected)
        {
            _state.DrawArea = new Rectangle(
                _optionWindowPadding.Left,
                _optionWindowPadding.Top,
                _optionWindowMaxBounds.Width > 0
                    ? _optionWindowMaxBounds.Width - _optionWindowPadding.Width
                    : int.MaxValue / 2,
                _optionWindowMaxBounds.Height > 0
                    ? _optionWindowMaxBounds.Height - _optionWindowPadding.Height
                    : int.MaxValue / 2);

            PresentInlineOptions(options);

            _bounds.Size = MeasureOptions();
            CheckGenerateWindowTexture(_bounds.Size);
            RepositionWindow(parentBounds);

            Redraw();

            _onOptionSelected = onOptionSelected;
        }

        private Size MeasureOptions()
        {
            Size result = _optionWindowMinBounds - _optionWindowPadding.Size;
            result.Height = Math.Max(result.Height, _optionWindowPadding.Height);

            if (_optionWindowAlwaysUseMaxBounds)
            {
                if (_optionWindowMaxBounds.Width > 0 && _optionWindowMaxBounds.Height > 0)
                    return _optionWindowMaxBounds;

                if (_optionWindowMaxBounds.Width > 0)
                    result.Width = _optionWindowMaxBounds.Width;

                if (_optionWindowMaxBounds.Height > 0)
                    result.Height = _optionWindowMaxBounds.Height;
            }

            foreach (var option in _options.Select(o => o.Line))
            {
                result.Width = Math.Max(option.TotalSize.Width + _optionWindowPadding.Width, result.Width);
                if (_optionWindowMaxBounds.Width > 0)
                    result.Width = Math.Min(result.Width, _optionWindowMaxBounds.Width);

                //result.Height += option.TotalSize.Height + _margin;
                result.Height += option.TotalSize.Height;
                if (_optionWindowMaxBounds.Height > 0)
                    result.Height = Math.Min(result.Height, _optionWindowMaxBounds.Height);

                if (result == _optionWindowMaxBounds)
                    break;
            }

            return result;
        }

        private void RepositionWindow(Rectangle dialogueBounds)
        {
            Point position = dialogueBounds.Location;

            switch (_optionWindowLocation)
            {
                case DialogueOptionRenderLocation.AboveLeft:
                    position.Y -= _bounds.Height;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.AboveCenter:
                    position.Y -= _bounds.Height;
                    position.X += (dialogueBounds.Width - _bounds.Width) / 2;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.AboveRight:
                    position.Y -= _bounds.Height;
                    position.X += dialogueBounds.Width - _bounds.Width;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.BelowLeft:
                    position.Y += dialogueBounds.Height;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.BelowCenter:
                    position.Y += dialogueBounds.Height;
                    position.X += (dialogueBounds.Width - _bounds.Width) / 2;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.BelowRight:
                    position.Y += dialogueBounds.Height;
                    position.X += dialogueBounds.Width - _bounds.Width;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.LeftTop:
                    position.X -= _bounds.Width;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.LeftCenter:
                    position.X -= _bounds.Width;
                    position.Y += (dialogueBounds.Height - _bounds.Height) / 2;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.LeftBottom:
                    position.X -= _bounds.Width;
                    position.Y += (dialogueBounds.Height - _bounds.Height);
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.RightTop:
                    position.X += dialogueBounds.Width;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.RightCenter:
                    position.X += dialogueBounds.Width;
                    position.Y += (dialogueBounds.Height - _bounds.Height) / 2;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.RightBottom:
                    position.X += dialogueBounds.Width;
                    position.Y += (dialogueBounds.Height - _bounds.Height);
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.CustomTopLeftPosition:
                    position = _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.CustomTopRightPosition:
                    position.X = _optionWindowOffset.X - _bounds.Width;
                    position.Y = _optionWindowOffset.Y;
                    break;
                case DialogueOptionRenderLocation.CustomBottomLeftPosition:
                    position.X = _optionWindowOffset.X;
                    position.Y = _optionWindowOffset.Y - _bounds.Height;
                    break;
                case DialogueOptionRenderLocation.CustomBottomRightPosition:
                    position.X = _optionWindowOffset.X - _bounds.Width;
                    position.Y = _optionWindowOffset.Y - _bounds.Height;
                    break;
                case DialogueOptionRenderLocation.CustomCenterPosition:
                    position.X = _optionWindowOffset.X - _bounds.Width / 2;
                    position.Y = _optionWindowOffset.Y - _bounds.Height / 2;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid render position: {_optionWindowLocation}");
            }

            _bounds.Location = position;
        }

        private void CheckGenerateWindowTexture(Size size)
        {
            var windowWidth = _texture?.Width ?? 0;
            var windowHeight = _texture?.Height ?? 0;
            if (size.Width > windowWidth || size.Height > windowHeight)
            {
                windowWidth = Math.Max(size.Width, windowWidth);
                windowHeight = Math.Max(size.Height, windowHeight);

                _texture?.Dispose();

                _texture = new RenderTarget2D(_game.GraphicsDevice, windowWidth, windowHeight);
            }
        }

        private void UpdateText(float delta)
        {
            var continuePressed = _continuePressed?.Invoke() ?? false;
            var fastForward = _fastForwardPressed?.Invoke() ?? false;
            var needsRedraw = _hasCustomProcessors;

            if (Finished)
            {
                if (continuePressed || fastForward || !_state.WaitForInput)
                {
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

            return originalStartingLine != _startingLineIndex;
        }

        public void Draw(SpriteBatchState state)
        {
            if (!_running)
                return;

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

        private Rectangle CalculateSelectedBackgroundBounds(Rectangle optionBounds)
        {
            optionBounds.X -= _optionSelectedBackgroundPadding.Left;
            optionBounds.Y -= _optionSelectedBackgroundPadding.Top;
            optionBounds.Width += _optionSelectedBackgroundPadding.Width;
            optionBounds.Height += _optionSelectedBackgroundPadding.Height;

            return optionBounds;
        }

        private Rectangle CalculateSelectIconBounds(Rectangle optionBounds)
        {
            var icon = _optionSelectedIconPlayer.CurrentFrame;
            if (icon is null)
                return Rectangle.Empty;

            var optionSize = _options[_optionSelectedIndex].Line.TotalSize;

            var x = optionBounds.X + _optionSelectedIconOffset.X;
            var y = optionBounds.Y + _optionSelectedIconOffset.Y;
            var width = icon.Width;
            var height = icon.Height;

            switch (_optionSelectedIconLocation)
            {
                case SelectIconLocation.Left:
                    x -= icon.Width;
                    y += (optionSize.Height - icon.Height) / 2;
                    break;
                case SelectIconLocation.Right:
                    x += optionSize.Width;
                    y += (optionSize.Height - icon.Height) / 2;
                    break;
                case SelectIconLocation.Top:
                    x += (optionSize.Width - icon.Width) / 2;
                    y -= icon.Height;
                    break;
                case SelectIconLocation.Bottom:
                    x += (optionSize.Width - icon.Width) / 2;
                    y += optionSize.Height;
                    break;
                case SelectIconLocation.LeftStretch:
                case SelectIconLocation.RightStretch:
                case SelectIconLocation.TopStretch:
                case SelectIconLocation.BottomStretch:
                    // Todo: Implement option select icon stretch.
                    throw new NotImplementedException("Option select icon stretch not yet implemented");
            }

            return new Rectangle(x, y, width, height);
        }
    }
}
