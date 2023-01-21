using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.YarnSpinner
{
    public struct DialogueOption
    {
        /// <summary>
        /// The ID of this dialogue option
        /// </summary>
        public int DialogueOptionId { get; set; }

        /// <summary>
        /// The ID of the dialogue option's text
        /// </summary>
        public string TextId { get; set; }

        /// <summary>
        /// The line for this dialogue option
        /// </summary>
        public LocalizedLine Line { get; set; }

        /// <summary>
        /// Indicates whether this value should be presented as available
        /// or not.
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}
