using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Events
{
    public class ContextMenuClosedEventArgs : EventArgs
    {
        public Control ContextMenu { get; }

        public ContextMenuClosedEventArgs(Control contextMenu)
        {
            ContextMenu = contextMenu;
        }
    }
}
