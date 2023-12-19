using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Input
{
    public class InputManager
    {
        private KeyboardState _keyboardCurrent;
        private KeyboardState _keyboardPrevious;
        private MouseState _mouseCurrent;
        private MouseState _mousePrevious;
        private GamePadState[] _gamepadCurrent = new GamePadState[GamePad.MaximumGamePadCount];
        private GamePadState[] _gamepadPrevious = new GamePadState[GamePad.MaximumGamePadCount];
        private float[] _leftAxisDeadZones = new float[GamePad.MaximumGamePadCount];
        private float[] _rightAxisDeadZones = new float[GamePad.MaximumGamePadCount];
        private List<int> _connectedGamePads = new List<int>();

        public event EventHandler<int> GamePadConnected;
        public event EventHandler<int> GamePadDisconnected;

        public IReadOnlyList<int> ConnectedGamePads => _connectedGamePads;
        public Point MousePosition => _mouseCurrent.Position;
        public bool MouseMoved => _mouseCurrent.Position != _mousePrevious.Position;
        public int MouseVerticalScrollWheelValue => _mouseCurrent.ScrollWheelValue;
        public int MouseHorizontalScrollWheelValue => _mouseCurrent.HorizontalScrollWheelValue;

        public TouchCollection TouchCurrent { get; set; }
        public TouchCollection TouchPrevious { get; set; }

        public InputManager()
        {
            for (int i = 0; i < GamePad.MaximumGamePadCount; i++)
            {
                var state = GamePad.GetState(i);
                if (state.IsConnected)
                    _connectedGamePads.Add(i);
            }
        }

        public bool KeyCheck(Keys key)
        {
            return _keyboardCurrent.IsKeyDown(key);
        }

        public bool KeyCheckPressed(Keys key)
        {
            return _keyboardCurrent.IsKeyDown(key) && _keyboardPrevious.IsKeyUp(key);
        }

        public bool KeyCheckReleased(Keys key)
        {
            return _keyboardCurrent.IsKeyUp(key) && _keyboardPrevious.IsKeyDown(key);
        }

        public bool MouseCheck(MouseButtons buttons)
        {
            switch (buttons)
            {
                case MouseButtons.WheelDown:
                    return _mouseCurrent.ScrollWheelValue > _mousePrevious.ScrollWheelValue;
                case MouseButtons.WheelUp:
                    return _mouseCurrent.ScrollWheelValue < _mousePrevious.ScrollWheelValue;
                case MouseButtons.WheelLeft:
                    return _mouseCurrent.HorizontalScrollWheelValue < _mousePrevious.HorizontalScrollWheelValue;
                case MouseButtons.WheelRight:
                    return _mouseCurrent.HorizontalScrollWheelValue > _mousePrevious.HorizontalScrollWheelValue;
                default:
                    return _mouseCurrent.IsButtonDown(buttons);
            }
        }

        public bool MouseCheckPressed(MouseButtons buttons)
        {
            switch (buttons)
            {
                case MouseButtons.WheelDown:
                    return _mouseCurrent.ScrollWheelValue > _mousePrevious.ScrollWheelValue;
                case MouseButtons.WheelUp:
                    return _mouseCurrent.ScrollWheelValue < _mousePrevious.ScrollWheelValue;
                case MouseButtons.WheelLeft:
                    return _mouseCurrent.HorizontalScrollWheelValue < _mousePrevious.HorizontalScrollWheelValue;
                case MouseButtons.WheelRight:
                    return _mouseCurrent.HorizontalScrollWheelValue > _mousePrevious.HorizontalScrollWheelValue;
                default:
                    return _mouseCurrent.IsButtonDown(buttons) && _mousePrevious.IsButtonUp(buttons);
            }
        }

        public bool MouseCheckReleased(MouseButtons buttons)
        {
            switch (buttons)
            {
                case MouseButtons.WheelDown:
                case MouseButtons.WheelUp:
                case MouseButtons.WheelLeft:
                case MouseButtons.WheelRight:
                    return false;
                default:
                    return _mouseCurrent.IsButtonUp(buttons) && _mousePrevious.IsButtonDown(buttons);
            }
        }

        public bool GamePadCheck(Buttons button, int index)
        {
            switch (button)
            {
                case Buttons.LeftThumbstickDown:
                    return _gamepadCurrent[index].ThumbSticks.Left.Y < -_leftAxisDeadZones[index];
                case Buttons.LeftThumbstickLeft:
                    return _gamepadCurrent[index].ThumbSticks.Left.X < -_leftAxisDeadZones[index];
                case Buttons.LeftThumbstickUp:
                    return _gamepadCurrent[index].ThumbSticks.Left.Y > _leftAxisDeadZones[index];
                case Buttons.LeftThumbstickRight:
                    return _gamepadCurrent[index].ThumbSticks.Left.X > _leftAxisDeadZones[index];
                case Buttons.RightThumbstickDown:
                    return _gamepadCurrent[index].ThumbSticks.Right.Y < -_rightAxisDeadZones[index];
                case Buttons.RightThumbstickLeft:
                    return _gamepadCurrent[index].ThumbSticks.Right.X < -_rightAxisDeadZones[index];
                case Buttons.RightThumbstickUp:
                    return _gamepadCurrent[index].ThumbSticks.Right.Y > _rightAxisDeadZones[index];
                case Buttons.RightThumbstickRight:
                    return _gamepadCurrent[index].ThumbSticks.Right.X > _rightAxisDeadZones[index];
                default:
                    return _gamepadCurrent[index].IsButtonDown(button);
            }
        }

        public bool GamePadCheckPressed(Buttons button, int index)
        {
            switch (button)
            {
                case Buttons.LeftThumbstickDown:
                    return _gamepadCurrent[index].ThumbSticks.Left.Y < -_leftAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Left.Y > -_leftAxisDeadZones[index];
                case Buttons.LeftThumbstickLeft:
                    return _gamepadCurrent[index].ThumbSticks.Left.X < -_leftAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Left.X > -_leftAxisDeadZones[index];
                case Buttons.LeftThumbstickUp:
                    return _gamepadCurrent[index].ThumbSticks.Left.Y > _leftAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Left.Y < _leftAxisDeadZones[index];
                case Buttons.LeftThumbstickRight:
                    return _gamepadCurrent[index].ThumbSticks.Left.X > _leftAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Left.X < _leftAxisDeadZones[index];
                case Buttons.RightThumbstickDown:
                    return _gamepadCurrent[index].ThumbSticks.Right.Y < -_rightAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Right.Y > -_rightAxisDeadZones[index];
                case Buttons.RightThumbstickLeft:
                    return _gamepadCurrent[index].ThumbSticks.Right.X < -_rightAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Right.X > -_rightAxisDeadZones[index];
                case Buttons.RightThumbstickUp:
                    return _gamepadCurrent[index].ThumbSticks.Right.Y > _rightAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Right.Y < _rightAxisDeadZones[index];
                case Buttons.RightThumbstickRight:
                    return _gamepadCurrent[index].ThumbSticks.Right.X > _rightAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Right.X < _rightAxisDeadZones[index];
                default:
                    return _gamepadCurrent[index].IsButtonDown(button) &&
                           _gamepadPrevious[index].IsButtonUp(button);
            }
        }

        public bool GamePadCheckReleased(Buttons button, int index)
        {
            switch (button)
            {
                case Buttons.LeftThumbstickDown:
                    return _gamepadCurrent[index].ThumbSticks.Left.Y > -_leftAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Left.Y < -_leftAxisDeadZones[index];
                case Buttons.LeftThumbstickLeft:
                    return _gamepadCurrent[index].ThumbSticks.Left.X > -_leftAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Left.X < -_leftAxisDeadZones[index];
                case Buttons.LeftThumbstickUp:
                    return _gamepadCurrent[index].ThumbSticks.Left.Y < _leftAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Left.Y > _leftAxisDeadZones[index];
                case Buttons.LeftThumbstickRight:
                    return _gamepadCurrent[index].ThumbSticks.Left.X < _leftAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Left.X > _leftAxisDeadZones[index];
                case Buttons.RightThumbstickDown:
                    return _gamepadCurrent[index].ThumbSticks.Right.Y > -_rightAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Right.Y < -_rightAxisDeadZones[index];
                case Buttons.RightThumbstickLeft:
                    return _gamepadCurrent[index].ThumbSticks.Right.X > -_rightAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Right.X < -_rightAxisDeadZones[index];
                case Buttons.RightThumbstickUp:
                    return _gamepadCurrent[index].ThumbSticks.Right.Y < _rightAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Right.Y > _rightAxisDeadZones[index];
                case Buttons.RightThumbstickRight:
                    return _gamepadCurrent[index].ThumbSticks.Right.X < _rightAxisDeadZones[index] &&
                           _gamepadPrevious[index].ThumbSticks.Right.X > _rightAxisDeadZones[index];
                default:
                    return _gamepadCurrent[index].IsButtonUp(button) &&
                           _gamepadPrevious[index].IsButtonDown(button);
            }
        }

        public float GamePadLeftTrigger(int index)
        {
            return _gamepadCurrent[index].Triggers.Left;
        }

        public float GamePadRightTrigger(int index)
        {
            return _gamepadCurrent[index].Triggers.Right;
        }

        public Vector2 GamePadLeftStick(int index)
        {
            Vector2 axis = _gamepadCurrent[index].ThumbSticks.Left;
            axis.X = Math.Abs(axis.X) >= GamePadGetLeftAxisDeadZone(index) ? axis.X : 0;
            axis.Y = Math.Abs(axis.Y) >= GamePadGetLeftAxisDeadZone(index) ? axis.Y : 0;
            return axis;
        }

        public Vector2 GamePadRightStick(int index)
        {
            Vector2 axis = _gamepadCurrent[index].ThumbSticks.Right;
            axis.X = Math.Abs(axis.X) >= GamePadGetRightAxisDeadZone(index) ? axis.X : 0;
            axis.Y = Math.Abs(axis.Y) >= GamePadGetRightAxisDeadZone(index) ? axis.Y : 0;
            return axis;
        }

        public void GamePadSetLeftAxisDeadZone(float deadzone, int index)
        {
            _leftAxisDeadZones[index] = deadzone;
        }

        public float GamePadGetLeftAxisDeadZone(int index)
        {
            return _leftAxisDeadZones[index];
        }

        public void GamePadSetRightAxisDeadZone(float deadzone, int index)
        {
            _rightAxisDeadZones[index] = deadzone;
        }

        public float GamePadGetRightAxisDeadZone(int index)
        {
            return _rightAxisDeadZones[index];
        }

        public void Update()
        {
            _keyboardPrevious = _keyboardCurrent;
            _keyboardCurrent = Keyboard.GetState();
            _mousePrevious = _mouseCurrent;
            _mouseCurrent = Mouse.GetState();
            TouchPrevious = TouchCurrent;
            TouchCurrent = TouchPanel.GetState();

            for (int i = 0; i < GamePad.MaximumGamePadCount; i++)
            {
                _gamepadPrevious[i] = _gamepadCurrent[i];
                _gamepadCurrent[i] = GamePad.GetState(i);
                if (_gamepadCurrent[i].IsConnected != _gamepadPrevious[i].IsConnected)
                {
                    if (_gamepadCurrent[i].IsConnected)
                    {
                        _connectedGamePads.Add(i);
                        GamePadConnected?.Invoke(this, i);
                    }
                    else
                    {
                        _connectedGamePads.Remove(i);
                        GamePadDisconnected?.Invoke(this, i);
                    }
                }
            }
        }

        public static ActionSystem<float> CreateInputSystem(Game game)
        {
            var input = game.Services.GetService<InputManager>();
            var actions = game.Services.GetService<IActionManager>();

            if (input is null || actions is null)
            {
                throw new InvalidOperationException($"Game must have both a " +
                    $"{nameof(InputManager)} and a " +
                    $"{nameof(IActionManager)} registered as services " +
                    $"in order to create an Input System");
            }

            return new ActionSystem<float>(_ =>
            {
                input.Update();
                actions.Update();
            });
        }
    }
}
