using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Precisamento.MonoGame.Resources
{
    public class ResourceConverter
    {
        public ResourceImporter Importer { get; }
        public ResourceTypeWriter Writer { get; }

        public ResourceConverter(
            ResourceImporter importer,
            ResourceTypeWriter writer)
        {
            Importer = importer;
            Writer = writer;
        }

        public void Convert(string inputFile, string outputFile)
        {
            var result = Importer.Import(inputFile);

            using (var output = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(output))
            {
                Writer.Write(writer, result);
            }
        }
    }
}
