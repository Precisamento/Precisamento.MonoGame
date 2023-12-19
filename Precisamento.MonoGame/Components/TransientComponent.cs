using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Components
{
    /// <summary>
    /// Tag component that determines if the entities collider needs to be
    /// considered when performing a collision resolution. Can be used for things
    /// like piercing bullets that will overlap with enemies but won't push them away.
    /// </summary>
    public struct TransientComponent
    {
    }
}
