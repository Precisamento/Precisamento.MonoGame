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

        public SingleActionMap Add(Keys key)
        {
            Keys.Add(key);
            return this;
        }

        public SingleActionMap Add(Buttons button)
        {
            Buttons.Add(button);
            return this;
        }

        public SingleActionMap Add(MouseButtons mouseButton)
        {
            MouseButtons.Add(mouseButton);
            return this;
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
