using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Dialogue.Characters;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueState
    {
        public Stack<Color> Colors { get; } = new Stack<Color>();
        public Stack<IFont> Fonts { get; } = new Stack<IFont>();
        public float TimePerLetter { get; set; }
        public Rectangle DrawArea { get; set; }
        public Vector2 CurrentPosition { get; set; }
        public bool WaitForInput { get; set; }
        public LineTransitionBehavior LineTransition { get; set; }
        public DialogueJustification HorizontalJustification { get; set; } = DialogueJustification.Left;
        public DialogueJustification VerticalJustification { get; set; } = DialogueJustification.Top;
        public DialogueJustification InnerJustification { get; set; } = DialogueJustification.Center;

        public DialogueState(Color defaultColor, IFont defaultFont)
        {
            Colors.Push(defaultColor);
            Fonts.Push(defaultFont);
        }
    }
}
