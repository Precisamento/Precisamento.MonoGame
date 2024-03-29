﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public class CircleCollider : Collider
    {
        private float _originalRadius;
        private float _radius;
        private RectangleF _boundingBox;
        private Vector2 _position;
        private bool _dirty;
        private float _scale = 1;

        public override ColliderType ColliderType => ColliderType.Circle;

        public float OriginalRadius
        {
            get => _originalRadius;
            set
            {
                if(value != _originalRadius)
                {
                    _originalRadius = value;
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
                    _dirty = true;
                }
            }
        }

        public float Radius
        {
            get
            {
                if (_dirty)
                    Clean();
                return _radius;
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

        // Rotation is a noop with circle colliders.
        public override float Rotation { get; set; }

        public override RectangleF BoundingBox
        {
            get
            {
                if (_dirty)
                    Clean();
                return _boundingBox;
            }
        }

        public CircleCollider(float radius)
        {
            OriginalRadius = radius;
        }

        public override bool Overlaps(Collider other)
        {
            switch(other.ColliderType)
            {
                case ColliderType.Point:
                    var point = (PointCollider)other;
                    if (point.InternalCollider != point)
                        return Overlaps(point.InternalCollider);
                    return ContainsPoint(point.Position);
                case ColliderType.Line:
                    return CollisionChecks.LineToCircle((LineCollider)other, this);
                case ColliderType.Circle:
                    return CollisionChecks.CircleToCircle(this, (CircleCollider)other);
                case ColliderType.Box:
                    var box = (BoxCollider)other;
                    if (box.IsUnrotated)
                        return CollisionChecks.CircleToBox(this, (BoxCollider)other);
                    else
                        return CollisionChecks.CircleToPolygon(this, box);
                case ColliderType.Polygon:
                    return CollisionChecks.CircleToPolygon(this, (PolygonCollider)other);
            }

            throw new NotImplementedException($"Overlaps of Circle to {other.GetType()} are not supported.");
        }

        public override bool CollidesWithShape(Collider other, out CollisionResult collision, out RaycastHit ray)
        {
            ray = default;
            switch (other.ColliderType)
            {
                case ColliderType.Point:
                    var point = (PointCollider)other;
                    if (point.InternalCollider != point)
                        return CollidesWithShape(point.InternalCollider, out collision, out ray);
                    return CollisionChecks.PointToCircle(point.Position, this, out collision);
                case ColliderType.Line:
                    collision = default;
                    return CollisionChecks.LineToCircle((LineCollider)other, this, out ray);
                case ColliderType.Circle:
                    return CollisionChecks.CircleToCircle(this, (CircleCollider)other, out collision);
                case ColliderType.Box:
                    var box = (BoxCollider)other;
                    if (box.IsUnrotated)
                        return CollisionChecks.CircleToBox(this, (BoxCollider)other, out collision);
                    else
                        return CollisionChecks.CircleToPolygon(this, box, out collision);
                case ColliderType.Polygon:
                    return CollisionChecks.CircleToPolygon(this, (PolygonCollider)other, out collision);
            }

            throw new NotImplementedException($"Overlaps of Circle to {other.GetType()} are not supported.");
        }

        public override bool CollidesWithRect(RectangleF rect)
            => CollisionChecks.CircleToBox(this, rect);

        public override bool CollidesWithRect(RectangleF rect, out CollisionResult result)
            => CollisionChecks.CircleToBox(this, rect, out result);

        public override bool CollidesWithLine(Vector2 start, Vector2 end)
            => CollisionChecks.LineToCircle(start, end, this);

        public override bool CollidesWithLine(Vector2 start, Vector2 end, out RaycastHit hit)
            => CollisionChecks.LineToCircle(start, end, this, out hit);

        public override bool CollidesWithPoint(Vector2 point, out CollisionResult result)
            => CollisionChecks.PointToCircle(point, this, out result);

        public override bool ContainsPoint(Vector2 point)
            => CollisionChecks.PointToCircle(point, this);

        public override void DebugDraw(SpriteBatch spriteBatch, Color color) 
            => Primitives2D.DrawCircle(spriteBatch, Position, Radius, 16, color);

        private void Clean()
        {
            if (!_dirty)
                return;

            _radius = _originalRadius * _scale;
            _boundingBox = new RectangleF(Position.X - _radius, Position.Y - _radius, _radius * 2, _radius * 2);

            _dirty = false;
        }
    }
}
