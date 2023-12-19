using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Options
{
    public class DialogueOptionDisplay
    {
        public DialogueFrame Line { get; set; }
        public int DialogueOptionId { get; set; }
        public bool IsAvailable { get; set; }

        public DialogueOptionDisplay(DialogueFrame line, int dialogueOptionId, bool isAvailable)
        {
            Line = line;
            DialogueOptionId = dialogueOptionId;
            IsAvailable = isAvailable;
        }
    }
}
