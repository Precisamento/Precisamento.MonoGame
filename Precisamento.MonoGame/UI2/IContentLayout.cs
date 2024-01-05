using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public interface IContentLayout
    {
        Point Measure(Control control, Point availableSize);
        void Arrange(Control control, Rectangle bounds);
    }
}
