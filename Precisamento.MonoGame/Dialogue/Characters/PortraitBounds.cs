using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public enum PortraitBoundsType
    {
        ExactSize,
        SurroundImage,
    }

    public class PortraitBounds
    {
        public PortraitBoundsType BoundsType { get; set; }
        public Thickness Padding { get; set; }
        public Size2 Size { get; set; }
    }
}
