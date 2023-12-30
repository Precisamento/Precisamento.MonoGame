using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public class ButtonComponent
    {
        public Sprite Sprite { get; set; }
        public string CurrentState { get; set; }
        public string NormalState { get; set; }
        public string? HoverState { get; set; }
        public string? PressState { get; set; }
        public string? DisabledState { get; set; }
        public bool Enabled { get; set; } = true;
        public bool IsDown { get; set; }

        public event Action? Clicked;

        public ButtonComponent()
        {
        }

        public ButtonComponent(Sprite sprite, string normalState, string? hoverState, string? pressState, string? disabledState)
        {
            Sprite = sprite;
            CurrentState = normalState;
            NormalState = normalState;
            HoverState = hoverState;
            PressState = pressState;
            DisabledState = disabledState;
        }

        public void OnClick()
        {
            Clicked?.Invoke();
        }
    }
}
