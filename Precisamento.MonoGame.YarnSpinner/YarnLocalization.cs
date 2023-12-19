using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yarn.Compiler;

namespace Precisamento.MonoGame.YarnSpinner
{
    public class YarnLocalization
    {
        /// <summary>
        /// Determines the behavior to use when adding a <see cref="YarnLocale"/> that already has a matching locale key loaded.
        /// </summary>
        public enum MergeBehavior
        {
            /// <summary>
            /// Updates the existing locale, only adding new lines.
            /// </summary>
            AddNew,

            /// <summary>
            /// Updates the existing locale, overwriting existing lines and adding new ones.
            /// </summary>
            AddReplace,

            /// <summary>
            /// Completely replaces the old locale with the new one.
            /// </summary>
            Overwrite,

            /// <summary>
            /// Throw an exception for trying to add the same locale twice.
            /// </summary>
            Throw
        }

        private string[] _emptyMetadata = Array.Empty<string>();

        public YarnLocale BaseLocale { get; set; }
        public Dictionary<string, YarnLocale> Locales { get; } = new Dictionary<string, YarnLocale>();
        public Dictionary<string, string[]> Metadata { get; } = new Dictionary<string, string[]>();

        public string GetString(string? localeName, string id)
        {
            if (TryGetString(localeName, id, out var value))
                return value;

            throw new InvalidOperationException($"Failed to find a string with ID {id}");
        }

        public bool TryGetString(string? localeName, string id, out string value)
        {
            var locale = GetLocale(localeName);

            return locale.StringTable.TryGetValue(id, out value) 
                || BaseLocale.StringTable.TryGetValue(id, out value);
        }

        public YarnLocale GetLocale(string? localeName)
        {
            if (localeName is null)
                return BaseLocale;

            if (Locales.TryGetValue(localeName, out var locale))
                return locale;

            return BaseLocale;
        }

        public string[] GetMetadata(string id)
        {
            if (Metadata.TryGetValue(id, out var value))
                return value;

            return _emptyMetadata;
        }

        public void AddLocale(YarnLocale locale, MergeBehavior mergeBehavior = MergeBehavior.AddNew)
        {
            if (Locales.TryGetValue(locale.Locale, out var other))
            {
                switch (mergeBehavior)
                {
                    case MergeBehavior.Overwrite:
                        Locales[locale.Locale] = locale;
                        break;
                    case MergeBehavior.Throw:
                        throw new ArgumentException($"Locale {locale.Locale} already exists", nameof(locale));
                    case MergeBehavior.AddNew:
                        foreach(var kvp in locale.StringTable)
                            other.StringTable.TryAdd(kvp.Key, kvp.Value);
                        break;
                    case MergeBehavior.AddReplace:
                        foreach(var kvp in locale.StringTable)
                            other.StringTable[kvp.Key] = kvp.Value;
                        break;
                }
            }
            else
            {
                Locales[locale.Locale] = locale;
            }
        }

        public YarnLocale LoadLocale(string localeName, string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return LoadLocale(localeName, stream);
        }

        public YarnLocale LoadLocale(string localeName, Stream stream)
        {
            var locale = YarnLocale.FromCsv(localeName, stream);
            Locales.Add(localeName, locale);
            return locale;
        }

        public void LoadMetadata(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                LoadMetadata(stream);
        }

        public void LoadMetadata(Stream stream)
        {
            using var reader = new TextFieldParser(stream);
            reader.SetDelimiters(",");
            reader.ReadLine();
            while (!reader.EndOfData)
            {
                var fields = reader.ReadFields();
                Metadata.Add(fields[0], fields[3..]);
            }
        }

        public static YarnLocalization FromYarnCompilation(CompilationResult result)
        {
            var locale = new YarnLocale("base", result.StringTable.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.text));
            var localization = new YarnLocalization();
            localization.BaseLocale = locale;
            localization.Locales[locale.Locale] = locale;
            foreach (var pair in result.StringTable)
                localization.Metadata.Add(pair.Key, pair.Value.metadata);

            return localization;
        }

        public static YarnLocalization FromCsv(string path)
        {
            var fname = Path.GetFileNameWithoutExtension(path);
            var ext = Path.GetExtension(path);
            var metadata = $"{fname}-metadata{ext}";
            return FromCsv(path, File.Exists(metadata) ? metadata : null);
        }

        public static YarnLocalization FromCsv(string textPath, string? metadataPath)
        {
            using var text = new FileStream(textPath, FileMode.Open, FileAccess.Read);
            if(metadataPath != null)
            {
                using var metadata = new FileStream(metadataPath, FileMode.Open, FileAccess.Read);
                return FromCsv(text, metadata);
            }

            return FromCsv(text, null);
        }

        public static YarnLocalization FromCsv(Stream textStream, Stream? metadataStream)
        {
            var l10n = new YarnLocalization();
            l10n.BaseLocale = l10n.LoadLocale("base", textStream);

            if(metadataStream is not null)
                l10n.LoadMetadata(metadataStream);

            return l10n;
        }
    }
}
