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
                {
                    _width = value;
                    _originalPoints = BuildBox(value, _height);
                    _dirty = true;
                }
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
                if (value == _height)
                    return;
                _height = value;
                _originalPoints = BuildBox(_width, value);
                _dirty = true;
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
                _width = value.Width;
                _height = value.Height;
                _originalPoints = BuildBox(_width, _height);
                _dirty = true;
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
    }
}
