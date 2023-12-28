using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    [Flags]
    public enum DarkenNonSpeaker
    {
        None,
        Face = 1,
        Background = 2,
        Both = 3
    }
}
