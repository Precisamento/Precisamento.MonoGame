using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Components
{
    public struct TextComponent
    {
        private IFont _font;
        private string _text;
        private Vector2 _size;
        private bool _dirty;

        public IFont Font
        {
            get => _font;
            set
            {
                if (value != _font)
                {
                    _font = value;
                    _dirty = true;
                }
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _dirty = true;
            }
        }

        public Vector2 Size
        {
            get
            {
                if (_dirty)
                    Clean();
                return _size;
            }
        }

        public Color TextColor { get; set; }

        public TextComponent(IFont font, string text)
        {
            _font = font;
            _text = text;
            _dirty = true;
            _size = Vector2.Zero;
            TextColor = Color.Black;
        }

        public TextComponent(IFont font, string text, Color color)
        {
            _font = font;
            _text = text;
            _dirty = true;
            _size = Vector2.Zero;
            TextColor = color;
        }

        private void Clean()
        {
            _size = _font.MeasureString(_text);
        }
    }
}
