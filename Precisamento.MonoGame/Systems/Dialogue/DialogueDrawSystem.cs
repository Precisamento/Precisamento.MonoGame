using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;
using Precisamento.MonoGame.Dialogue;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Systems.Dialogue
{
    public class DialogueDrawSystem : AComponentSystem<SpriteBatchState, DialogueBox>
    {
        public DialogueDrawSystem(World world) : base(world)
        {
        }

        public DialogueDrawSystem(World world, IParallelRunner runner) : base(world, runner)
        {
        }

        public DialogueDrawSystem(World world, IParallelRunner runner, int minComponentCountByRunnerIndex) : base(world, runner, minComponentCountByRunnerIndex)
        {
        }

        protected override void Update(SpriteBatchState state, ref DialogueBox component)
        {
            component.Draw(state);
        }
    }
}
