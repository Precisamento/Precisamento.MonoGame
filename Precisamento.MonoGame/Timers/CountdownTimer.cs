using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Timers
{
    public class CountdownTimer : GameTimer
    {
        public float TimeRemaining { get; private set; }

        public CountdownTimer(float interval) : base(interval)
        {
        }

        public event EventHandler? Completed;

        protected override void OnUpdate(float deltaTime)
        {
            TimeRemaining = Interval - CurrentTime;

            if(CurrentTime >= Interval)
            {
                State = TimerState.Completed;
                CurrentTime = Interval;
                TimeRemaining = 0;
                Completed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
