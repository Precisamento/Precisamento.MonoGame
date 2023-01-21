using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.MathHelpers;
using Precisamento.MonoGame.YarnSpinner;
using Soren.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue
{
    public enum DialogueOptionRenderLocation
    {
        InBox,
        AboveLeft,
        AboveCenter,
        AboveRight,
        BelowLeft,
        BelowCenter,
        BelowRight
    }

    public class DialogueBox
    {
        private const float MINUTE_IN_MS = 60000;
                         
        private Func<bool> _continuePressed;
        private Func<bool> _fastForwardPressed;
        private Func<int> _scroll;
        private Action _dismiss;
        private TextureRegion2D _background;
        private Rectangle _bounds;
        private DialogueState _state;
        private bool _running = false;
        private int _startingLineIndex = 0;
        private List<DialogueFrame> _frames = new List<DialogueFrame>();
        private int _line = 0;
        private int _column = 0;
        private DialogueRunner _runner;
        private float _ticks = 0;
        private bool _hasCustomProcessors = false;
        private RenderTarget2D _texture;
        private SpriteBatchState _spriteBatchState;
        private DialogueProcessorFactory _processorFactory;
        private LineTransitionBehavior _defaultTransition = LineTransitionBehavior.NewLine;
        private bool _defaultWaitForInput = true;
        private ISentenceSplitter _sentenceSplitter;

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
            _continuePressed = options.ContinuePressed;
            _fastForwardPressed = options.FastForwardPressed;
            _dismiss = options.Dismiss;
            _scroll = options.Scroll;
            _background = options.Background;
            _bounds = options.Bounds;
            _sentenceSplitter = options.SentenceSplitter;

            _state = new DialogueState(options.TextColor, options.Font)
            {
                TimePerLetter = options.TextSpeed / MINUTE_IN_MS,
                DrawArea = new Rectangle(
                    _bounds.X + options.Padding.Left,
                    _bounds.Y + options.Padding.Top,
                    _bounds.Width - options.Padding.Width,
                    _bounds.Height - options.Padding.Height),
            };

            _state.CurrentPosition = _state.DrawArea.Location.ToVector2();

            if (_state.DrawArea.Width < 0 || _state.DrawArea.Height < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(options.Padding),
                    "The text render bounds had a negative width or height.");
            }

            _spriteBatchState = game.Services.GetService<SpriteBatchState>();
            _processorFactory = new DialogueProcessorFactory(game);
            _texture = new RenderTarget2D(_spriteBatchState.GraphicsDevice, _bounds.Width, _bounds.Height);
        }

        public void AttachToRunner(DialogueRunner runner)
        {
            _runner = runner;
            _runner.DialogueStarted += OnDialogueStarted;
            _runner.LineNeedsPresented += OnLineNeedsPresented;
            _runner.DialogueCompleted += OnDialogueCompleted;
            _runner.OptionsNeedPresented += OnOptionsNeedPresenting;
        }

        private void OnOptionsNeedPresenting(object? sender, DialogueOption[] options)
        {

        }

        private void OnDialogueStarted(object? sender, EventArgs e)
        {
            _running = true;
        }

        private void OnDialogueCompleted(object? sender, EventArgs e)
        {
            _running = false;
            _dismiss?.Invoke();
        }

        private void OnLineNeedsPresented(object? sender, LocalizedLine line)
        {
            var startingFrameLine = _frames.Count == 0
                ? 0
                : _frames[^1].LineStart + _frames[^1].Lines.Count;

            var frame = new DialogueFrame(
                line,
                _processorFactory,
                _state,
                _sentenceSplitter,
                startingFrameLine);

            _line = 0;
            _column = 0;

            _frames.Add(frame);

            _state.LineTransition = _defaultTransition;
            _state.WaitForInput = _defaultWaitForInput;

            if (_state.LineTransition != LineTransitionBehavior.NewLine)
            {
                _startingLineIndex = frame.LineStart;
            }
            else
            {
                SetStartingDrawLine();
            }
        }

        public void Update(float delta)
        {
            if (!_running || _frames.Count == 0)
                return;

            UpdateText(delta);
        }

        private void UpdateText(float delta)
        {
            var continuePressed = _continuePressed?.Invoke() ?? false;
            var fastForward = _fastForwardPressed?.Invoke() ?? false;
            var needsRedraw = _hasCustomProcessors;

            if (Finished)
            {
                if (continuePressed || fastForward)
                {
                    _ticks = 0;
                    _runner.Continue();
                    return;
                }
                else if(_scroll != null)
                {
                    needsRedraw = CheckScroll();
                }
            }
            else if (fastForward)
            {
                var frame = _frames[^1];
                _line = frame.Lines.Count - 1;
                _column = frame.Lines[_line].Line.Length;
                SetStartingDrawLine();
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
                _spriteBatchState.SetRenderTarget(_texture);
                _spriteBatchState.GraphicsDevice.Clear(Color.Transparent);
                _spriteBatchState.Begin();
                _state.CurrentPosition = _state.DrawArea.Location.ToVector2() - _bounds.Location.ToVector2();
                DrawText(_spriteBatchState);
                _spriteBatchState.End();
                _spriteBatchState.SetRenderTarget(null);
            }
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
            var delta = _scroll();
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

            state.SpriteBatch.Draw(_background, _bounds, Color.White);
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
            for(var i = _frames.Count - 1; i >= 0; i--)
            {
                var frame = _frames[i];
                var frameEnd = frame.LineStart + frame.Lines.Count;
                if(frame.LineStart <= _startingLineIndex && frameEnd > _startingLineIndex)
                    return i;
            }

            return 0;
        }

        private bool DrawFrame(SpriteBatchState state, int frameIndex, int line, int column)
        {
            var frame = _frames[frameIndex];
            return frame.Draw(state, _state, _startingLineIndex, line, column);
        }
    }
}
