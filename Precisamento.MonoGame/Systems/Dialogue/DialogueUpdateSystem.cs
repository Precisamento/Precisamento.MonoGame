using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;
using Precisamento.MonoGame.Dialogue;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Systems.Dialogue
{
    public class DialogueUpdateSystem : AComponentSystem<float, DialogueBox>
    {
        public DialogueUpdateSystem(World world) : base(world)
        {
        }

        public DialogueUpdateSystem(World world, IParallelRunner runner) : base(world, runner)
        {
        }

        public DialogueUpdateSystem(World world, IParallelRunner runner, int minComponentCountByRunnerIndex) : base(world, runner, minComponentCountByRunnerIndex)
        {
        }

        protected override void Update(float state, ref DialogueBox dialogueBox)
        {
            dialogueBox.Update(state);
        }
    }
}
