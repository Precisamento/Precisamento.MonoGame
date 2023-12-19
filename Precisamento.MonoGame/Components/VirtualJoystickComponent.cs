using DefaultEcs;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Components
{
    /// <summary>
    /// Determines how the virtual joystick is positioned on screen.
    /// </summary>
    public enum JoystickMode
    {
        /// <summary>
        /// The joystick has a fixed location and doesn't move.
        /// </summary>
        Fixed,

        /// <summary>
        /// Everry time the joystick area is pressed, the joystick position is set on the touched position.
        /// </summary>
        Dynamic
    }

    public class VirtualJoystickComponent
    {
        public float Deadzone { get; set; }
        public JoystickMode JoystickMode { get; set; }
        public Rectangle TouchArea { get; set; }
        public float Radius { get; set; }
        public bool VisibleWhenNotPressed { get; set; }
        public bool Pressed { get; set; }
        public int TouchIndex { get; set; } = -1;
        public Vector2 Translation { get; set; }
        public Entity Base { get; set; }
        public Entity Tip { get; set; }

        public Vector2 Value
        {
            get
            {
                GetPositions(out var basePosition, out var tipPosition);
                return tipPosition - basePosition;
            }
        }

        public Directions Direction
        {
            get
            {
                if (!Pressed)
                    return Directions.None;

                var radians = MathExt.Direction(Value);
                return MathExt.DirectionFromRadians(radians);
            }
        }

        public float Magnitude
        {
            get
            {
                if (!Pressed)
                    return 0;
                return Math.Min(MathF.Round(Value.Length() / Radius), 1);
            }
        }

        public void Reset()
        {
            Pressed = false;
            TouchIndex = -1;
            ref var baseTransform = ref Base.Get<Transform2>();
            ref var tipTransform = ref Tip.Get<Transform2>();
            tipTransform.Position = baseTransform.Position;
        }

        public VirtualJoystickComponent()
        {
        }

        public VirtualJoystickComponent(World world, TextureRegion2D baseImg, TextureRegion2D tipImg, Vector2 position)
        {
            Base = world.CreateEntity();
            Tip = world.CreateEntity();

            var baseTransform = new Transform2(position);
            var tipTransform = new Transform2(position);

            Base.Set(baseImg);
            Base.Set(baseTransform);

            Tip.Set(tipImg);
            Tip.Set(tipTransform);

            Translation = new Vector2(baseImg.Width / 2, baseImg.Height / 2);
        }

        private void GetPositions(out Vector2 basePosition, out Vector2 tipPosition)
        {
            ref var baseTransform = ref Base.Get<Transform2>();
            ref var tipTransform = ref Tip.Get<Transform2>();

            basePosition = baseTransform.Position;
            tipPosition = tipTransform.Position;
        }
    }
}
