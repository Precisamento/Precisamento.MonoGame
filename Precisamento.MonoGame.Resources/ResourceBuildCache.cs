using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Resources
{
    public class ResourceBuildCache
    {
        public class CachedFile
        {
            public string Input { get; set; }
            public string Output { get; set; }
            public string ConverterName { get; set; }
            public DateTime LastEdited { get; set; }
        }

        public List<CachedFile> CachedFiles { get; set; } = new List<CachedFile>();
    }
}
