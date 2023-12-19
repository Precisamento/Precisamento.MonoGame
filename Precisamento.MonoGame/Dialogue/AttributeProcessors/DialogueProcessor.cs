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

        public MarkupAttribute Attribute { get; private set; }

        public Action? Release { get; set; }

        public DialogueProcessor()
        {
        }

        public virtual void Init(Game game, MarkupAttribute attribute)
        {
            Attribute = attribute;
        }

        public abstract void Pop(DialogueState state);
        public abstract void Push(DialogueState state);

        public virtual void Reset()
        {
            Release = null;
        }
    }
}
