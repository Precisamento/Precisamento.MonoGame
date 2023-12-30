using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue.AttributeProcessors
{
    public class DialogueImageProcessor : DialogueProcessor, IDialogueCustomDraw
    {

        public override void Init(Game game, MarkupAttribute attribute)
        {
            base.Init(game, attribute);
        }

        public void Draw(SpriteBatchState spriteBatchState, DialogueState dialogueState) => throw new NotImplementedException();
        public Vector2 Measure(char letter, DialogueState state) => throw new NotImplementedException();
        public override void Pop(DialogueState state) => throw new NotImplementedException();
        public override void Push(DialogueState state) => throw new NotImplementedException();
    }
}
