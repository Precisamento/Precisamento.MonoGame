using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Resources.Dialogue
{
    public class YarnWriter : ResourceTypeWriter<YarnDescription>
    {
        protected override void Write(BinaryWriter writer, YarnDescription yarn)
        {
            using var memoryStream = new MemoryStream();
            using var codedStream = new Google.Protobuf.CodedOutputStream(memoryStream, true);

            yarn.Result.Program.WriteTo(codedStream);

            codedStream.Flush();

            writer.Write((int)memoryStream.Position);

            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.CopyTo(writer.BaseStream);

            writer.Write(yarn.Locales.BaseLocale.Locale);
            writer.Write(yarn.Locales.Locales.Count);

            foreach(var locale in yarn.Locales.Locales.Values)
            {
                writer.Write(locale.Locale);
                writer.Write(locale.StringTable.Count);
                foreach(var pair in locale.StringTable)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }
            }

            var metadata = yarn.Locales.Metadata
                .Select(m => KeyValuePair.Create(m.Key, m.Value.Where(v => !v.StartsWith("line:")).ToList()))
                .Where(m => m.Value.Count > 0)
                .ToList();

            writer.Write(metadata.Count);
            foreach(var pair in metadata)
            {
                writer.Write(pair.Key);
                writer.Write((byte)pair.Value.Count);

                foreach(var value in pair.Value)
                {
                    writer.Write(value);
                }
            }
        }
    }
}
