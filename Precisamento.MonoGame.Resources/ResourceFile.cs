using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Resources
{
    public class ResourceFile
    {
        public string InputFile { get; set; }
        public string? OutputFile { get; set; }

        public ResourceFile(string inputFile)
        {
            InputFile = inputFile;
        }

        public ResourceFile(string inputFile, string? outputFile)
        {
            InputFile = inputFile;
            OutputFile = outputFile;
        }
    }
}
