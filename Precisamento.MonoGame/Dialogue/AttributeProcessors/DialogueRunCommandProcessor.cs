using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.AttributeProcessors
{
    /// <summary>
    /// Special class that can be used to run a command from an attribute.
    /// Shouldn't be used bydialogue directly, but specialized attributes can be
    /// be processed using this.
    /// </summary>
    public class DialogueRunCommandProcessor : DialogueProcessor
    {
        private DialogueRunner _runner;
        private string[] _args;
        private bool _completed = false;

        public DialogueRunCommandProcessor(DialogueRunner runner, string[] args)
        {
            _runner = runner;
            _args = args;
        }

        public override void Reset()
        {
            base.Reset();
            _completed = false;
        }

        public override void Pop(DialogueState state)
        {
        }

        public override void Push(DialogueState state)
        {
            if (_completed)
                return;

            _completed = true;

            _runner.TriggerCommand(_args);
        }
    }
}
