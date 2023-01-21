using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Graphics
{
    public class SpriteFontWrapper : IFont
    {
        public int LineSpacing => Font.LineSpacing;
        public float Spacing => Font.Spacing;

        public SpriteFont Font { get; }

        public SpriteFontWrapper(SpriteFont font)
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
