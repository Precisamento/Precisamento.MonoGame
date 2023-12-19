using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public class BoxCollider : PolygonCollider
    {
        private float _width;
        private float _height;

        /// <summary>
        /// The width of the <see cref="BoxCollider"/> before being scaled.
        /// </summary>
        public float Width
        {
            get => _width;
            set
            {
                if(value != _width)
                    SetSize(value, _height);
            }
        }

        /// <summary>
        /// The width of the <see cref="BoxCollider"/> after being scaled.
        /// </summary>
        public float ScaledWidth => _width * Scale;

        /// <summary>
        /// The height of the <see cref="BoxCollider"/> before being scaled.
        /// </summary>
        public float Height
        {
            get => _height;
            set
            {
                if (value != _height)
                    SetSize(_width, value);
            }
        }

        /// <summary>
        /// The height of the <see cref="BoxCollider"/> after being scaled.
        /// </summary>
        public float ScaledHeight => _height * Scale;

        public Size2 Size
        {
            get => new Size2(_width, _height);
            set
            {
                if (value.Width == _width && value.Height == _height)
                    return;
                SetSize(_width, _height);
            }
        }

        public BoxCollider(float width, float height)
            : base(BuildBox(width, height))
        {
            _width = width;
            _height = height;
        }

        public BoxCollider(Size2 size)
            : base(BuildBox(size.Width, size.Height))
        {
            _width = size.Width;
            _height = size.Height;
        }

        public override bool Overlaps(Collider other)
        {
            if(IsUnrotated)
            {
                switch(other.ColliderType)
                {
                    case ColliderType.Point:
                        var point = (PointCollider)other;
                        if (point.InternalCollider != point)
                            return Overlaps(point.InternalCollider);
                        return BoundingBox.Contains(point.Position);
                    case ColliderType.Circle:
                        return CollisionChecks.CircleToBox((CircleCollider)other, this);
                    case ColliderType.Box:
                        var box = (BoxCollider)other;
                        if (box.IsUnrotated)
                            return BoundingBox.Intersects(other.BoundingBox);
                        else
                            return CollisionChecks.PolygonToPolygon(this, (PolygonCollider)other);
                }
            }

            return base.Overlaps(other);
        }

        public override bool CollidesWithShape(Collider other, out CollisionResult collision, out RaycastHit ray)
        {
            ray = default;
            if (IsUnrotated)
            {
                switch (other.ColliderType)
                {
                    case ColliderType.Point:
                        var point = (PointCollider)other;
                        if (point.InternalCollider != point)
                            return CollidesWithShape(point.InternalCollider, out collision, out ray);
                        return CollisionChecks.PointToBox(point.Position, this, out collision);
                    case ColliderType.Circle:
                        return CollisionChecks.CircleToBox((CircleCollider)other, this, out collision);
                    case ColliderType.Box:
                        var box = (BoxCollider)other;
                        if (box.IsUnrotated)
                            return CollisionChecks.BoxToBox(this, box, out collision);
                        else
                            return CollisionChecks.PolygonToPolygon(this, (PolygonCollider)other, out collision);
                }
            }

            return base.CollidesWithShape(other, out collision, out ray);
        }

        public override bool CollidesWithRect(RectangleF rect)
        {
            if (IsUnrotated)
                return BoundingBox.Intersects(rect);
            else
                return base.CollidesWithRect(rect);
        }

        public override bool CollidesWithRect(RectangleF rect, out CollisionResult result)
        {
            if (IsUnrotated)
                return CollisionChecks.BoxToBox(this, rect, out result);
            else
                return base.CollidesWithRect(rect, out result);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            if (IsUnrotated)
                return BoundingBox.Contains(point);
            else
                return base.ContainsPoint(point);
        }

        public override bool CollidesWithPoint(Vector2 point, out CollisionResult result)
        {
            if (IsUnrotated)
                return CollisionChecks.PointToBox(point, this, out result);
            else
                return base.CollidesWithPoint(point, out result);
        }

        private void SetSize(float width, float height)
        {
            _width = width;
            _height = height;
            _dirty = true;
            _originalPoints[1] = new Vector2(width, 0);
            _originalPoints[2] = new Vector2(width, height);
            _originalPoints[3] = new Vector2(0, height);
        }
    }
}
