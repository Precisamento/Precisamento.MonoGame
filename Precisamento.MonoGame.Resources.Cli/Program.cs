using System.Text.Json;
using Precisamento.MonoGame.Resources.Sprites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.CommandLine;
using Yarn;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Precisamento.MonoGame.Resources.Cli
{
    public class Program
    {
        private static Microsoft.Extensions.Logging.ILogger _logger;
        private static int _directoryErrors = 0;
        private static int _fileErrors = 0;
        private static int _exitCode = 0;
        private static bool _stopOnError = false;

        public static int Main(string[] args)
        {
            var serviceBuilder = new ServiceCollection();
            serviceBuilder.AddLogging(config =>
            {
                config.AddSerilog(new LoggerConfiguration()
                    .WriteTo.Console(
                        outputTemplate: "[{Level:t4} {Timestamp:HH:mm:ss}] {Message:lj}{NewLine}{Exception}",
                        theme: AnsiConsoleTheme.Code)
                    .MinimumLevel.Verbose()
                    .CreateLogger());
            });

            var provider = serviceBuilder.BuildServiceProvider();
            _logger = provider.GetService<ILogger<Program>>()!;

            var rootCommand = new RootCommand("Resource Processor for Precisamento.MonoGame projects");
            var configOption = new Option<FileInfo?>(
                name: "--config",
                description: "The location of the config file used to determine which directories and files to process",
                getDefaultValue: () => 
                {
                    var result = File.Exists("resources.config") ? new FileInfo("resources.config") : null;
                    return result;
                });

            configOption.AddAlias("-c");

            var rebuildOption = new Option<bool>(
                name: "--rebuild",
                description: "Set to rebuild all files even if they haven't been changed from the last build");

            var stopOnErrorOption = new Option<bool>(
                name: "--stop",
                description: "Stop processing files when an error occurs");

            rebuildOption.AddAlias("-r");

            rootCommand.Add(configOption);
            rootCommand.Add(rebuildOption);
            rootCommand.Add(stopOnErrorOption);

            rootCommand.SetHandler(async (configFile, rebuild, stopOnError) =>
            {
                if (configFile is null)
                {
                    _logger.LogError("Must provide a valid config file containing the files to process.");
                    _exitCode = 1;
                    return;
                }

                _stopOnError = stopOnError;

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var config = await CreateResourceConfig(configFile);
                if (config is null)
                {
                    _exitCode = 1;
                    return;
                }

                var cache = rebuild ? new ResourceBuildCache() : await LoadBuildCache(configFile);

                var processor = CreateResourceProcessor(config, cache);
                if(processor is null)
                {
                    _exitCode = 1;
                    return;
                }

                var processedFiles = RunResourceConversion(processor, config);

                if(processedFiles is not null)
                    await SaveBuildCache(processedFiles, configFile);

                stopwatch.Stop();

                _logger.LogInformation("Finished processing files. Took {Time}. {DirectoryErrors} Directory Errors. {FileErrors} File Errors.", stopwatch.ElapsedMilliseconds, _directoryErrors, _fileErrors);
            },
            configOption,
            rebuildOption,
            stopOnErrorOption);

            rootCommand.Invoke(args);

            return ((_directoryErrors != 0 || _fileErrors != 0) ? 1 : 0) & _exitCode;
        }

        private static List<ResourceBuildCache.CachedFile>? RunResourceConversion(ResourceProcessor processor, ResourceProcessorConfig config)
        {
            var key = new object();
            var filesBeingProcessed = new Dictionary<string, ResourceBuildCache.CachedFile>();
            var processedFiles = new List<ResourceBuildCache.CachedFile>();

            processor.NoConverterFound += (_, e) => _logger.LogWarning("No {Type} registered for the file {File}", nameof(ResourceImporter), e);
            processor.ProcessingDirectory += (_, e) => _logger.LogTrace("Processing directory {Directory}", e.Name);
            processor.ProcessDirectoryError += (_, e) =>
            {
                _directoryErrors++;
                _logger.LogError("Failed to find directory {Directory}", e.Name);
            };

            processor.ProcessedDirectory += (_, e) => _logger.LogTrace("Finished processing directory {Directory}", e.Name);
            processor.ProcessingFile += (_, e) =>
            {
                lock (key)
                {
                    filesBeingProcessed.Add(e.Name, new ResourceBuildCache.CachedFile()
                    {
                        Input = e.Name,
                        ConverterName = e.Converter.Importer.GetType().FullName!
                    });
                    _logger.LogInformation("Processing {File} using {Importer}", e.Name, e.Converter.Importer.GetType().FullName);
                }
            };

            processor.ProcessedFile += (_, e) => 
            {
                _logger.LogInformation("Finished processing {Input} -> {Output}", e.Name, e.OutputFile);
                lock(key)
                {
                    if(filesBeingProcessed.TryGetValue(e.Name, out var fileInProgress))
                    {
                        try
                        {
                            fileInProgress.Output = e.OutputFile;
                            fileInProgress.LastEdited = File.GetLastWriteTimeUtc(fileInProgress.Input);
                            processedFiles.Add(fileInProgress);
                            filesBeingProcessed.Remove(e.Name);
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError(ex, "Error when caching file:");
                        }
                    }
                }
            };

            processor.ProcessFileError += (_, e) =>
            {
                _fileErrors++;
                _logger.LogError(e.Exception, "Failed to process {File}", e.Name);
            };

            processor.SkippingCachedFile += (_, e) =>
            {
                _logger.LogInformation("Skipping cached file {File}", e.Input);
                lock (key)
                {
                    processedFiles.Add(e);
                }
            };

            foreach (var directory in config.Directories)
            {
                if (_stopOnError && _directoryErrors != 0)
                    return null;

                processor.ProcessDirectory(directory.InputFile, directory.OutputFile ?? directory.InputFile);
            }

            foreach (var file in config.Files)
            {
                if (_stopOnError && _fileErrors != 0)
                    return null;

                if (file.OutputFile is null)
                    processor.ProcessFile(file.InputFile);
                else
                    processor.ProcessFile(file.InputFile, file.OutputFile!);
            }

            return processedFiles;
        }

        private static async Task SaveBuildCache(List<ResourceBuildCache.CachedFile> processedFiles, FileInfo configFile)
        {
            try
            {
                var cache = new ResourceBuildCache() { CachedFiles = processedFiles };
                var cacheFile = new FileInfo(configFile.FullName + ".cache");
                var stream = cacheFile.Create();
                await JsonSerializer.SerializeAsync(stream, cache);
                await stream.FlushAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to update build cache file:");
            }
        }

        private static async Task<ResourceProcessorConfig?> CreateResourceConfig(FileInfo configFile)
        {
            try
            {
                using var stream = configFile.OpenText();
                var text = await stream.ReadToEndAsync();
                var options = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    Converters =
                    {
                        ResourceProcessorConfig.GetJsonConvertor()
                    }
                };

                var config = JsonSerializer.Deserialize<ResourceProcessorConfig>(text, options)!;

                var workingDir = configFile.DirectoryName!;

                foreach(var directory in config.Directories)
                {
                    NormalizeResourceFilePath(directory, workingDir);
                }

                foreach(var file in config.Files)
                {
                    NormalizeResourceFilePath(file, workingDir);
                }

                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize config file {File}", configFile);
                return null;
            }
        }

        private static void NormalizeResourceFilePath(ResourceFile resourceFile, string workingDirectory)
        {
            if (!Path.IsPathFullyQualified(resourceFile.InputFile))
            {
                resourceFile.InputFile = Path.Combine(workingDirectory, resourceFile.InputFile);
            }

            if (resourceFile.OutputFile != null && !Path.IsPathFullyQualified(resourceFile.OutputFile))
            {
                resourceFile.OutputFile = Path.Combine(workingDirectory, resourceFile.OutputFile);
            }
        }

        private static async Task<ResourceBuildCache> LoadBuildCache(FileInfo configFile)
        {
            try
            {
                var cacheFile = configFile.Directory?.EnumerateFiles().FirstOrDefault(f => f.Name == configFile.Name + ".cache");
                if (cacheFile is null)
                    return new ResourceBuildCache();

                using var stream = cacheFile.OpenRead();
                var cache = await JsonSerializer.DeserializeAsync<ResourceBuildCache>(stream);

                return cache ?? new ResourceBuildCache();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize cache file:");
                return new ResourceBuildCache();
            }
        }

        private static ResourceProcessor? CreateResourceProcessor(ResourceProcessorConfig config, ResourceBuildCache cache)
        {
            var assemblies = new List<Assembly>()
            {
                typeof(ResourceImporter).Assembly
            };

            foreach (var dll in config.Assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load assembly {Assembly}", dll);
                    return null;
                }
            }

            var resourceConverters = new List<ResourceConverter>();

            try
            {
                foreach (var assembly in assemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        foreach (var attribute in type.GetCustomAttributes(false))
                        {
                            if (attribute is ResourceImporterAttribute importerAttribute)
                            {
                                var importer = (ResourceImporter)Activator.CreateInstance(type)!;
                                var writer = (ResourceTypeWriter)Activator.CreateInstance(importerAttribute.Writer)!;
                                var converter = new ResourceConverter(importer, writer);
                                resourceConverters.Add(converter);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a {Convertor}", nameof(ResourceConverter));
                return null;
            }

            return new ResourceProcessor(resourceConverters, cache);
        }
    }
}
