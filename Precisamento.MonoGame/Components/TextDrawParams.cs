using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Components
{
    public struct TextDrawParams
    {
        public Vector2 Offset { get; set; }
        public Vector2 Origin { get; set; }
        public Color Color { get; set; }
        public SpriteEffects Effects { get; set; }
        public float Depth { get; set; }
        public bool Invisible { get; set; }
    }
}
