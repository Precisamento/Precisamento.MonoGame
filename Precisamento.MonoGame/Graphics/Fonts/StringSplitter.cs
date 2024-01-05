using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Graphics.Fonts
{
    public abstract class StringSplitter : IStringSplitter
    {
        public abstract IEnumerable<string> Split(string text, IFont font, Point availableSize);

        public int GetNextLineEnd(string text, int start)
        {
            var index = text.IndexOf('\n', start);
            return index == -1 ? text.Length - start : index - start;
        }
    }
}
