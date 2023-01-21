using System.Text.Json;
using Precisamento.MonoGame.Resources.Sprites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Precisamento.MonoGame.Resources.Cli
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine($"{Path.GetFileName(Environment.GetCommandLineArgs()[0])} " +
                    $"expected at least two arguments: input_dir output_dir [config_file]");
                return 1;
            }

            var inputDirectory = args[0];
            var outputDirectory = args[1];
            var configFile = args.Length > 2 ? args[2] : null;

            if(!File.Exists(configFile))
            {
                Console.WriteLine($"[Failure] Could not find config file {configFile}");
                return 1;
            }

            Config config = new();

            try
            {
                var text = File.ReadAllText(configFile);
                config = JsonSerializer.Deserialize<Config>(text);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Failure] Failed to deserialize config file {configFile}");
                Console.WriteLine("Error:");
                Console.WriteLine(ex);
                return 1;
            }

            var assemblies = new List<Assembly>()
            {
                typeof(ResourceImporter).Assembly
            };

            foreach(var dll in config.Assemblies)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    assemblies.Add(assembly);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"[Failure] Failed to load assembly {dll} defined in config file {configFile}");
                    Console.WriteLine("Error:");
                    Console.WriteLine(ex);
                    return 1;
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
                                var importer = (ResourceImporter)Activator.CreateInstance(type);
                                var writer = (ResourceTypeWriter)Activator.CreateInstance(importerAttribute.Writer);
                                var converter = new ResourceConverter(importer, writer);
                                resourceConverters.Add(converter);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Failure] Failed to create a {nameof(ResourceConverter)}");
                Console.WriteLine("Error:");
                Console.WriteLine(ex);
            }

            var processor = new ResourceProcessor(resourceConverters);
            var success = true;

            processor.ProcessDirectoryError += (_, e) => Console.WriteLine($"Failed to find directory {e.Name}");
            processor.ProcessingFile += (_, e) => Console.Write($"Processing {e.Name} using {e.Converter.GetType().FullName}");
            processor.ProcessedFile += (_, e) => Console.WriteLine(" [Success]");
            processor.ProcessFileError += (_, e) =>
            {
                Console.WriteLine(" [Failure]");
                Console.WriteLine(e.Exception);
                Console.WriteLine();

                success = false;
            };

            processor.ProcessDirectory(inputDirectory, outputDirectory);

            return success ? 1 : 0;
        }
    }
}
