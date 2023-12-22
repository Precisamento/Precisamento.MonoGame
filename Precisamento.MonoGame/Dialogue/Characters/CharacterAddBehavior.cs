using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public enum CharacterAddBehavior
    {
        /// <summary>
        /// Replaces the old character position with the new one right away.
        /// </summary>
        Overwrite,

        /// <summary>
        /// Moves the character portrait using the default move effect.
        /// </summary>
        Move,

        /// <summary>
        /// Ignore the new character position.
        /// </summary>
        Ignore,

        /// <summary>
        /// Ignores the new character position unless the location has been explicitly specified.
        /// </summary>
        IgnoreUnlessExplicitlySet,

        /// <summary>
        /// Allows multiple portraits of the same character to exist.
        /// </summary>
        Multiply
    }
}
