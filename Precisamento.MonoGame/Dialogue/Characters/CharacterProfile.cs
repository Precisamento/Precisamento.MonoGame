using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public class CharacterProfile
    {
        public string Name { get; set; }
        public Sprite? CharacterSprite { get; set; }
        public Sprite? BackgroundSprite { get; set; }
        public string DefaultCharacterSprite { get; set; }
        public string DefaultBackground { get; set; }
        public Color? NameColor { get; set; }
        public Color? TextColor { get; set; }
    }
}
