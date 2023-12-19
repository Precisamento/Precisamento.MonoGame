using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yarn.Compiler;

namespace Precisamento.MonoGame.Resources.Dialogue
{
    [ResourceImporter(typeof(YarnWriter))]
    public class YarnImporter : ResourceImporter<YarnDescription>
    {
        public override string FileExtension => ".yarn";

        protected override YarnDescription ImportImpl(string fileName)
        {
            var job = CompilationJob.CreateFromFiles(fileName);
            var result = Compiler.Compile(job);

            var localization = new YarnLocalization();
            var baseLocale = new YarnLocale(CultureInfo.CurrentCulture.Name);

            foreach(var pair in result.StringTable)
            {
                baseLocale.StringTable.Add(pair.Key, pair.Value.text);
                localization.Metadata.Add(pair.Key, pair.Value.metadata);
            }

            localization.BaseLocale = baseLocale;
            localization.Locales[baseLocale.Locale] = baseLocale;

            return new YarnDescription(result, localization);
        }
    }
}
