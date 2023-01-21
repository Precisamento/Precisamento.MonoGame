using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Precisamento.MonoGame.Graphics
{
    public class SpriteAnimationPlayer
    {
        private const int COMPLETE_INDEX = -1;

        private SpriteAnimation _animation;
        private int _index = -1;
        private int _direction = 1;
        private float _ticks;
        private float _maxTicks;

        public SpriteAnimation Animation
        {
            get => _animation;
            set
            {
                if (value == _animation)
                    return;

                if (value is null)
                    throw new ArgumentNullException(nameof(value));

                Paused = false;
                _animation = value;
                _ticks = 0;
                _direction = 1;
                _maxTicks = value.FramesPerSecond == 0 ? 0 : 1f / _animation.FramesPerSecond;
                _index = value.StartFrameIndex;
            }
        }

        public TextureRegion2D CurrentFrame
        {
            get
            {
                if (_index < 0 || _index >= _animation.Frames.Count)
                    return null;

                return _animation.Frames[_index];
            }
        }

        public bool Paused { get; set; }
        public bool Completed => _index == COMPLETE_INDEX;

        public event Action CycleCompleted;

        public void Restart()
        {
            _ticks = 0;
            _direction = 1;
            _index = _animation.StartFrameIndex;
        }

        public void Update(float delta)
        {
            if (Paused || Completed || Animation is null)
                return;

            _ticks += delta;
            while(_ticks >= _maxTicks)
            {
                _ticks -= _maxTicks;
                NextFrame();
            }
        }

        private void NextFrame()
        {
            _index += _direction;
            if(_index >= _animation.Frames.Count || _index < 0)
            {
                switch(_animation.UpdateMode)
                {
                    case SpriteUpdateMode.PingPong:
                        _direction *= -1;
                        _index += _direction * 2;
                        break;
                    case SpriteUpdateMode.Cycle:
                        _index = 0;
                        break;
                    case SpriteUpdateMode.Once:
                        _index = COMPLETE_INDEX;
                        break;
                }

                CycleCompleted?.Invoke();
            }
        }

        public void Draw(SpriteBatchState state, Vector2 position)
        {
            if (Completed || Animation is null)
                return;

            state.SpriteBatch.Draw(
                Animation.Frames[_index].Texture,
                position,
                Animation.Frames[_index].Bounds,
                Color.White);
        }

        public void Draw(SpriteBatchState state, Transform2 transform)
        {
            if (Completed || Animation is null)
                return;

            state.SpriteBatch.Draw(
                Animation.Frames[_index].Texture,
                transform.Position,
                Animation.Frames[_index].Bounds,
                Color.White,
                transform.Rotation,
                Animation.Origin,
                transform.Scale,
                SpriteEffects.None,
                0f);
        }

        public void Draw(SpriteBatchState state, Vector2 position, ref SpriteDrawParams draw)
        {
            if (Completed || Animation is null || draw.Invisible)
                return;

            state.SpriteBatch.Draw(
                Animation.Frames[_index].Texture,
                position,
                Animation.Frames[_index].Bounds,
                draw.Color,
                0f,
                Animation.Origin,
                Vector2.One,
                draw.Effects,
                draw.Depth);
        }

        public void Draw(SpriteBatchState state, Transform2 transform, ref SpriteDrawParams draw)
        {
            if (Completed || Animation is null || draw.Invisible)
                return;

            state.SpriteBatch.Draw(
                Animation.Frames[_index].Texture,
                transform.Position,
                Animation.Frames[_index].Bounds,
                draw.Color,
                transform.Rotation,
                Animation.Origin,
                transform.Scale,
                draw.Effects,
                draw.Depth);
        }

        public RectangleF GetBounds(Vector2 position)
        {
            if (Animation is null)
                return RectangleF.Empty;

            return new RectangleF(position - _animation.Origin, _animation.Frames[_index].Size);
        }

        public RectangleF GetBounds(Vector2 position, Vector2 scale)
        {
            if (Animation is null)
                return RectangleF.Empty;

            return new RectangleF(position - _animation.Origin * scale, _animation.Frames[_index].Size * scale);
        }

        public RectangleF GetBounds(Transform2 transform)
            => GetBounds(transform.Position, transform.Scale, transform.Rotation);

        public RectangleF GetBounds(Vector2 position, Vector2 scale, float rotation)
        {
            if (rotation == 0)
                return GetBounds(position, scale);

            if (Animation is null)
                return RectangleF.Empty;

            var topLeft = -_animation.Origin * scale;
            var bottomRight = topLeft + _animation.Frames[_index].Size * scale;

            var topRight = new Vector2(bottomRight.X, topLeft.Y);
            var bottomLeft = new Vector2(topLeft.X, bottomRight.Y);

            var matrix = Matrix.CreateRotationZ(rotation);

            Vector2.Transform(ref topLeft, ref matrix, out topLeft);
            Vector2.Transform(ref bottomRight, ref matrix, out bottomRight);
            Vector2.Transform(ref topRight, ref matrix, out topRight);
            Vector2.Transform(ref bottomLeft, ref matrix, out bottomLeft);

            topLeft += position;
            bottomRight += position;
            topRight += position;
            bottomLeft += position;

            RectangleF rect;

            rect.X = BoundsMin(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
            rect.Y = BoundsMin(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);
            rect.Width = BoundsMax(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X) - rect.X;
            rect.Height = BoundsMax(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y) - rect.Y;

            return rect;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float BoundsMin(float v1, float v2, float v3, float v4)
        {
            return Math.Min(v1, Math.Min(v2, Math.Min(v3, v4)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float BoundsMax(float v1, float v2, float v3, float v4)
        {
            return Math.Max(v1, Math.Max(v2, Math.Max(v3, v4)));
        }
    }
}
