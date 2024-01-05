using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public class SingleItemLayout : IContentLayout
    {
        public void Arrange(Control control, Rectangle bounds)
        {
            control.Arrange(bounds);
        }

        public Point Measure(Control control, Point availableSize)
        {
            return control.Measure(availableSize);
        }
    }
}
