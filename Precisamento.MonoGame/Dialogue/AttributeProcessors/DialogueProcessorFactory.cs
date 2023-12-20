using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Dialogue.AttributeProcessors;
using Precisamento.MonoGame.YarnSpinner.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueProcessorFactory
    {
        private class StringEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string? x, string? y)
            {
                return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return string.GetHashCode(obj, StringComparison.OrdinalIgnoreCase);
            }
        }

        private Dictionary<string, Pool<IDialogueProcessor>> _processorPools 
            = new(new StringEqualityComparer());

        private Game _game;
        private DialogueNopProcessor _nop = new DialogueNopProcessor();

        public DialogueProcessorFactory(Game game)
        {
            _game = game;

            RegisterProcessorType<DialogueColorProcessor>("color", "colour");
            RegisterProcessorType<DialogueFontProcessor>("font");
            RegisterProcessorType<DialogueTransitionProcessor>("trans", "transition");
            RegisterProcessorType<DialogueJustificationProcessor>("just", "justification");
            RegisterProcessorType<DialogueCharacterProcessor>("character");
        }

        public void RegisterProcessorType<T>(string attribute)
            where T : IDialogueProcessor, new()
        {
            var pool = new Pool<IDialogueProcessor>(() => new T(), (dp) => dp.Reset(), true);
            _processorPools.Add(attribute, pool);
        }

        public void RegisterProcessorType<T>(params string[] attributes)
            where T : IDialogueProcessor, new()
        {
            var pool = new Pool<IDialogueProcessor>(() => new T(), (dp) => dp.Reset(), true);
            foreach(var attribute in attributes)
                _processorPools.Add(attribute, pool);
        }

        public void RegisterProcessorType<T>(string attribute, Func<IDialogueProcessor> createProcessor)
        {
            var pool = new Pool<IDialogueProcessor>(createProcessor, (dp) => dp.Reset(), true);
            _processorPools.Add(attribute, pool);
        }

        public IDialogueProcessor[] HandleCharacter(ref MarkupParseResult markup)
        {

        }

        public IDialogueProcessor Get(MarkupAttribute attribute)
        {
            if(!_processorPools.TryGetValue(attribute.Name, out var processorPool))
            {
                // No need to require a character attribute processor since it's built-in with
                // no well-defined behavior.
                if (attribute.Name == "character")
                    return _nop;

                throw new InvalidOperationException(
                    $"Failed to get an attribute processor for the attribute {attribute.Name}");
            }

            var processor = processorPool.Get();
            processor.Release = () => processorPool.Release(processor);
            processor.Init(_game, attribute);
            return processor;
        }
    }
}