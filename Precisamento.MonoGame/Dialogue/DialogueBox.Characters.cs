using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Dialogue.Characters;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue
{
    public partial class DialogueBox
    {
        private void HandleShowCommand(string[] args)
        {
            var character = ShowCommand.ParseShowCommand(args, _characterFactory);
            _characterState.Adding.Add(character);
        }

        private void HandleHideCommand(string[] args)
        {
            var character = _characterFactory.GetCharacter(args[1]);
            _characterState.Removing.Add(character);
        }
    }
}
