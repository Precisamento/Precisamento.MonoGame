using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public interface IContainerLayout
    {
        Point Measure(IEnumerable<Control> controls, Point availableSize);
        void Arrange(IEnumerable<Control> controls, Rectangle bounds);
    }
}
