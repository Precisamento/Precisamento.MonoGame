using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.YarnSpinner
{
    public record struct CommandTriggeredArgs(string[] CommandElements, bool Handled);
}
