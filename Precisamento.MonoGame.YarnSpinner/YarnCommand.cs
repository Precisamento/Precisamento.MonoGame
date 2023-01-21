using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.YarnSpinner
{
    [AttributeUsage(AttributeTargets.Method)]
    public class YarnCommandAttribute : Attribute
    {
        public string? Name { get; }

        public YarnCommandAttribute(string? name = null)
        {
            Name = name;
        }
    }
}
