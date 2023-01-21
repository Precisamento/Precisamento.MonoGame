using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.YarnSpinner
{
    public class SeparatorSplitter : ISentenceSplitter
    {
        public string Separator { get; }

        public SeparatorSplitter()
        {
            Separator = " ";
        }

        public SeparatorSplitter(string separator)
        {
            Separator = separator;
        }

        public bool CanSplit(string sentence, int index)
        {
            return string.Compare(sentence, index, Separator, 0, Separator.Length) == 0;
        }

        public List<string> Split(string sentence)
        {
            var results = new List<string>();
            Split(sentence, results);
            return results;
        }

        public void Split(string sentence, List<string> output)
        {
            string[] splitResults;
            if (Separator.Length == 1)
                splitResults = sentence.Split(Separator[0]);
            else
                splitResults = sentence.Split(Separator);

            output.AddRange(splitResults);
        }
    }
}
