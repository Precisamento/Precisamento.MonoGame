using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Graphics
{
    public struct SpriteDrawParams
    {
        public Color Color { get; set; }
        public SpriteEffects Effects { get; set; }
        public float Depth { get; set; }
        public bool Invisible { get; set; }
    }
}
