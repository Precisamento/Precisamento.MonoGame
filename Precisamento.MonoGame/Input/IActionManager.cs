using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Input
{
    public interface IActionManager
    {
        int PlayerCount { get; }

        bool ActionCheck(int action);
        bool ActionCheck(int action, int player);
        bool ActionCheckPressed(int action);
        bool ActionCheckPressed(int action, int player);
        bool ActionCheckReleased(int action);
        bool ActionCheckReleased(int action, int player);

        void Update();
    }
}
