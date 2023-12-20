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

        public void DrawCharacters(SpriteBatchState state)
        {
            var speaker = _state.Characters.FirstOrDefault(c => c.Profile.Name == _state.CurrentSpeaker);
            if (speaker is null)
                return;

            if (speaker.Background != null)
            {
                var backgroundAnimation = speaker.Profile.BackgroundSprite!.Animations[speaker.Background];

                _characterBackgroundPlayer.Animation = backgroundAnimation;
            }
        }
    }
}
