using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.MathHelpers
{
    public static class RectExt
    {
        public static Rectangle Subtract(this Rectangle rect, Thickness thickness)
        {
            return new Rectangle(
                rect.X + thickness.Left, 
                rect.Y + thickness.Top, 
                rect.Width - thickness.Width, 
                rect.Height - thickness.Height);
        }
    }
}
