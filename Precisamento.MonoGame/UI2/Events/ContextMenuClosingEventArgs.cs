using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Events
{
    public class ContextMenuClosingEventArgs : ICancelableEvent
    {
        public bool Cancel { get; set; }
        public Control ContextMenu { get; }

        public ContextMenuClosingEventArgs(Control contextMenu)
        {
            ContextMenu = contextMenu;
        }
    }
}
