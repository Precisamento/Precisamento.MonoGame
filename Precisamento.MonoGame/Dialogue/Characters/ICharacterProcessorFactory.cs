using Precisamento.MonoGame.Dialogue.AttributeProcessors;
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
        public IEnumerable<IDialogueProcessor> CreateProcessorsForCharacter(ref MarkupParseResult markup);
    }
}
