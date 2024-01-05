using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Graphics.Fonts
{
    public interface IStringSplitter
    {
        public IEnumerable<string> Split(string text, IFont font, Point availableSize);
    }
}
