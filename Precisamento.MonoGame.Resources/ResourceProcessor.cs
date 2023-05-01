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
        private readonly ResourceBuildCache _buildCache;
        private bool _stopOnError;

        public event EventHandler<ResourceProcessDirectoryEventArgs>? ProcessingDirectory;
        public event EventHandler<ResourceProcessDirectoryErrorEventArgs>? ProcessDirectoryError;
        public event EventHandler<ResourceProcessDirectoryEventArgs>? ProcessedDirectory;
        public event EventHandler<ResourceProcessFileEventArgs>? ProcessingFile;
        public event EventHandler<ResourceProcessFileErrorEventArgs>? ProcessFileError;
        public event EventHandler<ResourceProcessFileCompleteEventArgs>? ProcessedFile;
        public event EventHandler<string>? NoConverterFound;
        public event EventHandler<ResourceBuildCache.CachedFile>? SkippingCachedFile;

        public ResourceProcessor(IEnumerable<ResourceConverter> converters, ResourceBuildCache buildCache)
        {
            _converters = new List<ResourceConverter>(converters);
            _buildCache = buildCache;
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
                {
                    NoConverterFound?.Invoke(this, file);
                    continue;
                }

                try
                {
                    var outputFile = Path.Combine(outputDirectory, Path.GetFileName(file));

                    outputFile = ReplaceFileExtensionWithConvertor(outputFile, converter);

                    var cachedFile = _buildCache.CachedFiles.FirstOrDefault(cf => FileIsCached(cf, file, outputFile, converter));
                    if(cachedFile is not null)
                    {
                        SkippingCachedFile?.Invoke(this, cachedFile);
                        continue;
                    }

                    ProcessingFile?.Invoke(this, new ResourceProcessFileEventArgs(file, converter));

                    PerformConversion(file, outputFile, converter);

                    ProcessedFile?.Invoke(this, new ResourceProcessFileCompleteEventArgs(file, outputFile, converter));
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
                var dirName = Path.GetDirectoryName(dir.TrimEnd('\\', '/'))!;
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

        private bool FileIsCached(ResourceBuildCache.CachedFile cachedFile, string input, string output, ResourceConverter converter)
        {
            return cachedFile.Input == input
                && cachedFile.Output == output
                && cachedFile.ConverterName == converter.GetType().FullName
                && File.Exists(output)
                && File.GetLastWriteTimeUtc(input) == cachedFile.LastEdited;
        }

        public void ProcessFile(string inputFile)
        {
            var converter = GetConverterForFile(inputFile)
                ?? throw new InvalidOperationException(
                    $"No {nameof(ResourceConverter)} defined for file type {Path.GetExtension(inputFile)}");

            var output = ReplaceFileExtensionWithConvertor(inputFile, converter);
            ProcessFile(inputFile, output, converter);
        }

        public void ProcessFile(string inputFile, string outputFile)
        {
            var converter = GetConverterForFile(inputFile) 
                ?? throw new InvalidOperationException(
                    $"No {nameof(ResourceConverter)} defined for file type {Path.GetExtension(inputFile)}");

            ProcessFile(inputFile, outputFile, converter);
        }

        private void ProcessFile(string inputFile, string outputFile, ResourceConverter converter)
        {
            if (inputFile == outputFile)
                throw new InvalidOperationException($"Failed to convert {inputFile}: input and output filenames are the same.");

            var cachedFile = _buildCache.CachedFiles.FirstOrDefault(cf => FileIsCached(cf, inputFile, outputFile, converter));
            if (cachedFile is not null)
            {
                SkippingCachedFile?.Invoke(this, cachedFile);
                return;
            }

            var directory = Path.GetDirectoryName(outputFile)!;

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            PerformConversion(inputFile, outputFile, converter);
        }

        private ResourceConverter? GetConverterForFile(string file)
        {
            return _converters.FirstOrDefault(rc => rc.Importer.FileExtension == Path.GetExtension(file));
        }

        private void PerformConversion(string inputFile, string outputFile, ResourceConverter converter)
        {
            converter.Convert(inputFile, outputFile);
        }

        private string ReplaceFileExtensionWithConvertor(string file, ResourceConverter converter)
        {
            return Path.Combine(Path.GetDirectoryName(file)!, Path.GetFileNameWithoutExtension(file) + converter.Writer.FileExtension);
        }
    }
}
