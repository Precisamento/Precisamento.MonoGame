using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Input
{
    public class SinglePlayerActionManager : IActionManager
    {
        private InputManager _manager;

        public SingleActionMap[] Actions { get; }
        public int PlayerCount => 1;

        public SinglePlayerActionManager(int actionCount, InputManager manager)
        {
            Actions = new SingleActionMap[actionCount];
            for (int i = 0; i < actionCount; i++)
                Actions[i] = new SingleActionMap();

            _manager = manager;
        }

        public bool ActionCheck(int action)
        {
            return Actions[action].CurrentPressed;
        }

        bool IActionManager.ActionCheck(int action, int player) => ActionCheck(action);

        public bool ActionCheckPressed(int action)
        {
            return Actions[action].CurrentPressed && !Actions[action].PreviousPressed;
        }

        bool IActionManager.ActionCheckPressed(int action, int player) => ActionCheckPressed(action);

        public bool ActionCheckReleased(int action)
        {
            return !Actions[action].CurrentPressed && Actions[action].PreviousPressed;
        }

        bool IActionManager.ActionCheckReleased(int action, int player) => ActionCheckReleased(action);

        public void Update()
        {
            for (int i = 0; i < Actions.Length; i++)
            {
                Actions[i].Update(_manager);
            }
        }
    }
}
