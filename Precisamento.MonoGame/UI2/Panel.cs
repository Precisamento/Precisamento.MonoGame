using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public class Panel : Container
    {
        protected override void InternalArrange()
        {
            foreach(var control in ChildrenCopy)
            {
                if (!control.Visible)
                    continue;

                LayoutControl(control);
            }
        }

        private void LayoutControl(Control control)
        {
            control.Arrange(ActualBounds);
        }

        protected override Point InternalMeasure(Point availableSize)
        {
            var result = Point.Zero;

            foreach(var control in ChildrenCopy)
            {
                if (!control.Visible)
                    continue;

                var measure = control.Measure(availableSize);

                if (measure.X > result.X)
                    result.X = measure.X;

                if (measure.Y > result.Y)
                    result.Y = measure.Y;
            }

            return result;
        }
    }
}
