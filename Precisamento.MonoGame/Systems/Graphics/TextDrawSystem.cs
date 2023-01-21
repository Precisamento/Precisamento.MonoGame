using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Systems.Graphics
{
    [With(typeof(TextComponent), typeof(Transform2))]
    public abstract class TextDrawSystem : AEntitySetSystem<SpriteBatchState>
    {
        protected TextDrawSystem(EntitySet set)
            : base(set)
        {
        }

        protected TextDrawSystem(World world)
            : base(world)
        {
        }

        protected TextDrawSystem(World world, IParallelRunner runner)
            : base(world, runner)
        {
        }

        protected TextDrawSystem(World world, bool useBuffer)
            : base(world, useBuffer)
        {
        }

        protected TextDrawSystem(World world, IParallelRunner runner, int minEntityCountByRunnerIndex)
            : base(world, runner, minEntityCountByRunnerIndex)
        {
        }

        protected override void Update(SpriteBatchState state, in Entity entity)
        {
            ref var text = ref entity.Get<TextComponent>();
            ref var transform = ref entity.Get<Transform2>();

            if(entity.Has<TextDrawParams>())
            {
                ref var drawParams = ref entity.Get<TextDrawParams>();
                text.Font.Draw(
                    state.SpriteBatch,
                    text.Text,
                    transform.Position,
                    text.TextColor,
                    transform.Rotation,
                    drawParams.Origin,
                    transform.Scale,
                    drawParams.Effects,
                    drawParams.Depth);
            }
            else
            {
                text.Font.Draw(
                    state.SpriteBatch,
                    text.Text,
                    transform.Position,
                    text.TextColor,
                    transform.Rotation,
                    text.Size / 2,
                    transform.Scale,
                    SpriteEffects.None,
                    0f);
            }
        }
    }

    [With(typeof(GuiComponent))]
    public class GuiTextDrawSystem : TextDrawSystem
    {
        public GuiTextDrawSystem(EntitySet set) 
            : base(set)
        {
        }

        public GuiTextDrawSystem(World world) 
            : base(world)
        {
        }

        public GuiTextDrawSystem(World world, IParallelRunner runner) 
            : base(world, runner)
        {
        }

        public GuiTextDrawSystem(World world, bool useBuffer) 
            : base(world, useBuffer)
        {
        }

        public GuiTextDrawSystem(World world, IParallelRunner runner, int minEntityCountByRunnerIndex) 
            : base(world, runner, minEntityCountByRunnerIndex)
        {
        }
    }

    [Without(typeof(GuiComponent))]
    public class WorldTextDrawSystem : TextDrawSystem
    {
        public WorldTextDrawSystem(EntitySet set)
            : base(set)
        {
        }

        public WorldTextDrawSystem(World world)
            : base(world)
        {
        }

        public WorldTextDrawSystem(World world, IParallelRunner runner)
            : base(world, runner)
        {
        }

        public WorldTextDrawSystem(World world, bool useBuffer)
            : base(world, useBuffer)
        {
        }

        public WorldTextDrawSystem(World world, IParallelRunner runner, int minEntityCountByRunnerIndex)
            : base(world, runner, minEntityCountByRunnerIndex)
        {
        }
    }
}
