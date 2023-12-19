using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Compiler;

namespace Precisamento.MonoGame.Resources.Dialogue
{
    public class YarnDescription
    {
        public CompilationResult Result { get; }
        public YarnLocalization Locales { get; }

        public YarnDescription(CompilationResult result, YarnLocalization locales)
        {
            Result = result;
            Locales = locales;
        }
    }
}
