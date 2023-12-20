using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Dialogue.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue.AttributeProcessors
{
    public class DialogueCharacterProcessor : DialogueProcessor
    {
        private string _name;

        public override void Init(Game game, MarkupAttribute attribute)
        {
            base.Init(game, attribute);
            _name = attribute.Properties["name"].StringValue;
        }

        public override void Push(DialogueState state)
        {
            state.Colors.Push(_name switch
            {
                "Mystborn" => Color.Purple,
                "Kelsier" => Color.Gold,
                "Vin" => Color.Red,
                _ => Color.White
            });
        }

        public override void Pop(DialogueState state)
        {
            state.Colors.Pop();
            if (_name == "Mystborn")
            {
                state.Colors.Push(Color.Blue);
            }
        }
    }
}
