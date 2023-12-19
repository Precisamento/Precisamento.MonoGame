using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Timers
{
    public class ContinuousTimer : GameTimer
    {
        public float NextTickTime { get; private set; }
        public bool Ticked { get; private set; }

        public ContinuousTimer(float interval) : base(interval)
        {
            NextTickTime = interval;
        }

        public event EventHandler? Tick;

        protected override void OnStart()
        {
            NextTickTime = Interval;
        }

        protected override void OnStopped()
        {
            NextTickTime = 0;
        }

        protected override void OnUpdate(float deltaTime)
        {
            NextTickTime -= deltaTime;
            Ticked = false;

            if (CurrentTime >= Interval)
            {
                NextTickTime = Interval;
                CurrentTime -= Interval;
                Tick?.Invoke(this, EventArgs.Empty);
                Ticked = true;
            }
        }
    }
}
