using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Systems.Graphics
{
    [WithEither(typeof(SpriteAnimationPlayer), typeof(SpriteAnimation), typeof(TextureRegion2D), typeof(Texture2D))]
    [WithEither(typeof(Transform2), typeof(NinePatchComponent))]
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
            if(entity.Has<SpriteAnimationPlayer>())
            {
                ref var animationPlayer = ref entity.Get<SpriteAnimationPlayer>();
                if (entity.Has<Transform2>())
                {
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
                else
                {
                    var bounds = entity.Get<NinePatchComponent>().Bounds;

                    if (entity.Has<SpriteDrawParams>())
                    {
                        ref var drawParams = ref entity.Get<SpriteDrawParams>();
                        animationPlayer.Draw(state, bounds, ref drawParams);
                    }
                    else
                    {
                        animationPlayer.Draw(state, bounds);
                    }
                }
            }
            else if(entity.Has<SpriteAnimation>())
            {
                ref var animation = ref entity.Get<SpriteAnimation>();

                if (entity.Has<Transform2>())
                {
                    ref var transform = ref entity.Get<Transform2>();

                    if (entity.Has<SpriteDrawParams>())
                    {
                        ref var drawParams = ref entity.Get<SpriteDrawParams>();
                        if (drawParams.Invisible)
                            return;

                        state.SpriteBatch.Draw(
                            animation.Frames[animation.StartFrameIndex],
                            Vector2Ext.Round(transform.Position),
                            drawParams.Color,
                            transform.Rotation,
                            animation.Origin,
                            transform.Scale,
                            drawParams.Effects,
                            drawParams.Depth);
                    }
                    else
                    {
                        state.SpriteBatch.Draw(
                            animation.Frames[animation.StartFrameIndex],
                            Vector2Ext.Round(transform.Position),
                            Color.White,
                            transform.Rotation,
                            animation.Origin,
                            transform.Scale,
                            SpriteEffects.None,
                            0f);
                    }
                }
                else
                {
                    var bounds = entity.Get<NinePatchComponent>().Bounds;

                    if (entity.Has<SpriteDrawParams>())
                    {
                        ref var drawParams = ref entity.Get<SpriteDrawParams>();
                        if (drawParams.Invisible)
                            return;

                        state.SpriteBatch.Draw(
                            animation.Frames[animation.StartFrameIndex],
                            bounds,
                            drawParams.Color);
                    }
                    else
                    {
                        state.SpriteBatch.Draw(
                            animation.Frames[animation.StartFrameIndex],
                            bounds,
                            Color.White);
                    }
                }
            }
            else if(entity.Has<TextureRegion2D>())
            {
                ref var texture = ref entity.Get<TextureRegion2D>();

                if (entity.Has<Transform2>())
                {
                    ref var transform = ref entity.Get<Transform2>();

                    if (entity.Has<SpriteDrawParams>())
                    {
                        ref var drawParams = ref entity.Get<SpriteDrawParams>();
                        if (drawParams.Invisible)
                            return;

                        state.SpriteBatch.Draw(
                            texture,
                            Vector2Ext.Round(transform.Position),
                            drawParams.Color,
                            transform.Rotation,
                            Vector2.Zero,
                            transform.Scale,
                            drawParams.Effects,
                            drawParams.Depth);
                    }
                    else
                    {
                        state.SpriteBatch.Draw(
                            texture,
                            Vector2Ext.Round(transform.Position),
                            Color.White,
                            transform.Rotation,
                            Vector2.Zero,
                            transform.Scale,
                            SpriteEffects.None,
                            0f);
                    }
                }
                else
                {
                    var bounds = entity.Get<NinePatchComponent>().Bounds;

                    if (entity.Has<SpriteDrawParams>())
                    {
                        ref var drawParams = ref entity.Get<SpriteDrawParams>();
                        if (drawParams.Invisible)
                            return;

                        state.SpriteBatch.Draw(
                            texture,
                            bounds,
                            drawParams.Color);
                    }
                    else
                    {
                        state.SpriteBatch.Draw(
                            texture,
                            bounds,
                            Color.White);
                    }
                }
            }
            else if(entity.Has<Texture2D>())
            {
                ref var texture = ref entity.Get<Texture2D>();

                if (entity.Has<Transform2>())
                {
                    ref var transform = ref entity.Get<Transform2>();

                    if (entity.Has<SpriteDrawParams>())
                    {
                        ref var drawParams = ref entity.Get<SpriteDrawParams>();
                        if (drawParams.Invisible)
                            return;

                        state.SpriteBatch.Draw(
                            texture,
                            Vector2Ext.Round(transform.Position),
                            null,
                            drawParams.Color,
                            transform.Rotation,
                            Vector2.Zero,
                            transform.Scale,
                            drawParams.Effects,
                            drawParams.Depth);
                    }
                    else
                    {
                        state.SpriteBatch.Draw(
                            texture,
                            Vector2Ext.Round(transform.Position),
                            null,
                            Color.White,
                            transform.Rotation,
                            Vector2.Zero,
                            transform.Scale,
                            SpriteEffects.None,
                            0f);
                    }
                }
                else
                {
                    var bounds = entity.Get<NinePatchComponent>().Bounds;

                    if (entity.Has<SpriteDrawParams>())
                    {
                        ref var drawParams = ref entity.Get<SpriteDrawParams>();
                        if (drawParams.Invisible)
                            return;

                        state.SpriteBatch.Draw(
                            texture,
                            bounds,
                            drawParams.Color);
                    }
                    else
                    {
                        state.SpriteBatch.Draw(
                            texture,
                            bounds,
                            Color.White);
                    }
                }
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
        public GuiAnimationDrawSystem(World world)
            : base(world)
        {
        }

        public GuiAnimationDrawSystem(World world, bool useBuffer)
            : base(world, useBuffer)
        {
        }
    }
}
