using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.Systems.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Compiler;

namespace Precisamento.MonoGame.Systems
{
    public class JoystickUpdateSystem : AComponentSystem<float, VirtualJoystickComponent>
    {
        private InputManager _inputManager;
        private OrthographicCamera? _camera;
        private bool _mouseSimulatesTouch = true;
        private DebugDisplay _debug;

        public JoystickUpdateSystem(World world, Game game, OrthographicCamera? camera) : base(world)
        {
            _inputManager = game.Services.GetService<InputManager>();
            _camera = camera;
            _debug = game.Services.GetService<DebugDisplay>();
        }

        protected override void PreUpdate(float delta)
        {
        }

        protected override void Update(float state, ref VirtualJoystickComponent joystick)
        {
            if (!joystick.Base.IsEnabled() || !joystick.Tip.IsEnabled())
                return;

            foreach (var touch in GetTouches())
            {
                switch (touch.State)
                {
                    case TouchLocationState.Pressed:
                        if (joystick.TouchIndex != -1)
                            break;

                        if (!TouchInJoystickArea(touch.Position, ref joystick))
                            break;

                        if (!TouchInBase(touch.Position, ref joystick))
                            break;

                        if (joystick.JoystickMode == JoystickMode.Dynamic)
                            MoveBase(touch.Position, ref joystick);

                        joystick.TouchIndex = touch.Id;
                        UpdatePosition(touch.Position, ref joystick);

                        break;
                    case TouchLocationState.Released:
                        if (touch.Id != joystick.TouchIndex)
                            break;
                        joystick.Reset();
                        break;
                    case TouchLocationState.Moved:
                        if (touch.Id != joystick.TouchIndex)
                            break;

                        UpdatePosition(touch.Position, ref joystick);
                        break;
                }
            }
        }

        private bool TouchInJoystickArea(Vector2 position, ref VirtualJoystickComponent joystick)
        {
            return joystick.TouchArea.Contains(position);
        }

        private void MoveBase(Vector2 position, ref VirtualJoystickComponent joystick)
        {
            ref Transform2 transform = ref joystick.Base.Get<Transform2>();
            transform.Position = position + joystick.Translation;
        }

        private bool TouchInBase(Vector2 touchPosition, ref VirtualJoystickComponent joystick)
        {
            if (joystick.JoystickMode == JoystickMode.Dynamic)
                return true;

            ref Transform2 transform = ref joystick.Base.Get<Transform2>();

            return CollisionChecks.PointToCircle(
                touchPosition,
                transform.Position + joystick.Translation,
                joystick.Radius);
        }

        private void UpdatePosition(Vector2 position, ref VirtualJoystickComponent joystick)
        {
            ref Transform2 transform = ref joystick.Base.Get<Transform2>();

            var delta = position - transform.Position - joystick.Translation;
            delta = delta.Truncate(joystick.Radius);

            MoveTip(delta, ref joystick);

            if (delta.LengthSquared() > joystick.Deadzone * joystick.Deadzone)
            {
                joystick.Pressed = true;
            }
            else
            {
                joystick.Pressed = false;
            }
        }

        private void MoveTip(Vector2 delta, ref VirtualJoystickComponent joystick)
        {
            ref Transform2 tipTransform = ref joystick.Tip.Get<Transform2>();
            ref Transform2 baseTransform = ref joystick.Base.Get<Transform2>();

            tipTransform.Position = baseTransform.Position + delta;

        }

        private IEnumerable<TouchLocation> GetTouches()
        {
            IEnumerable<TouchLocation> touches = _inputManager.TouchCurrent;
            if (_mouseSimulatesTouch)
                SimulateJoystickFromMouse(ref touches);

            return touches;
        }

        private void SimulateJoystickFromMouse(ref IEnumerable<TouchLocation> touches)
        {
            if (_inputManager.MouseCheckPressed(MouseButtons.Left))
            {
                touches = touches.Append(new TouchLocation(1, TouchLocationState.Pressed, _inputManager.MousePosition.ToVector2()));
            }
            else if (_inputManager.MouseMoved && _inputManager.MouseCheck(MouseButtons.Left))
            {
                touches = touches.Append(new TouchLocation(1, TouchLocationState.Moved, _inputManager.MousePosition.ToVector2()));
            }
            else if (_inputManager.MouseCheckReleased(MouseButtons.Left))
            {
                touches = touches.Append(new TouchLocation(1, TouchLocationState.Released, _inputManager.MousePosition.ToVector2()));
            }
        }
    }
}
