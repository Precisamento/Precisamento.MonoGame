using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Components
{
    public struct NinePatchComponent
    {
        public Rectangle Bounds { get; set; }

        public NinePatchComponent(Rectangle bounds)
        {
            Bounds = bounds;
        }
    }
}
