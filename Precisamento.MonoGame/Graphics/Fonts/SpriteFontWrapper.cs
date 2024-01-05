using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static SpriteFontWrapper FromSprite(Sprite sprite, int? spacing = null, int? lineHeight = null, char? defaultCharacter = null)
        {
            var bounds = new List<Rectangle>();
            var cropping = new List<Rectangle>();
            var kerning = new List<Vector3>();
            var characters = new List<char>();
            Texture2D? texture = null;
            var height = 0;

            foreach(var kvp in sprite.Animations.Where(kvp => kvp.Key.Length == 1 && kvp.Value.Frames.Count == 1)
                .OrderBy(kvp => kvp.Key[0]))
            {
                var frame = kvp.Value.Frames[0];

                if (texture is null)
                {
                    texture = frame.Texture;
                }
                else
                {
                    if (texture != frame.Texture)
                        throw new ArgumentException("SpriteFont can only be created from a sprite that uses a single texture", nameof(sprite));
                }

                bounds.Add(frame.Bounds);
                cropping.Add(new Rectangle(Point.Zero, frame.Bounds.Size));
                kerning.Add(new Vector3(0, frame.Width, 0));
                characters.Add(kvp.Key[0]);
            }

            if (texture is null)
                throw new ArgumentException("Sprite had no valid characters", nameof(sprite));

            var font = new SpriteFont(texture, bounds, cropping, characters, lineHeight ?? height, spacing ?? 2, kerning, defaultCharacter);

            return new SpriteFontWrapper(font);
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
