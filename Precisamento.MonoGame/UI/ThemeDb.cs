using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public class ThemeDb
    {
        private static ThemeDb _singleton;

        private Theme _defaultTheme;

        private float _fallbackBaseScale = 1;
        private IFont? _fallbackFont;
        private int _fallbackFontSize;
        private TextureRegion2D? _fallbackIcon;
        private StyleBox? _fallbackStyleBox;

        private ThemeContext? _defaultThemeContext;
        private Dictionary<string, ThemeContext> _themeContexts;


        private void PropogateThemeContext()
        {
            throw new NotImplementedException();
        }

        private void InitDefaultThemeContext()
        {
            throw new NotImplementedException();
        }

        private void FinalizeThemeContexts()
        {
            throw new NotImplementedException();
        }
    }
}
