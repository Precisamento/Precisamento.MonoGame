using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Resources
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ResourceImporterAttribute : Attribute
    {
        public Type Writer { get; }

        public ResourceImporterAttribute(Type writer)
        {
            Writer = writer;
        }
    }
}
