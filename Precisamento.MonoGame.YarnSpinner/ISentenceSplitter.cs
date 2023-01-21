using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.YarnSpinner
{
    public interface ISentenceSplitter
    {
        public string Separator { get; }

        public bool CanSplit(string sentence, int index);
    }
}
