using Precisamento.MonoGame.Resources;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueReader : ResourceTypeReader<DialogueData>
    {
        public override string FileExtension => ".bin";

        public override DialogueData Read(ResourceReader reader)
        {
            var localization = new YarnLocalization();
            var programSize = reader.ReadInt32();
            var buffer = reader.ReadBytes(programSize);

            var program = Yarn.Program.Parser.ParseFrom(buffer);

            if (program is null)
            {
                throw new InvalidOperationException("Failed to read yarn program");
            }

            var baseLocale = reader.ReadString();
            var localeCount = reader.ReadInt32();

            for(var i = 0; i < localeCount; i++)
            {
                var name = reader.ReadString();
                var count = reader.ReadInt32();
                var table = new Dictionary<string, string>(count);

                for (var item = 0; item < count; item++)
                {
                    var key = reader.ReadString();
                    var value = reader.ReadString();
                    table.Add(key, value);
                }

                var locale = new YarnLocale(name, table);
                localization.Locales[name] = locale;
                if (name == baseLocale)
                    localization.BaseLocale = locale;
            }

            var metadataCount = reader.ReadInt32();
            for (var i = 0; i < metadataCount; i++)
            {
                var key = reader.ReadString();
                var valueCount = reader.ReadByte();
                var values = new string[valueCount];
                for (var item = 0; item < valueCount; item++)
                {
                    values[item] = reader.ReadString();
                }

                localization.Metadata[key] = values;
            }

            return new DialogueData(program, localization);
        }
    }
}
