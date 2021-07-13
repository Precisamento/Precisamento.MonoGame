using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
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

        public override void DebugDraw(SpriteBatch spriteBatch, Color color) 
            => spriteBatch.DrawPoint(Position, color);

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

                _box.Center = new Vector2(size / 2);
                _box.Rotation = _rotation;
                _box.Position = _position;

                _internalCollider = _box;
            }
        }
    }
}
