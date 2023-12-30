using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public enum LayoutPreset
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        CenterTop,
        CenterLeft,
        CenterBottom,
        CenterRight,
        Center,
        TopWide,
        LeftWide,
        BottomWide,
        RightWide,
        VerticalCenterWide,
        HorizontalCenterWide,
        FullRect,
        CustomAnchors = -1
    }

    public enum LayoutPresetMode
    {
        MinSize,
        KeepWidth,
        KeepHeight,
        KeepSize
    }
}
