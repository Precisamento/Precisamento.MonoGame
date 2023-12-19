using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Timers
{
    public abstract class GameTimer
    {
        public float CurrentTime { get; protected set; }
        public float Interval { get; set; }
        public TimerState State { get; protected set; }

        public GameTimer(float interval)
        {
            if(interval < 0)
                throw new ArgumentOutOfRangeException(nameof(interval), $"{nameof(interval)} must be greater than 0");
            Interval = interval;
        }

        public void Update(float deltaTime)
        {
            if (State != TimerState.Started || Interval == 0)
                return;

            CurrentTime += deltaTime;
            OnUpdate(deltaTime);
        }

        public void Start()
        {
            State = TimerState.Started;
            OnStart();
        }

        public void Stop()
        {
            State = TimerState.Stopped;
            CurrentTime = 0;
            OnStopped();
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Pause()
        {
            State = TimerState.Paused;
        }

        protected virtual void OnStart() { }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnStopped() { }
    }
}
