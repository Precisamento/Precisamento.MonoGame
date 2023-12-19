using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Yarn;
using Yarn.Compiler;

namespace Precisamento.MonoGame.Resources.Dialogue
{
    [ResourceImporter(typeof(YarnWriter))]
    public class YarnProjectImporter : ResourceImporter<YarnDescription>
    {
        public override string FileExtension => ".yarnproject";

        protected override YarnDescription ImportImpl(string fileName)
        {
            var project = Project.LoadFromFile(fileName);
            var basePath = Path.GetDirectoryName(fileName)!;
            var job = CompilationJob.CreateFromFiles(project.SourceFiles);

            // Todo: Determine if this actually affects the result of the
            //       compilation at all.
            if (!string.IsNullOrWhiteSpace(project.DefinitionsPath))
            {
                var definitionsFileName = Path.Combine(
                    basePath,
                    project.DefinitionsPath);

                var definitions = JsonSerializer.Deserialize<YarnDefinitions>(definitionsFileName)!;

                var declarations = definitions.GetDeclarations();
                job.VariableDeclarations = (job.VariableDeclarations ?? Array.Empty<Declaration>()).Concat(declarations);
            }

            var result = Compiler.Compile(job);

            var localization = new YarnLocalization();
            var baseLocale = new YarnLocale(project.BaseLanguage);

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
