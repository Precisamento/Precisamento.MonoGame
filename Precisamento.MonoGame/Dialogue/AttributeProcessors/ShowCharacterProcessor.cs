﻿using Precisamento.MonoGame.Dialogue.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.AttributeProcessors
{
    public class ShowCharacterProcessor : DialogueProcessor
    {
        private CharacterParams _character;
        private bool _set = false;

        public ShowCharacterProcessor(CharacterParams character)
        {
            _character = character;
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

            state.Characters.Adding.Add(_character);
            _set = true;
        }
    }
}
