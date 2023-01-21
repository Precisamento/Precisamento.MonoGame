using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Resources
{
    public class ResourceProcessor
    {
        private readonly List<ResourceConverter> _converters;
        private bool _stopOnError;

        public event EventHandler<ResourceProcessDirectoryEventArgs> ProcessingDirectory;
        public event EventHandler<ResourceProcessDirectoryErrorEventArgs> ProcessDirectoryError;
        public event EventHandler<ResourceProcessDirectoryEventArgs> ProcessedDirectory;
        public event EventHandler<ResourceProcessFileEventArgs> ProcessingFile;
        public event EventHandler<ResourceProcessFileErrorEventArgs> ProcessFileError;
        public event EventHandler<ResourceProcessFileEventArgs> ProcessedFile;

        public ResourceProcessor(IEnumerable<ResourceConverter> converters)
        {
            _converters = new List<ResourceConverter>(converters);
        }

        public bool ProcessDirectory(string inputDirectory, string outputDirectory)
        {
            ProcessingDirectory?.Invoke(this, new ResourceProcessDirectoryEventArgs(inputDirectory));
            
            if(!Directory.Exists(inputDirectory))
            {
                var error = new ArgumentException($"Input directory {inputDirectory} did not exist.", nameof(inputDirectory));
                ProcessDirectoryError?.Invoke(this, new ResourceProcessDirectoryErrorEventArgs(error, inputDirectory));
                return false;
            }

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            foreach(var file in Directory.EnumerateFiles(inputDirectory))
            {
                var converter = GetConverterForFile(file);
                if (converter is null)
                    continue;
                try
                {
                    var args = new ResourceProcessFileEventArgs(file, converter);

                    ProcessingFile?.Invoke(this, args);

                    PerformConversion(file, outputDirectory, converter);

                    ProcessedFile?.Invoke(this, args);
                }
                catch(Exception e)
                {
                    ProcessFileError?.Invoke(this, new ResourceProcessFileErrorEventArgs(e, file, converter));

                    if (_stopOnError)
                        return false;
                }
            }

            foreach (var dir in Directory.EnumerateDirectories(inputDirectory))
            {
                var dirName = Path.GetDirectoryName(dir.TrimEnd('\\', '/'));
                var nextOutputDirectory = Path.Combine(outputDirectory, dirName);

                var result = ProcessDirectory(dir, nextOutputDirectory);
                if (_stopOnError && !result)
                {
                    ProcessedDirectory?.Invoke(this, new ResourceProcessDirectoryEventArgs(inputDirectory));
                    return false;
                }
            }

            ProcessedDirectory?.Invoke(this, new ResourceProcessDirectoryEventArgs(inputDirectory));

            return true;
        }

        public void ProcessFile(string inputFile, string outputDirectory)
        {
            var converter = GetConverterForFile(inputFile);
            if (converter is null)
                throw new InvalidOperationException($"No {nameof(ResourceConverter)} defined for file type {Path.GetExtension(inputFile)}");

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            PerformConversion(inputFile, outputDirectory, converter);
        }

        private ResourceConverter GetConverterForFile(string file)
        {
            return _converters.FirstOrDefault(rc => rc.Importer.FileExtension == Path.GetExtension(file));
        }

        private void PerformConversion(string inputFile, string outputDirectory, ResourceConverter converter)
        {
            converter.Convert(inputFile, Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(inputFile) + ".bin"));
        }
    }
}
