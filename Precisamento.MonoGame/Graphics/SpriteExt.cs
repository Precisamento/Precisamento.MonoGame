using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Graphics
{
    public static class SpriteExt
    {
        public static RectangleF Bounds(this Sprite sprite, Vector2 position)
        {
            return new RectangleF(position - sprite.Origin, sprite.TextureRegion.Size);
        }

        public static RectangleF Bounds(this Sprite sprite, Vector2 position, Vector2 scale)
        {
            return new RectangleF(position - sprite.Origin * scale, sprite.TextureRegion.Size * scale);
        }

        public static RectangleF Bounds(this Sprite sprite, Vector2 position, float rotation, Vector2 scale)
        {
            if (rotation <= MathF.Epsilon && scale == Vector2.One)
                return Bounds(sprite, position);

            return sprite.GetBoundingRectangle(position, rotation, scale);
        }

        public static RectangleF Bounds(this Sprite sprite, Transform2 transform)
        {
            return Bounds(sprite, transform.Position, transform.Rotation, transform.Scale);
        }
    }
}
