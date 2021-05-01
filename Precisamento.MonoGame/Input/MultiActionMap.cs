using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Input
{
    public class MultipleActionMap : IActionMap
    {
        public struct MultiplayerGamePadButton
        {
            public Buttons Button;
            public int GamePadIndex;
        }

        public List<Keys> Keys { get; } = new List<Keys>();
        public List<MultiplayerGamePadButton> Buttons { get; } = new List<MultiplayerGamePadButton>();
        public List<MouseButtons> MouseButtons { get; } = new List<MouseButtons>();

        public bool CurrentPressed { get; private set; }
        public bool PreviousPressed { get; private set; }

        public void Add(Buttons button, int gamePadIndex)
        {
            Buttons.Add(new MultiplayerGamePadButton { Button = button, GamePadIndex = gamePadIndex });
        }

        public void Update(InputManager manager)
        {
            PreviousPressed = CurrentPressed;
            foreach (var key in Keys)
            {
                if (manager.KeyCheck(key))
                {
                    CurrentPressed = true;
                    return;
                }
            }

            foreach (var button in MouseButtons)
            {
                if (manager.MouseCheck(button))
                {
                    CurrentPressed = true;
                    return;
                }
            }

            foreach (var button in Buttons)
            {
                if (manager.GamePadCheck(button.Button, button.GamePadIndex))
                {
                    CurrentPressed = true;
                    return;
                }
            }

            CurrentPressed = false;
        }
    }
}
