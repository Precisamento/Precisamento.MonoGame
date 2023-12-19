using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Systems.Debugging
{
    [With(typeof(Collider))]
    public class ColliderDebugSystem : AEntitySetSystem<SpriteBatchState>
    {
        public Color DebugColor { get; set; }

        public ColliderDebugSystem(Color color, World world) : base(world)
        {
            DebugColor = color;
        }

        protected override void Update(SpriteBatchState state, in Entity entity)
        {
            ref var collider = ref entity.Get<Collider>();
            var color = DebugColor;

            if(entity.Has<DebugColor>())
                color = entity.Get<DebugColor>().Color;

            collider.DebugDraw(state.SpriteBatch, color);
            state.SpriteBatch.DrawCircle(collider.Position, 4, 16, color);
        }
    }
}
