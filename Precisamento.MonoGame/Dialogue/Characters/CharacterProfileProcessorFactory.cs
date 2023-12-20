using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Dialogue.AttributeProcessors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public class CharacterProfileProcessorFactory : ICharacterProcessorFactory
    {
        private Dictionary<string, CharacterProfile> _characters;
        private bool _trimName;
        private Game _game;

        public CharacterProfileProcessorFactory(Game game, Dictionary<string, CharacterProfile> characters)
        {
            _game = game;
            _characters = characters;
        }

        public IEnumerable<IDialogueProcessor> CreateProcessorsForCharacter(ref MarkupParseResult markup)
        {
            if (!markup.TryGetAttributeWithName("character", out var attribute))
                return Array.Empty<IDialogueProcessor>();

            if (_trimName)
            {
                markup = markup.DeleteRange(attribute);
            }

            var name = attribute.Properties["name"].StringValue;

            if(!_characters.TryGetValue(name, out var profile))
                return Array.Empty<IDialogueProcessor>();

            var processors = new List<IDialogueProcessor>();

            if (!_trimName)
            {
                if (profile.NameColor.HasValue)
                {
                    var color = new DialogueColorProcessor(profile.NameColor.Value);
                    color.Init(_game, attribute.Position, attribute.Length);
                    processors.Add(color);
                }
            }

            var textStart = _trimName ? 0 : attribute.Length;
            var textLength = markup.Text.Length - textStart;

            if (profile.TextColor.HasValue)
            {
                var color = new DialogueColorProcessor(profile.TextColor.Value);
                color.Init(_game, textStart, textLength);
                processors.Add(color);
            }

            return processors;
        }
    }
}
