using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Graphics
{
    public static class FontExtensions
    {
        public static void DrawString(this SpriteBatch batch, IFont font, string text, Vector2 position, Color color)
        {
            font.Draw(batch, text, position, color);
        }

        public static void DrawString(this SpriteBatch batch, IFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            font.Draw(batch, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        public static void DrawString(this SpriteBatch batch, IFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            font.Draw(batch, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        public static void DrawString(this SpriteBatch batch, IFont font, StringBuilder text, Vector2 position, Color color)
        {
            font.Draw(batch, text, position, color);
        }

        public static void DrawString(this SpriteBatch batch, IFont font, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            font.Draw(batch, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        public static void DrawString(this SpriteBatch batch, IFont font, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            font.Draw(batch, text, position, color, rotation, origin, scale, effects, layerDepth);
        }
    }
}
