using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public class ThemeData
    {
        private Dictionary<string, TextureRegion2D>? _icons;
        private Dictionary<string, StyleBox>? _styles;
        private Dictionary<string, IFont>? _fonts;
        private Dictionary<string, int>? _fontSizes;
        private Dictionary<string, Color>? _colors;
        private Dictionary<string, int>? _constants;

        public Dictionary<string, TextureRegion2D> Icons
        {
            get
            {
                _icons ??= new Dictionary<string, TextureRegion2D>();
                return _icons;
            }
        }

        public Dictionary<string, StyleBox> Styles
        {
            get
            {
                _styles ??= new Dictionary<string, StyleBox>();
                return _styles;
            }
        }

        public Dictionary<string, IFont> Fonts
        {
            get
            {
                _fonts ??= new Dictionary<string, IFont>();
                return _fonts;
            }
        }

        public Dictionary<string, int> FontSizes
        {
            get
            {
                _fontSizes ??= new Dictionary<string, int>();
                return _fontSizes;
            }
        }

        public Dictionary<string, Color> Colors
        {
            get
            {
                _colors ??= new Dictionary<string, Color>();
                return _colors;
            }
        }

        public Dictionary<string, int> Constants
        {
            get
            {
                _constants ??= new Dictionary<string, int>();
                return _constants;
            }
        }
    }
}
