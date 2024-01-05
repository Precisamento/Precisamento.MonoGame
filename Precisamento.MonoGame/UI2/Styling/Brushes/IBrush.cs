using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Styling.Brushes
{
    public interface IBrush
    {
        void Update(float delta);
        void Draw(SpriteBatchState state, Rectangle dest, Color color);
    }

    public static class BrushUtils
    {
        public static void Draw(SpriteBatchState state, IBrush brush, Rectangle border, Thickness borderSize, Color color)
        {
            if (brush is IBorderBrush borderBrush)
            {
                borderBrush.Draw(state, border, borderSize, color);
                return;
            }

            if (borderSize.Left > 0)
            {
                brush.Draw(
                    state,
                    new Rectangle(border.X, border.Y, borderSize.Left, border.Height),
                    color);
            }

            if (borderSize.Top > 0)
            {
                brush.Draw(
                    state,
                    new Rectangle(border.X, border.Y, border.Width, borderSize.Top),
                    color);
            }

            if (borderSize.Right > 0)
            {
                brush.Draw(
                    state,
                    new Rectangle(border.Right - borderSize.Right, border.Y, borderSize.Right, border.Height),
                    color);
            }

            if (borderSize.Bottom > 0)
            {
                brush.Draw(
                    state,
                    new Rectangle(border.X, border.Bottom - borderSize.Bottom, border.Width, borderSize.Bottom),
                    color);
            }
        }
    }
}
