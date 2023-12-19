using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Resources
{
    public class ResourceProcessorConfig
    {
        public List<string> Assemblies { get; } = new List<string>();
        public List<ResourceFile> Files { get; } = new List<ResourceFile>();
        public List<ResourceFile> Directories { get; } = new List<ResourceFile>();

        public static JsonConverter<ResourceProcessorConfig> GetJsonConvertor() => new InternalJsonConvertor();

        private class InternalJsonConvertor : JsonConverter<ResourceProcessorConfig>
        {
            public override ResourceProcessorConfig Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var result = new ResourceProcessorConfig();

                switch(reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        ReadConfigObject(ref reader, result);
                        break;
                    case JsonTokenType.StartArray:
                        ReadResourceList(ref reader, result.Directories);
                        break;
                    default:
                        throw new JsonException();
                }

                return result;
            }

            public override void Write(Utf8JsonWriter writer, ResourceProcessorConfig value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName("Assemblies") ?? "Assemblies");
                writer.WriteStartArray();
                foreach(var asm in value.Assemblies)
                {
                    writer.WriteStringValue(asm);
                }
                writer.WriteEndArray();

                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName("Resources") ?? "Resources");
                writer.WriteStartArray();
                foreach (var resource in value.Files)
                {
                    if(resource.OutputFile is null)
                    {
                        writer.WriteStringValue(resource.InputFile);
                    }
                    else
                    {
                        writer.WriteStartObject();

                        writer.WriteString(options.PropertyNamingPolicy?.ConvertName("Input") ?? "Input", resource.InputFile);
                        writer.WriteString(options.PropertyNamingPolicy?.ConvertName("Output") ?? "Output", resource.OutputFile);

                        writer.WriteEndObject();
                    }
                }
                writer.WriteEndArray();
            }

            private void ReadConfigObject(ref Utf8JsonReader reader, ResourceProcessorConfig config)
            {
                bool hasAssemblies = false;
                bool hasFiles = false;
                bool hasDirectories = false;

                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();
                reader.Read();

                while (reader.TokenType != JsonTokenType.EndObject)
                {
                    switch(reader.GetString())
                    {
                        case "Assemblies":
                        case "assemblies":
                            if(hasAssemblies)
                                throw new JsonException();

                            reader.Read();

                            if (reader.TokenType != JsonTokenType.StartArray)
                                throw new JsonException();

                            hasAssemblies = true;

                            while(reader.TokenType != JsonTokenType.EndArray)
                            {
                                config.Assemblies.Add(reader.GetString()!);
                                reader.Read();
                            }

                            break;
                        case "Files":
                        case "files":
                            if(hasFiles)
                                throw new JsonException();

                            reader.Read();

                            ReadResourceList(ref reader, config.Files);
                            break;
                        case "Directories":
                        case "directories":
                            if (hasDirectories)
                                throw new JsonException();

                            reader.Read();

                            ReadResourceList(ref reader, config.Directories);
                            break;
                    }

                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;
                }
            }

            private void ReadResourceList(ref Utf8JsonReader reader, List<ResourceFile> resources)
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                    throw new JsonException();

                reader.Read();

                while (reader.TokenType != JsonTokenType.EndArray)
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.StartObject:
                            resources.Add(ReadFullResourceFile(ref reader));
                            break;
                        case JsonTokenType.String:
                            resources.Add(new ResourceFile(reader.GetString()!));
                            reader.Read();
                            break;
                        default:
                            throw new JsonException();
                    }
                }

                reader.Read();
            }

            private ResourceFile ReadFullResourceFile(ref Utf8JsonReader reader)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                reader.Read();
                string? input = null;
                string? output = null;
                while (reader.TokenType != JsonTokenType.EndObject)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Input":
                        case "input":
                            if(input != null)
                                throw new JsonException();
                            input = reader.GetString();
                            reader.Read();
                            break;
                        case "Output":
                        case "output":
                            if(output != null)
                                throw new JsonException();
                            output = reader.GetString();
                            reader.Read();
                            break;
                        default:
                            throw new JsonException();
                    }
                }

                if (input is null)
                    throw new JsonException();

                reader.Read();

                return new ResourceFile(input, output);
            }
        }
    }
}
