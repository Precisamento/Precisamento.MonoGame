using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Precisamento.MonoGame.Resources
{
    public class ResourceReader : BinaryReader
    {
        public ResourceLoader ResourceLoader { get; }

        public ResourceReader(ResourceLoader resourceLoader, Stream input)
            : base(input)
        {
            ResourceLoader = resourceLoader;
        }

        public ResourceReader(ResourceLoader resourceLoader, Stream input, Encoding encoding)
            : base(input, encoding)
        {
            ResourceLoader = resourceLoader;
        }

        public ResourceReader(ResourceLoader resourceLoader, Stream input, Encoding encoding, bool leaveOpen)
            : base(input, encoding, leaveOpen)
        {
            ResourceLoader = resourceLoader;
        }
    }
}
