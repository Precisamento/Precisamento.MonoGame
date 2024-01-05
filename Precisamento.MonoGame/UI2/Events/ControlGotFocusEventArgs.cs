using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Events
{
    public class ControlGotFocusEventArgs : EventArgs
    {
        public Control Control { get; }

        public ControlGotFocusEventArgs(Control control)
        {
            Control = control;
        }
    }
}
