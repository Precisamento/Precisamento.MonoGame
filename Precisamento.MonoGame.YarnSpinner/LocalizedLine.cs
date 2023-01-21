using System;
using System.Collections.Generic;
using System.Text;
using Yarn.Markup;

namespace Precisamento.MonoGame.YarnSpinner
{
    public class LocalizedLine
    {
        public string TextId { get; set; }

        public string[] Substitutions { get; set; }

        public string RawText { get; set; }

        public string[] Metadata { get; set; }

        public MarkupParseResult Text { get; set; }

        public string? Character
        {
            get
            {
                if (Text.TryGetAttributeWithName("character", out var character) && character.Properties.TryGetValue("name", out var name))
                    return name.StringValue;
                return null;
            }
        }

        public MarkupParseResult TextWithoutCharacterName
        {
            get
            {
                if (Text.TryGetAttributeWithName("character", out var character))
                {
                    return Text.DeleteRange(character);
                }
                else
                {
                    return Text;
                }
            }
        }

        public LocalizedLine Clone()
        {
            return new LocalizedLine()
            {
                TextId = TextId,
                Substitutions = Substitutions,
                RawText = RawText,
                Metadata = Metadata,
                Text = Text
            };
        }
    }
}
