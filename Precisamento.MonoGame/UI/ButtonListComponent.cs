using DefaultEcs;
using Precisamento.MonoGame.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public class ButtonListComponent
    {
        public const float INITIAL_WAIT = 0.5f;
        public const float HOLD_WAIT = 1f / 6;

        public List<Entity> Buttons { get; set; }
        public int UpAction { get; set; } = -1;
        public int DownAction { get; set; } = -1;
        public int ClickAction { get; set; } = -1;

        public float InitialWait { get; set; } = INITIAL_WAIT;
        public float HoldWait { get; set; } = HOLD_WAIT;
        public int SelectedIndex { get; set; } = -1;
        public int Direction { get; set; }
        public ContinuousTimer HoldTimer { get; } = new ContinuousTimer(0f);

        public ButtonListComponent(List<Entity> buttons)
        {
            Buttons = buttons;
            HoldTimer.Stop();
        }

        public ButtonListComponent(
            List<Entity> buttons,
            int upAction,
            int downAction,
            int clickAction)
        {
            Buttons = buttons;
            UpAction = upAction;
            DownAction = downAction;
            ClickAction = clickAction;
            HoldTimer.Stop();
        }
    }
}
