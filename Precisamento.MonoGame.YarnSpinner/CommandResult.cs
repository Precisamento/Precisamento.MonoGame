using Precisamento.MonoGame.YarnSpinner.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.YarnSpinner
{
    public class CommandResult
    {
        public bool Complete { get; set; }

        public static Pool<CommandResult> Pool { get; }
            = new Pool<CommandResult>(() => new CommandResult(), cr => cr.Complete = false, true);
    }
}
