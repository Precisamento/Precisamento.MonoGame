using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public class CharacterDisplay
    {
        public CharacterParams Character { get; set; }
        public SpriteAnimationPlayer FaceAnimationPlayer { get; } = new SpriteAnimationPlayer();
        public SpriteAnimationPlayer BackgroundAnimationPlayer { get; } = new SpriteAnimationPlayer();
        public bool Speaking { get; set; }
        public Rectangle BackgroundBounds { get; set; }
        public Rectangle FaceBounds { get; set; }

        public CharacterDisplay()
        {
        }

        public CharacterDisplay(CharacterParams character, bool speaking)
        {
            Character = character;
            Speaking = speaking;
        }
    }
}
