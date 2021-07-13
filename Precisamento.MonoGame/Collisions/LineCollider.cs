using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public enum LinePivotPoint
    {
        Start,
        Center,
        End
    }

    public class LineCollider : Collider
    {
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

        public override float Rotation 
        {
            get => _rotation;
            set
            {
                if(value != _rotation)
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
                if(value != _scale)
                {
                    AssertScale(value);
                    _scale = value;
                    _dirty = true;
                }
            }
        }

        public override Vector2 Position { get; set; }

        public override RectangleF BoundingBox 
        {
            get
            {
                if (_dirty)
                    Clean();
                return new RectangleF(_boundingBox.Position + Position, _boundingBox.Size);
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
                if(value != _originalStart)
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
                if(value != _originalEnd)
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

        public override void DebugDraw(SpriteBatch spriteBatch, Color color) => throw new NotImplementedException();

        private void Clean()
        {
            _dirty = false;
            _start = _originalStart;
            _end = _originalEnd;
            _center = _originalCenter;

            if(_scale != 1)
            {
                _start *= _scale;
                _end *= _scale;
                _center *= _scale;
            }

            if(_rotation != 0)
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
}
