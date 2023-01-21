using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Precisamento.MonoGame.Resources
{
    public abstract class ResourceTypeReader<T>
    {
        public abstract T Read(ResourceReader reader);
        public abstract string FileExtension { get; }
    }
}
