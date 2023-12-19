using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueColorProcessor : DialogueProcessor
    {
        public Color Color { get; private set; }

        public DialogueColorProcessor()
        {
        }

        public override void Init(Game game, MarkupAttribute attribute)
        {
            base.Init(game, attribute);

            var colorString = attribute.Properties[attribute.Name].StringValue;
            if(colorString.StartsWith("#"))
            {
                Color = ParseHexColor(colorString[1..]);
            }
            else
            {
                var type = typeof(Color);
                var properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
                var property = properties.FirstOrDefault(p => string.Equals(p.Name, colorString, StringComparison.OrdinalIgnoreCase));
                if (property != null)
                {
                    var obj = property.GetValue(null);
                    if(obj is not Color color)
                    {
                        throw GetColorError(colorString);
                    }

                    Color = color;
                }
                else
                {
                    Color = ParseHexColor(colorString);
                }
            }
        }

        private Color ParseHexColor(string color)
        {
            if (!uint.TryParse(color, out var hex))
                throw GetColorError(color);

            uint r, g, b, a = 255;

            switch(color.Length)
            {
                case 3:
                    r = (hex & 0xF00) >> 8;
                    r |= (r << 4);
                    g = (hex & 0x0F0) >> 4;
                    g |= (g << 4);
                    b = hex & 0x00F;
                    b |= (b << 4);
                    break;
                case 6:
                    r = (hex & 0xFF0000) >> 16;
                    g = (hex & 0xFF00) >> 8;
                    b = (hex & 0xFF);
                    break;
                case 8:
                    r = (hex & 0xFF000000) >> 24;
                    g = (hex & 0xFF0000) >> 16;
                    b = (hex & 0xFF00) >> 8;
                    a = (hex & 0xFF);
                    break;
                default:
                    throw GetColorError(color);
            }

            return new Color(r, g, b, a);
        }

        private Exception GetColorError(string color)
        {
            return new InvalidOperationException(
                $"Expected a color value on [color='value'] markup attribute. Got {color}");
        }

        public override void Push(DialogueState state)
        {
            state.Colors.Push(Color);
        }

        public override void Pop(DialogueState state)
        {
            state.Colors.Pop();
        }
    }
}
