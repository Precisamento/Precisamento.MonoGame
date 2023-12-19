using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueNopProcessor : DialogueProcessor
    {
        public override void Pop(DialogueState state) { }
        public override void Push(DialogueState state) { }
    }
}
