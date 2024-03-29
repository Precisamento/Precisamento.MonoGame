﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public class PointCollider : Collider
    {
        // The Point Collider will "switch" to a box collider field when the Scale
        // value is increased.

        private Collider _internalCollider;
        private BoxCollider _box;
        private bool _dirty;
        private float _rotation;
        private float _scale;
        private Vector2 _position;

        public override float Rotation
        {
            get => _rotation;
            set
            {
                if(value != _rotation)
                {
                    _rotation = value;
                    if(InternalCollider != this)
                        InternalCollider.Rotation = value;
                }
            }
        }

        public override float Scale
        {
            get => _scale;
            set
            {
                if(value != _scale)
                {
                    AssertScale(value);
                    _scale = value;
                    _dirty = true;
                }
            }
        }
        public override Vector2 Position
        {
            get => _position;
            set
            {
                if(value != _position)
                {
                    _position = value;
                    if (InternalCollider != this)
                        InternalCollider.Position = value;
                }
            }
        }

        public Collider InternalCollider
        {
            get
            {
                if (_dirty)
                    Clean();
                return _internalCollider;
            }
        }

        public override RectangleF BoundingBox => new RectangleF(Position, new Size2(1, 1));

        public override ColliderType ColliderType => ColliderType.Point;

        public override bool Overlaps(Collider other)
        {
            if (InternalCollider != this)
                return InternalCollider.Overlaps(other);

            switch(other.ColliderType)
            {
                case ColliderType.Point:
                    var point = (PointCollider)other;
                    if (point.InternalCollider != point)
                        return point.InternalCollider.ContainsPoint(Position);

                    return Position == point.Position;
                case ColliderType.Line:
                    return CollisionChecks.PointToLine(this, (LineCollider)other);
                case ColliderType.Circle:
                    return CollisionChecks.PointToCircle(Position, (CircleCollider)other);
                case ColliderType.Box:
                    var box = (BoxCollider)other;
                    if (box.IsUnrotated)
                        return box.BoundingBox.Contains(Position);
                    else
                        return CollisionChecks.PointToPoly(Position, box);
                case ColliderType.Polygon:
                    return CollisionChecks.PointToPoly(Position, (PolygonCollider)other);
            }

            throw new NotImplementedException($"Overlaps of Point to {other.GetType()} are not supported.");
        }

        public override bool CollidesWithShape(Collider other, out CollisionResult collision, out RaycastHit ray)
        {
            ray = default;
            if (InternalCollider != this)
                return InternalCollider.CollidesWithShape(other, out collision, out ray);

            switch (other.ColliderType)
            {
                case ColliderType.Point:
                    var point = (PointCollider)other;
                    if (point.InternalCollider != point)
                        return point.InternalCollider.CollidesWithPoint(Position, out collision);

                    return CollisionChecks.PointToPoint(this, point, out collision);
                case ColliderType.Line:
                    return CollisionChecks.PointToLine(this, (LineCollider)other, out collision);
                case ColliderType.Circle:
                    return CollisionChecks.PointToCircle(Position, (CircleCollider)other, out collision);
                case ColliderType.Box:
                    var box = (BoxCollider)other;
                    if (box.IsUnrotated)
                        return CollisionChecks.PointToBox(Position, box, out collision);
                    else
                        return CollisionChecks.PointToPoly(Position, box, out collision);
                case ColliderType.Polygon:
                    return CollisionChecks.PointToPoly(Position, (PolygonCollider)other, out collision);
            }

            throw new NotImplementedException($"Collisions of Point to {other.GetType()} are not supported.");
        }

        public override bool CollidesWithRect(RectangleF rect)
        {
            if (InternalCollider != this)
                return InternalCollider.CollidesWithRect(rect);
            else
                return rect.Contains(Position);
        }

        public override bool CollidesWithRect(RectangleF rect, out CollisionResult result)
        {
            if (InternalCollider != this)
                return InternalCollider.CollidesWithRect(rect, out result);
            else
                return CollisionChecks.PointToBox(Position, rect, out result);
        }

        public override bool CollidesWithLine(Vector2 start, Vector2 end)
        {
            if (InternalCollider != this)
                return InternalCollider.CollidesWithLine(start, end);
            else
                return CollisionChecks.PointToLine(Position, start, end);
        }

        public override bool CollidesWithLine(Vector2 start, Vector2 end, out RaycastHit hit)
        {
            if(InternalCollider != this)
            {
                return InternalCollider.CollidesWithLine(start, end, out hit);
            }
            else
            {
                hit = default;
                return CollisionChecks.PointToLine(Position, start, end);
            }
        }

        public override bool ContainsPoint(Vector2 point)
        {
            if (InternalCollider != this)
                return InternalCollider.ContainsPoint(point);
            else
                return Position == point;
        }

        public override bool CollidesWithPoint(Vector2 point, out CollisionResult result)
        {
            if (InternalCollider != this)
                return InternalCollider.CollidesWithPoint(point, out result);
            else
                return CollisionChecks.PointToPoint(this, point, out result);
        }

        public override void DebugDraw(SpriteBatch spriteBatch, Color color)
        {
            if (InternalCollider != this)
                InternalCollider.DebugDraw(spriteBatch, color);
            else
                spriteBatch.DrawPoint(Position, color);
        }

        private void Clean()
        {
            _dirty = false;
            if(_scale <= 1)
            {
                _internalCollider = this;
            }
            else
            {
                var size = 2 * _scale + 1;

                if(_box is null)
                    _box = new BoxCollider(size, size);
                else
                    _box.Size = new Size2(size, size);

                _box.OriginalCenter = new Vector2(size / 2);
                _box.Rotation = _rotation;
                _box.Position = _position;

                _internalCollider = _box;
            }
        }
    }
}
