using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Graphics
{
    public class BitmapFontWrapper : IFont
    {
        public int LineSpacing => Font.LineHeight;
        public float Spacing => Font.LetterSpacing;

        public BitmapFont Font { get; }

        public BitmapFontWrapper(BitmapFont font)
        {
            Font = font;
        }

        public Vector2 MeasureString(string text)
        {
            return Font.MeasureString(text);
        }

        public Vector2 MeasureString(StringBuilder text)
        {
            return Font.MeasureString(text);
        }

        public void Draw(SpriteBatch batch, string text, Vector2 position, Color color)
        {
            batch.DrawString(Font, text, position, color);
        }

        public void Draw(SpriteBatch batch, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            batch.DrawString(Font, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        public void Draw(SpriteBatch batch, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            batch.DrawString(Font, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        public void Draw(SpriteBatch batch, StringBuilder text, Vector2 position, Color color)
        {
            batch.DrawString(Font, text, position, color);
        }

        public void Draw(SpriteBatch batch, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            batch.DrawString(Font, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        public void Draw(SpriteBatch batch, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            batch.DrawString(Font, text, position, color, rotation, origin, scale, effects, layerDepth);
        }
    }
}
