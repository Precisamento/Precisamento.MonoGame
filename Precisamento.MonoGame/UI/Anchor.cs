using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public struct Anchor
    {
        private float _top;
        private float _left;
        private float _bottom;
        private float _right;

        public const float Begin = 0f;
        public const float Center = 0.5f;
        public const float End = 1f;

        public float Top
        {
            get => _top;
            set
            {
                if (_top < 0 || _top > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _top = value;
            }
        }

        public float Left
        {
            get => _left;
            set
            {
                if (_left < 0 || _left > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _left = value;
            }
        }

        public float Bottom
        {
            get => _bottom;
            set
            {
                if (_bottom < 0 || _bottom > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _bottom = value;
            }
        }

        public float Right
        {
            get => _right;
            set
            {
                if (_right < 0 || _right > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _right = value;
            }
        }

        public Anchor(float top, float left, float bottom, float right)
        {
            _top = top;
            _left = left;
            _bottom = bottom;
            _right = right;
        }
    }
}
