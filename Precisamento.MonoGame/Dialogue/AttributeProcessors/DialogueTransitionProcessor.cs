using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue
{
    public enum LineTransitionBehavior
    {
        NewLine,
        Clear,
        Scroll
    }

    public class DialogueTransitionProcessor : DialogueProcessor
    {
        private bool _hasWaitForInput = false;
        private bool _hasLineEndBehavior = false;
        private bool _waitForInput;
        private LineTransitionBehavior _lineTransitionBehavior;
        private bool _set = false;

        public override void Init(Game game, MarkupAttribute attribute)
        {
            base.Init(game, attribute);

            if (attribute.Properties.TryGetValue("wait", out var wait))
            {
                _waitForInput = wait.BoolValue;
                _hasWaitForInput = true;
            }

            if(attribute.Properties.TryGetValue(attribute.Name, out var scroll))
            {
                switch(scroll.StringValue)
                {
                    case "newline":
                        _lineTransitionBehavior = LineTransitionBehavior.NewLine;
                        break;
                    case "clear":
                        _lineTransitionBehavior = LineTransitionBehavior.Clear;
                        break;
                    case "scroll":
                        _lineTransitionBehavior = LineTransitionBehavior.Scroll;
                        break;
                    default:
                        throw new ArgumentException(
                            $"Invalid value for 'transition' property in [transition] tag: {scroll}",
                            nameof(attribute));
                }

                _hasLineEndBehavior = true;
            }

            if(!_hasWaitForInput && !_hasLineEndBehavior)
            {
                throw new ArgumentException(
                    $"Invalid [transition] attribute. Must contain at least one of 'wait' and 'transition'");
            }
        }

        public override void Reset()
        {
            base.Reset();
            _set = false;
        }

        public override void Pop(DialogueState state)
        {
        }

        public override void Push(DialogueState state)
        {
            if (_set)
                return;

            _set = true;
            if(_hasLineEndBehavior)
                state.LineTransition = _lineTransitionBehavior;

            if( _hasWaitForInput)
                state.WaitForInput = _waitForInput;
        }
    }
}
