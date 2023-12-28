using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public class CharacterParams
    {
        public CharacterProfile Profile { get; set; }
        public string? Background { get; set; }
        public string? Sprite { get; set; }
        public bool Flipped { get; set; }
        public bool Speaking { get; set; } = true;
        public CharacterLocation? Location { get; set; }
    }
}
