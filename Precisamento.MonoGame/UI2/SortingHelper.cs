using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public static class SortingHelper
    {
        public class ZIndexComparer : IComparer<Control>
        {
            public int Compare(Control? x, Control? y)
            {
                return x!.ZIndex.CompareTo(y!.ZIndex);
            }
        }

        private static ZIndexComparer _indexComparer = new();

        public static void SortControlsByZIndex(List<Control> controls)
        {
            controls.Sort(_indexComparer);
        }
    }
}
