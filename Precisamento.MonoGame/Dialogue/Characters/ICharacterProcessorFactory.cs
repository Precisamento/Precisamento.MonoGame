using Precisamento.MonoGame.Dialogue.AttributeProcessors;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public interface ICharacterProcessorFactory
    {
        public IEnumerable<IDialogueProcessor> CreateProcessorsForCharacter(LocalizedLine line, out MarkupParseResult markup);
        public CharacterProfile GetCharacter(string name);
    }
}
