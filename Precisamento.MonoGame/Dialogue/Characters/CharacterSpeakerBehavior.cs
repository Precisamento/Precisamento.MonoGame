using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public enum CharacterSpeakerBehavior
    {
        /// <summary>
        /// Only the most recent character is tracked as a speaker.
        /// </summary>
        MostRecentOnly,

        /// <summary>
        /// The most recent character to speak in each unique location is considered a speaker.
        /// When the speaker in a location moves, the next most recent speaker will become active.
        /// </summary>
        MostRecentInEachLocationUpdateWithMove,

        /// <summary>
        /// The most recent character to speak in each unique location is considered a speaker.
        /// When the speaker in a location moves, no one in the location will be a speaker.
        /// </summary>
        MostRecentInEachLocation
    }
}
