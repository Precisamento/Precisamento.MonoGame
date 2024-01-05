using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Events
{
    public class RenderEventArgs : EventArgs
    {
        public SpriteBatchState State { get; }

        public RenderEventArgs(SpriteBatchState state)
        {
            State = state;
        }
    }
}
