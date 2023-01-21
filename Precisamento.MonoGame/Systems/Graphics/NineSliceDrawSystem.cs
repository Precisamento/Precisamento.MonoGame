using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Systems.Graphics
{
    [With(typeof(NinePatchRegion2D), typeof(NinePatchComponent))]
    public abstract class GeneralNinePatchDrawSystem : AEntitySetSystem<SpriteBatchState>
    {
        protected GeneralNinePatchDrawSystem(EntitySet set, bool useBuffer) : base(set, useBuffer)
        {
        }

        protected GeneralNinePatchDrawSystem(EntitySet set) : base(set)
        {
        }

        protected GeneralNinePatchDrawSystem(World world, Func<object, World, EntitySet> factory, bool useBuffer) : base(world, factory, useBuffer)
        {
        }

        protected GeneralNinePatchDrawSystem(World world, bool useBuffer) : base(world, useBuffer)
        {
        }

        protected GeneralNinePatchDrawSystem(World world) : base(world)
        {
        }

        protected override sealed void Update(SpriteBatchState state, in Entity entity)
        {
            var color = Color.White;

            if (entity.Has<SpriteDrawParams>())
            {
                ref var drawParams = ref entity.Get<SpriteDrawParams>();
                if (drawParams.Invisible)
                    return;

                color = drawParams.Color;
            }

            ref var ninePatch = ref entity.Get<NinePatchRegion2D>();
            var bounds = entity.Get<NinePatchComponent>().Bounds;

            state.SpriteBatch.Draw(ninePatch, bounds, color, null);
        }
    }

    [Without(typeof(GuiComponent))]
    public class NinePatchDrawSystem : GeneralNinePatchDrawSystem
    {
        public NinePatchDrawSystem(EntitySet set) : base(set)
        {
        }

        public NinePatchDrawSystem(World world) : base(world)
        {
        }

        public NinePatchDrawSystem(EntitySet set, bool useBuffer) : base(set, useBuffer)
        {
        }

        public NinePatchDrawSystem(World world, bool useBuffer) : base(world, useBuffer)
        {
        }

        public NinePatchDrawSystem(World world, Func<object, World, EntitySet> factory, bool useBuffer) : base(world, factory, useBuffer)
        {
        }
    }

    [With(typeof(GuiComponent))]
    public class GuiNinePatchDrawSystem : GeneralNinePatchDrawSystem
    {
        public GuiNinePatchDrawSystem(EntitySet set) : base(set)
        {
        }

        public GuiNinePatchDrawSystem(World world) : base(world)
        {
        }

        public GuiNinePatchDrawSystem(EntitySet set, bool useBuffer) : base(set, useBuffer)
        {
        }

        public GuiNinePatchDrawSystem(World world, bool useBuffer) : base(world, useBuffer)
        {
        }

        public GuiNinePatchDrawSystem(World world, Func<object, World, EntitySet> factory, bool useBuffer) : base(world, factory, useBuffer)
        {
        }
    }
}
