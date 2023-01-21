using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yarn;

namespace Precisamento.MonoGame.YarnSpinner
{
    public class YarnLocale
    {
        public string Locale { get; set; }
        public Dictionary<string, string> StringTable { get; }

        public YarnLocale(string localeName)
            : this(localeName, new Dictionary<string, string>())
        {
        }

        public YarnLocale(string localeName, Dictionary<string, string> stringTable)
        {
            Locale = localeName ?? throw new ArgumentNullException(nameof(localeName));
            StringTable = stringTable ?? new Dictionary<string, string>();
        }

        public static YarnLocale FromCsv(string localeName, string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return FromCsv(localeName, stream);
        }

        public static YarnLocale FromCsv(string localeName, Stream stream)
        {
            var locale = new YarnLocale(localeName);

            using(var reader = new TextFieldParser(stream))
            {
                reader.Delimiters = new[] { "," };
                reader.HasFieldsEnclosedInQuotes = true;
                reader.ReadLine();
                while(!reader.EndOfData)
                {
                    string[] fields = reader.ReadFields();
                    locale.StringTable.Add(fields[0], fields[1]);
                }
            }

            return locale;
        }
    }
}
