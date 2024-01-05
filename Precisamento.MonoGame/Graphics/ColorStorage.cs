using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Graphics
{
    public class ColorStorage
    {
        private static ColorStorage? _instance = null;

        public Dictionary<string, Color> Colors { get; } = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);

        public static ColorStorage Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = MakeWithMonoGameColors();
                }

                return _instance;
            }
            set => _instance = value;
        }

        public static ColorStorage MakeWithMonoGameColors()
        {
            var result = new ColorStorage();
            var type = typeof(Color);
            foreach (var color in type.GetRuntimeProperties())
            {
                if (color.PropertyType != typeof(Color))
                    continue;

                var value = (Color)color.GetValue(null)!;
                result.Colors.Add(color.Name, value);
            }

            return result;
        }

        public Color? FromName(string name)
        {
            if (name.StartsWith('#'))
            {
                name = name.Substring(1);
                if (uint.TryParse(name, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var packedColor))
                {
                    // Parsed value contains color in RGBA form
                    // Extract color components

                    byte r = 0, g = 0, b = 0, a = 0;

                    unchecked
                    {
                        if (name.Length == 6)
                        {
                            r = (byte)(packedColor >> 16);
                            g = (byte)(packedColor >> 8);
                            b = (byte)packedColor;
                            a = 255;
                        }
                        else if (name.Length == 8)
                        {
                            r = (byte)(packedColor >> 24);
                            g = (byte)(packedColor >> 16);
                            b = (byte)(packedColor >> 8);
                            a = (byte)packedColor;
                        }
                    }

                    return new Color(r, g, b, a);
                }
            }
            else if (Colors.TryGetValue(name, out var result))
            {
                return result;
            }

            return null;
        }
    }
}
