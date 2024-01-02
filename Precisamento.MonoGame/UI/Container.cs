using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public class Container : Control
    {
        public ObservableCollection<Control> Children { get; } = new ObservableCollection<Control>();
    }
}
