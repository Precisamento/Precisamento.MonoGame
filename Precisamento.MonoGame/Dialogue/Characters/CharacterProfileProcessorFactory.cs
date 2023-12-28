using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Dialogue.AttributeProcessors;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public class CharacterProfileProcessorFactory : ICharacterProcessorFactory
    {
        private Game _game;
        private bool _trimName;
        private Dictionary<string, CharacterProfile> _characters;

        public CharacterProfileProcessorFactory(Game game, Dictionary<string, CharacterProfile> characters)
        {
            _game = game;
            _characters = characters;
            //PopulateTestData();
        }

        public CharacterProfile GetCharacter(string name)
        {
            return _characters[name];
        }

        public IEnumerable<IDialogueProcessor> CreateProcessorsForCharacter(LocalizedLine line, out MarkupParseResult markup)
        {
            markup = line.Text;
            var name = line.Character;
            if (name is null)
                return Array.Empty<IDialogueProcessor>();

            if (_trimName)
            {
                markup = line.TextWithoutCharacterName;
            }

            if(!_characters.TryGetValue(name, out var profile))
                return Array.Empty<IDialogueProcessor>();

            var processors = new List<IDialogueProcessor>();

            if (!_trimName)
            {
                if (profile.NameColor.HasValue)
                {
                    var color = new DialogueColorProcessor(profile.NameColor.Value);
                    color.Init(_game, 0, name.Length);
                    processors.Add(color);
                }
            }

            var textStart = _trimName ? 0 : name.Length;
            var textLength = markup.Text.Length - textStart;

            if (profile.TextColor.HasValue)
            {
                var color = new DialogueColorProcessor(profile.TextColor.Value);
                color.Init(_game, textStart, textLength);
                processors.Add(color);
            }

            if (GetCharacterParams(line, profile, out var characterParams))
            {
                var show = new ShowCharacterProcessor(characterParams!);
                show.Init(_game, textStart, textLength);
                processors.Add(show);
            }

            return processors;
        }

        private bool GetCharacterParams(LocalizedLine line, CharacterProfile profile, out CharacterParams? characterParams)
        {
            characterParams = default;

            if (line.Metadata.Contains("display:false"))
            {
                return false;
            }

            var sprite = profile.DefaultCharacterSprite;
            var background = profile.DefaultBackground;
            CharacterLocation? location = null;

            if (sprite is null && background is null)
                return false;

            if (sprite != null
                && profile.CharacterSprite != null
                && !profile.CharacterSprite.Animations.ContainsKey(sprite))
            {
                throw new ArgumentException($"Character {profile.Name} has no face sprite {sprite}");
            }

            if (background != null
                && profile.BackgroundSprite != null
                && !profile.BackgroundSprite.Animations.ContainsKey(background))
            {
                throw new ArgumentException($"Character {profile.Name} has no background sprite {sprite}");
            }

            var locationMeta = line.Metadata.FirstOrDefault(m => m.StartsWith("location:"));
            if (locationMeta != null)
            {
                var parts = locationMeta.Split(':');
                location = ShowCommand.ParseLocation(parts[1]);
            }

            characterParams = new CharacterParams()
            {
                Profile = profile,
                Sprite = sprite,
                Background = background,
                Location = location
            };

            return true;
        }

        private void PopulateTestData()
        {
            _characters = new Dictionary<string, CharacterProfile>()
            {
                {
                    "Mystborn", new CharacterProfile
                    {
                        NameColor = Color.Purple,
                        TextColor = Color.Blue,
                        BackgroundSprite = new Graphics.Sprite(),
                        DefaultBackground = ""
                    }
                },
                {
                    "Kelsier", new CharacterProfile
                    {
                        NameColor = Color.Gold
                    }
                },
                {
                    "Vin", new CharacterProfile
                    {
                        NameColor = Color.Red
                    }
                }
            };
        }
    }
}
