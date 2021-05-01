using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Input
{
    public interface IActionMap
    {
        bool CurrentPressed { get; }
        bool PreviousPressed { get; }

        void Update(InputManager manager);
    }
}
