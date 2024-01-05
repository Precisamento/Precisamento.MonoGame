using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public class Label : Control
    {
        private string _text = string.Empty;
        private bool _wrap = false;
        private IFont? _font;
        private List<string> _lines = new List<string>();
        private NoContextStringSplitter _splitter = new NoContextStringSplitter();

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text)
                    return;

                _text = value;
                InvalidateMeasure();
            }
        }

        public IFont? Font
        {
            get => _font;
            set
            {
                if (value == _font)
                    return;

                _font = value;
                InvalidateMeasure();
            }
        }

        public bool Wrap
        {
            get => _wrap;
            set
            {
                if (value == _wrap)
                    return;

                _wrap = value;
                InvalidateMeasure();
            }
        }

        public Color TextColor { get; set; } = Color.White;

        public Color? DisabledTextColor { get; set; }

        public Color? HoverTextColor { get; set; }

        public Color? FocusedTextColor { get; set; }

        public Color? PressedTextColor { get; set; }

        public Color GetTextColor()
        {
            var color = TextColor;

            if (!Enabled && DisabledTextColor.HasValue)
            {
                color = DisabledTextColor.Value;
            }
            else if (Parent is IPressable pressable && pressable.Pressed && PressedTextColor.HasValue)
            {
                color = PressedTextColor.Value;
            }
            else if (Hovering && HoverTextColor.HasValue)
            {
                color = HoverTextColor.Value;
            }
            else if (Focused && FocusedTextColor.HasValue)
            {
                color = FocusedTextColor.Value;
            }

            return color;
        }

        protected override void InternalDraw(SpriteBatchState state)
        {
            if (Font is null)
                return;

            var color = GetTextColor();

            state.SpriteBatch.DrawString(Font, Text, ActualBounds.Location.ToVector2(), color);
        }

        protected override Point InternalMeasure(Point availableSize)
        {
            if (Font is null)
                return Point.Zero;

            var result = Font.MeasureString(Text).ToPoint();

            if (result.X > availableSize.X)
                result.X = availableSize.X;

            if (result.Y > availableSize.Y)
                result.Y = availableSize.Y;

            return result;
        }
    }
}
