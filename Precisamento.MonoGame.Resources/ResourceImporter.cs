using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Resources
{
    public abstract class ResourceImporter
    {
        public abstract object Import(string fileName);

        public abstract string FileExtension { get; }
    }

    public abstract class ResourceImporter<T> : ResourceImporter
    {
        protected abstract T ImportImpl(string fileName);

        public sealed override object Import(string fileName)
        {
            return ImportImpl(fileName);
        }
    }
}
