using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Systems.Graphics
{
    [With(typeof(SpriteAnimationPlayer))]
    [With(typeof(Transform2))]
    public abstract class GeneralAnimationDrawSystem : AEntitySetSystem<SpriteBatchState>
    {
        protected GeneralAnimationDrawSystem(World world)
            : base(world)
        {
        }

        protected GeneralAnimationDrawSystem(World world, bool useBuffer)
            : base(world, useBuffer)
        {
        }

        protected override void Update(SpriteBatchState state, ReadOnlySpan<Entity> entities)
        {
            var moreToDraw = false;
            var currentLayer = int.MinValue;
            var nextLayer = int.MaxValue;

            do
            {
                moreToDraw = false;

                foreach(ref readonly Entity entity in entities)
                {
                    int layer = entity.Has<LayerComponent>() ? entity.Get<LayerComponent>().Layer : int.MinValue;
                    if (layer == currentLayer)
                    {
                        Update(state, in entity);
                    }
                    else if (layer > currentLayer && layer < nextLayer)
                    {
                        nextLayer = layer;
                        moreToDraw = true;
                    }
                }

                currentLayer = nextLayer;
                nextLayer = int.MaxValue;
            }
            while (moreToDraw);
        }

        protected override sealed void Update(SpriteBatchState state, in Entity entity)
        {
            ref var animationPlayer = ref entity.Get<SpriteAnimationPlayer>();
            ref var transform = ref entity.Get<Transform2>();

            if (entity.Has<SpriteDrawParams>())
            {
                ref var drawParams = ref entity.Get<SpriteDrawParams>();
                animationPlayer.Draw(state, transform, ref drawParams);
            }
            else
            {
                animationPlayer.Draw(state, transform);
            }
        }
    }

    [Without(typeof(GuiComponent))]
    public class AnimationDrawSystem : GeneralAnimationDrawSystem
    {
        public AnimationDrawSystem(World world)
            : base(world)
        {
        }

        public AnimationDrawSystem(World world, bool useBuffer)
            : base(world, useBuffer)
        {
        }
    }

    [With(typeof(GuiComponent))]
    public class GuiAnimationDrawSystem : GeneralAnimationDrawSystem
    {
        protected GuiAnimationDrawSystem(World world)
            : base(world)
        {
        }

        protected GuiAnimationDrawSystem(World world, bool useBuffer)
            : base(world, useBuffer)
        {
        }
    }
}
