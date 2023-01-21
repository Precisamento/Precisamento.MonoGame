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

    public abstract class Collider
    {
        /// <summary>
        /// Used to perform rectangle collisions when there isn't a more optimized solution.
        /// </summary>
        private static BoxCollider _box = new BoxCollider(0, 0);

        /// <summary>
        /// The rotation of the collider in radians.
        /// </summary>
        public abstract float Rotation { get; set; }

        /// <summary>
        /// Gets or sets the uniform scale of the collider.
        /// </summary>
        public abstract float Scale { get; set; }

        /// <summary>
        /// Gets or sets the position of the collider in the world.
        /// </summary>
        public abstract Vector2 Position { get; set; }

        /// <summary>
        /// Gets the maximum bounding box of the collider.
        /// </summary>
        public abstract RectangleF BoundingBox { get; }

        /// <summary>
        /// Gets the type of the collider.
        /// </summary>
        public abstract ColliderType ColliderType { get; }

        /// <summary>
        /// A generic tag that can be used to hold context information about the collider, such as what object it's attached to.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// A generic id that can be used to hold context information about the collider, such as the ID of the object it's attached to.
        /// </summary>
        public int Id { get; set; }

        protected void AssertScale(float scale)
        {
            if (scale < MathExt.Epsilon)
                throw new ArgumentOutOfRangeException(nameof(scale), "Scale cannot be less than or equal to zero.");
        }

        public virtual bool CollidesWithRect(RectangleF rect)
        {
            _box.Position = rect.Position;
            _box.Size = rect.Size;
            return Overlaps(_box);
        }

        public virtual bool CollidesWithRect(RectangleF rect, out CollisionResult result)
        {
            _box.Position = rect.Position;
            _box.Size = rect.Size;
            return CollidesWithShape(_box, out result, out _);
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
