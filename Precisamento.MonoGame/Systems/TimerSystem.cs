using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;
using Precisamento.MonoGame.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Systems
{
    [WithEither(typeof(GameTimer), typeof(CountdownTimer), typeof(ContinuousTimer))]
    public class TimerSystem : AEntitySetSystem<float>
    {
        public TimerSystem(EntitySet set, bool useBuffer = false) : base(set, useBuffer)
        {
        }

        public TimerSystem(World world, bool useBuffer = false) : base(world, useBuffer)
        {
        }

        public TimerSystem(EntitySet set, IParallelRunner runner, int minEntityCountByRunnerIndex = 0) : base(set, runner, minEntityCountByRunnerIndex)
        {
        }

        public TimerSystem(World world, IParallelRunner runner, int minEntityCountByRunnerIndex = 0) : base(world, runner, minEntityCountByRunnerIndex)
        {
        }

        public TimerSystem(World world, Func<object, World, EntitySet> factory, bool useBuffer) : base(world, factory, useBuffer)
        {
        }

        public TimerSystem(World world, Func<object, World, EntitySet> factory, IParallelRunner runner, int minEntityCountByRunnerIndex) : base(world, factory, runner, minEntityCountByRunnerIndex)
        {
        }

        protected override void Update(float state, in Entity entity)
        {
            if(entity.Has<GameTimer>())
            {
                ref var timer = ref entity.Get<GameTimer>();
                timer.Update(state);
            }

            if (entity.Has<CountdownTimer>())
            {
                ref var timer = ref entity.Get<CountdownTimer>();
                timer.Update(state);
            }

            if (entity.Has<ContinuousTimer>())
            {
                ref var timer = ref entity.Get<ContinuousTimer>();
                timer.Update(state);
            }
        }
    }
}
