using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Collisions
{
    /*
    public class CapsuleCollider : Collider
    {
        private float _originalRadius;
        private float _radius;
        private float _rotation = 0;
        private float _scale = 1;
        private Vector2 _center;
        private Vector2 _originalCenter;
        private RectangleF _boundingBox;
        private bool _dirty;
        private Vector2 _originalStart;
        private Vector2 _originalEnd;
        private Vector2 _start;
        private Vector2 _end;
        private Vector2 _position;

        public float OriginalRadius
        {
            get => _originalRadius;
            set
            {
                if (value != _originalRadius)
                {
                    _originalRadius = value;
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

        public override float Rotation
        {
            get => _rotation;
            set
            {
                if (value != _rotation)
                {
                    _rotation = value;
                    _dirty = true;
                }
            }
        }

        public override float Scale
        {
            get => _scale;
            set
            {
                if (value != _scale)
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
                if (value != _position)
                {
                    _position = value;
                }
            }
        }

        public Vector2 Start
        {
            get
            {
                if (_dirty)
                    Clean();
                return _start;
            }
        }

        public Vector2 End
        {
            get
            {
                if (_dirty)
                    Clean();
                return _end;
            }
        }

        public Vector2 Center
        {
            get
            {
                if (_dirty)
                    Clean();
                return _center;
            }
        }

        public Vector2 AdjustedStart => Start + Position;
        public Vector2 AdjustedEnd => End + Position;

        public Vector2 OriginalStart
        {
            get => _originalStart;
            set
            {
                if (value != _originalStart)
                {
                    _originalStart = value;
                    _dirty = true;
                }
            }
        }

        public Vector2 OriginalEnd
        {
            get => _originalEnd;
            set
            {
                if (value != _originalEnd)
                {
                    _originalEnd = value;
                    _dirty = true;
                }
            }
        }

        public Vector2 OriginalCenter
        {
            get => _originalCenter;
            set
            {
                if (value != _originalCenter)
                {
                    if (!Collisions.PointToLine(value, OriginalStart, OriginalEnd))
                        throw new ArgumentOutOfRangeException(
                            nameof(value),
                            $"OriginalCenter must be on the line between " +
                                $"OriginalStart ({OriginalStart}) and OriginalEnd ({OriginalEnd}).");

                    _originalCenter = value;
                    _dirty = true;
                }
            }
        }

        public override void DebugDraw(SpriteBatch spriteBatch, Color color)
        {
            var normal = Vector2.Normalize(End - Start);
            var perp = normal.PerpendicularClockwise();

            var r1 = Start + normal * Radius;
            var r2 = End - normal * Radius;

            var c1 = r1 + perp * Radius;
            var c2 = r1 - perp * Radius;
            var c3 = r2 + perp * Radius;
            var c4 = r2 - perp * Radius;

            spriteBatch.DrawLine(Start, End, color);

            spriteBatch.DrawPolygon(
                            Vector2.Zero,
                            new List<Vector2>()
                            {
                                c1, c2, c4, c3
                            },
                            color);

            spriteBatch.DrawCircle(r1, Radius, 24, color);
            spriteBatch.DrawCircle(r2, Radius, 24, color);
        }

        private void Clean()
        {
            _dirty = false;
            _start = _originalStart;
            _end = _originalEnd;
            _center = _originalCenter;

            if (_scale != 1)
            {
                _start *= _scale;
                _end *= _scale;
                _center *= _scale;
            }

            if (_rotation != 0)
            {
                var sin = MathF.Sin(_rotation);
                var cos = MathF.Cos(_rotation);

                _start -= _center;
                _start.X = _start.X * cos - _start.Y * sin;
                _start.Y = _start.X * sin + _start.Y * cos;
                _start += _center;

                _end -= _center;
                _end.X = _end.X * cos - _end.Y * sin;
                _end.Y = _end.X * sin + _end.Y * cos;
                _end += _center;
            }

            var minX = Math.Min(_start.X, _start.X);
            var minY = Math.Min(_start.Y, _start.Y);
            var maxX = Math.Max(_start.X, _start.X);
            var maxY = Math.Min(_start.Y, _start.Y);

            _boundingBox = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }
    */
}
