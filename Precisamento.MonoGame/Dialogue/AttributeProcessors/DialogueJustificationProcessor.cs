using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueJustificationProcessor : DialogueProcessor
    {
        private bool _hasVertical;
        private bool _hasHorizontal;
        private bool _hasInner;
        private DialogueJustification _vertical;
        private DialogueJustification _horizontal;
        private DialogueJustification _inner;

        public DialogueJustificationProcessor()
        { }

        public DialogueJustificationProcessor(DialogueJustification? vertical, DialogueJustification? horizontal, DialogueJustification? inner)
        {
            if (vertical is DialogueJustification v) 
            {
                _hasVertical = true;
                _vertical = v;
            }

            if (horizontal is DialogueJustification h)
            {
                _hasHorizontal = true;
                _horizontal = h;
            }

            if (inner is DialogueJustification i)
            {
                _hasInner = true;
                _inner = i;
            }

            if (!_hasVertical && !_hasHorizontal && !_hasInner)
            {
                throw new ArgumentException(
                    $"Invalid [justification] attribute. Must contain at least one of 'vertical', 'horizontal', or 'inner'");
            }
        }

        public override void Init(Game game, MarkupAttribute attribute)
        {
            base.Init(game, attribute);

            foreach(var kvp in attribute.Properties)
            {
                switch(kvp.Key)
                {
                    case "vert":
                    case "vertical":
                        _hasVertical = true;
                        _vertical = ParseJustification(kvp.Value);
                        break;
                    case "hor":
                    case "horizontal":
                        _hasHorizontal = true;
                        _horizontal = ParseJustification(kvp.Value);
                        break;
                    case "inner":
                        _hasInner = true;
                        _inner = ParseJustification(kvp.Value);
                        break;
                }
            }

            if(!_hasVertical && !_hasHorizontal && !_hasInner)
            {
                throw new ArgumentException(
                    $"Invalid [justification] attribute. Must contain at least one of 'vertical', 'horizontal', or 'inner'");
            }
        }

        public override void Pop(DialogueState state)
        {
        }

        public override void Push(DialogueState state)
        {
            if(_hasHorizontal)
                state.HorizontalJustification = _horizontal;

            if(_hasVertical)
                state.VerticalJustification = _vertical;

            if(_hasInner)
                state.InnerJustification = _inner;
        }

        private DialogueJustification ParseJustification(MarkupValue value)
        {
            switch(value.StringValue.ToLower())
            {
                case "top": return DialogueJustification.Top;
                case "left": return DialogueJustification.Left;
                case "bottom": return DialogueJustification.Bottom;
                case "right": return DialogueJustification.Right;
                case "center":
                case "middle":
                    return DialogueJustification.Center;
                default:
                    throw new ArgumentException(
                        $"Invalid justification value in [justification] tag: {value}");
            }
        }
    }
}
