using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueOptionDisplay
    {
        public DialogueFrame Line { get; set; }
        public int DialogueOptionId { get; set; }
        public bool IsAvailable { get; set; }
    }
}
