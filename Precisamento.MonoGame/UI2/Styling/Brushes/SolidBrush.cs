using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Styling.Brushes
{
    public class SolidBrush : IBrush, IBorderBrush
    {
        public Color Color { get; set; }

        public SolidBrush(Color color)
        {
            Color = color;
        }

        public SolidBrush(string color)
        {
            var c = ColorStorage.Instance.FromName(color)
                ?? throw new ArgumentException($"Invalid color {color}", nameof(color));

            Color = c;
        }

        public void Update(float delta)
        {
        }

        public void Draw(SpriteBatchState state, Rectangle rect, Color color)
        {
            if (color == Color.White)
            {
                state.SpriteBatch.FillRectangle(rect, Color);
            }
            else
            {
                var c = new Color(
                    (int)(Color.R * color.R / 255f),
                    (int)(Color.G * color.G / 255f),
                    (int)(Color.B * color.B / 255f),
                    (int)(Color.A * color.A / 255f));

                state.SpriteBatch.FillRectangle(rect, c);
            }
        }

        public void Draw(SpriteBatchState state, Rectangle rect, Thickness borderSize, Color color)
        {
            if (color == Color.White)
            {
                color = Color;
            }
            else
            {
                color = new Color(
                    (int)(Color.R * color.R / 255f),
                    (int)(Color.G * color.G / 255f),
                    (int)(Color.B * color.B / 255f),
                    (int)(Color.A * color.A / 255f));
            }

            if (borderSize.Left > 0)
            {
                state.SpriteBatch.FillRectangle(
                    new Rectangle(rect.X, rect.Y, borderSize.Left, rect.Height), 
                    color);
            }

            if (borderSize.Top > 0)
            {
                state.SpriteBatch.FillRectangle(
                    new Rectangle(rect.X, rect.Y, rect.Width, borderSize.Top),
                    color);
            }

            if (borderSize.Right > 0)
            {
                state.SpriteBatch.FillRectangle(
                    new Rectangle(rect.Right - borderSize.Right, rect.Y, borderSize.Right, rect.Height),
                    color);
            }

            if (borderSize.Bottom > 0)
            {
                state.SpriteBatch.FillRectangle(
                    new Rectangle(rect.X, rect.Bottom - borderSize.Bottom, rect.Width, borderSize.Bottom),
                    color);
            }
        }
    }
}
