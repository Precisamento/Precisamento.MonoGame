using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Events
{
    public interface ICancelableEvent
    {
        bool Cancel { get; set; }
    }
}
