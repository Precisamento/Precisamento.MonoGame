using Precisamento.MonoGame.Dialogue.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public class CharacterLocation : IEquatable<CharacterLocation>
    {
        public DialogueOptionRenderLocation RenderLocation { get; set; }
        public Point ExactLocation { get; set; }

        public CharacterLocation(DialogueOptionRenderLocation renderLocation)
        {
            RenderLocation = renderLocation;
        }

        public CharacterLocation(Point exactLocation)
        {
            ExactLocation = exactLocation;
        }

        public bool Equals(CharacterLocation? other)
        {
            if (other is null)
                return false;

            return RenderLocation == other.RenderLocation 
                && ExactLocation == other.ExactLocation;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CharacterLocation);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RenderLocation, ExactLocation);
        }
    }
}
