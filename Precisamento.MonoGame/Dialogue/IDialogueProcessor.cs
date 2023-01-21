using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue
{
    public interface IDialogueProcessor
    {
        bool CustomDraw { get; }
        MarkupAttribute Attribute { get; }
        Action? Release { get; set; }
        void Push(DialogueState state);
        void Pop(DialogueState state);
        void Init(Game game, MarkupAttribute attribute);
        void Reset();
    }

    public interface IDialogueCustomDraw : IDialogueProcessor
    {
        void Draw(SpriteBatchState spriteBatchState, DialogueState dialogueState);
        Vector2 Measure(char letter, DialogueState state);
    }
}
