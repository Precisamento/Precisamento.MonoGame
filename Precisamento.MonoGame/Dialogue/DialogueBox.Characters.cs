using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue
{
    public partial class DialogueBox
    {
        public void UpdateCharacters(float delta)
        {
            var oldSpeaker = _characterState.CurrentSpeaker;

            foreach(var character in _characterState.Adding)
            {
                _characterState.Characters.Add(character);
                _characterState.CurrentSpeaker = character.Profile.Name;
            }

            _characterState.Adding.Clear();

            foreach(var character in _characterState.Removing)
            {
                var removed = _characterState.Characters.Remove(character);
                if (removed && _characterState.CurrentSpeaker == character.Profile.Name)
                {
                    _characterState.CurrentSpeaker = _characterState.Characters.Count == 0
                        ? null
                        : _characterState.Characters[^1].Profile.Name;
                }
            }

            if (oldSpeaker != _characterState.CurrentSpeaker)
            {
                var profile = _characterState.Characters.LastOrDefault(c => c.Profile.Name == _characterState.CurrentSpeaker);
                if (profile?.Background != null)
                {
                    var background = profile.Profile.BackgroundSprite!.Animations[profile.Background];
                    _characterBackgroundPlayer.Animation = background;
                }
                else
                {
                    _characterBackgroundPlayer.Animation = null;
                }

                if (profile?.Sprite != null)
                {
                    var face = profile.Profile.CharacterSprite!.Animations[profile.Sprite];
                    _characterFacePlayer.Animation = face;
                }
                else
                {
                    _characterFacePlayer.Animation = null;
                }
            }
            else
            {
                _characterBackgroundPlayer.Update(delta);
                _characterFacePlayer.Update(delta);
            }
        }

        public void DrawCharacters(SpriteBatchState state)
        {
            _characterBackgroundPlayer.Draw(state,
                new Rectangle(20, 200, 64, 64));

            _characterFacePlayer.Draw(state, new Vector2(28, 208));
        }
    }
}
