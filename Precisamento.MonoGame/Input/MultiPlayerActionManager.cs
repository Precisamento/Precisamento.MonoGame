using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Input
{
    public class MultiPlayerActionMananger : IActionManager
    {
        private InputManager _manager;

        public MultipleActionMap[,] Actions { get; }
        public int PlayerCount => Actions.GetLength(1);

        public MultiPlayerActionMananger(int actionCount, int playerCount, InputManager manager)
        {
            Actions = new MultipleActionMap[actionCount, playerCount];

            for (int a = 0; a < Actions.GetLength(0); a++)
                for (int p = 0; p < Actions.GetLength(1); p++)
                    Actions[a, p] = new MultipleActionMap();

            _manager = manager;
        }

        public bool ActionCheck(int action, int player)
        {
            return Actions[action, player].CurrentPressed;
        }

        bool IActionManager.ActionCheck(int action) => ActionCheck(action, 0);

        public bool ActionCheckPressed(int action, int player)
        {
            return Actions[action, player].CurrentPressed && !Actions[action, player].PreviousPressed;
        }

        bool IActionManager.ActionCheckPressed(int action) => ActionCheckPressed(action, 0);

        public bool ActionCheckReleased(int action, int player)
        {
            return !Actions[action, player].CurrentPressed && Actions[action, player].PreviousPressed;
        }

        bool IActionManager.ActionCheckReleased(int action) => ActionCheckReleased(action, 0);

        public void Update()
        {
            for (int a = 0; a < Actions.GetLength(0); a++)
                for (int p = 0; p < Actions.GetLength(1); p++)
                    Actions[a, p].Update(_manager);
        }
    }
}
