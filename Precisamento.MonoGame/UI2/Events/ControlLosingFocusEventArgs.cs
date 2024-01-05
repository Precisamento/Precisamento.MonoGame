using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Events
{
    public class ControlLosingFocusEventArgs : ICancelableEvent
    {
        public bool Cancel { get; set; }
        public Control Control { get; }

        public ControlLosingFocusEventArgs(Control control)
        {
            Control = control;
        }
    }
}
