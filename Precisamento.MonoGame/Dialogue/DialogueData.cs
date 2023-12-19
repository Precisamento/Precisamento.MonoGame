using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueData
    {
        public Program YarnProgram { get; }
        public YarnLocalization Localization { get; }

        public DialogueData(Program yarnProgram, YarnLocalization localization)
        {
            YarnProgram = yarnProgram;
            Localization = localization;
        }
    }
}
