using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Dialogue.AttributeProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue
{
    public abstract class DialogueProcessor : IDialogueProcessor
    {
        public virtual bool CustomDraw => false;

        public int Position { get; private set; }
        public int Length { get; private set; }

        public Action? Release { get; set; }

        public DialogueProcessor()
        {
        }

        public virtual void Init(Game game, MarkupAttribute attribute)
        {
            Position = attribute.Position;
            Length = attribute.Length;
        }

        public virtual void Init(Game game, int position, int length)
        {
            Position = position;
            Length = length;
        }

        public virtual void Pop(DialogueState state) {}
        public virtual void Push(DialogueState state) {}

        public virtual void Reset()
        {
            Release = null;
        }
    }
}
