using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Precisamento.MonoGame.Resources
{
    public abstract class ResourceTypeWriter
    {
        public virtual string FileExtension => ".bin";

        public abstract void Write(BinaryWriter writer, object value);
    }

    public abstract class ResourceTypeWriter<T> : ResourceTypeWriter
    {
        protected abstract void Write(BinaryWriter writer, T value);

        public sealed override void Write(BinaryWriter writer, object value)
        {
            Write(writer, (T)value);
        }
    }
}
