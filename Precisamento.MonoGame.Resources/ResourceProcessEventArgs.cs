using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Resources
{
    public class ResourceProcessFileEventArgs
    {
        public string Name { get; }
        public ResourceConverter Converter { get; }

        public ResourceProcessFileEventArgs(string name, ResourceConverter converter)
        {
            Name = name;
            Converter = converter;
        }
    }

    public class ResourceProcessFileCompleteEventArgs : ResourceProcessFileEventArgs
    {
        public string OutputFile { get; }

        public ResourceProcessFileCompleteEventArgs(string input, string output, ResourceConverter converter)
            : base(input, converter)
        {
            OutputFile = output;
        }
    }

    public class ResourceProcessFileErrorEventArgs : ResourceProcessFileEventArgs
    {
        public Exception Exception { get; }

        public ResourceProcessFileErrorEventArgs(Exception exception, string name, ResourceConverter converter)
            : base(name, converter)
        {
            Exception = exception;
        }
    }

    public class ResourceProcessDirectoryEventArgs
    {
        public string Name { get; }

        public ResourceProcessDirectoryEventArgs(string name)
        {
            Name = name;
        }
    }

    public class ResourceProcessDirectoryErrorEventArgs : ResourceProcessDirectoryEventArgs
    {
        public Exception Exception { get; }

        public ResourceProcessDirectoryErrorEventArgs(Exception exception, string name)
            : base(name)
        {
            Exception = exception;
        }
    }
}
