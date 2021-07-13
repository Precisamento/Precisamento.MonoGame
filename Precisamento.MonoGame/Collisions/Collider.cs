using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public enum ColliderType
    {
        Point,
        Line,
        Circle,
        Box,
        Polygon
    }

    public abstract partial class Collider
    {
        public abstract float Rotation { get; set; }
        public abstract float Scale { get; set; }
        public abstract Vector2 Position { get; set; }
        public abstract RectangleF BoundingBox { get; }
        public abstract ColliderType ColliderType { get; }

        protected void AssertScale(float scale)
        {
            if (scale < MathF.Epsilon)
                throw new ArgumentOutOfRangeException(nameof(scale), "Scale cannot be less than or equal to zero.");
        }

        public abstract bool Overlaps(Collider other);
        public abstract bool CollidesWithShape(Collider other, out CollisionResult collision, out RaycastHit ray);
        public abstract bool CollidesWithLine(Vector2 start, Vector2 end);
        public abstract bool CollidesWithLine(Vector2 start, Vector2 end, out RaycastHit hit);
        public abstract bool ContainsPoint(Vector2 point);
        public abstract bool CollidesWithPoint(Vector2 point, out CollisionResult result);
        public abstract void DebugDraw(SpriteBatch spriteBatch, Color color);
    }
}
