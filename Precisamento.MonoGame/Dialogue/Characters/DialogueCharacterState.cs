﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public class DialogueCharacterState
    {
        public List<CharacterParams> Adding { get; } = new();
        public List<CharacterProfile> Removing { get; } = new();
    }
}
