using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public class Gui
    {
        public int ThreadId { get; set; }

        public Rectangle GetVisibleRect() => throw new NotImplementedException();
        internal void ThreadGuard() => throw new NotImplementedException();
    }
}
