using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Input
{
    public class SingleActionMap : IActionMap
    {
        public List<Keys> Keys { get; } = new List<Keys>();
        public List<Buttons> Buttons { get; } = new List<Buttons>();
        public List<MouseButtons> MouseButtons { get; } = new List<MouseButtons>();

        public bool CurrentPressed { get; private set; }
        public bool PreviousPressed { get; private set; }

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
                foreach (var controller in manager.ConnectedGamePads)
                {
                    if (manager.GamePadCheck(button, controller))
                    {
                        CurrentPressed = true;
                        return;
                    }
                }
            }

            CurrentPressed = false;
        }
    }
}
