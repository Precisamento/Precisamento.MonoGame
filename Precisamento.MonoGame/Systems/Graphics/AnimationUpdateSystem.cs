using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Systems.Graphics
{
    public class AnimationUpdateSystem : AComponentSystem<float, SpriteAnimationPlayer>
    {
        public AnimationUpdateSystem(World world)
            : base(world)
        {
        }

        public AnimationUpdateSystem(World world, IParallelRunner runner)
            : base(world, runner)
        {
        }

        public AnimationUpdateSystem(World world, IParallelRunner runner, int minComponentCountByRunnerIndex)
            : base(world, runner, minComponentCountByRunnerIndex)
        {
        }

        protected override void Update(float delta, ref SpriteAnimationPlayer component)
        {
            component.Update(delta);
        }
    }
}
