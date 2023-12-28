using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public class CharacterMoving
    {
        public Vector2 Position { get; set; }
        public Rectangle FinalBackgroundBounds { get; set; }
        public Rectangle FinalFaceBounds { get; set; }
        public CharacterDisplay Character { get; set; }
        public Tween Tween { get; set; }
    }
}
