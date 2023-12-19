using DefaultEcs.System;
using DefaultEcs;
using MonoGame.Extended;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Precisamento.MonoGame.UI;

namespace Precisamento.MonoGame.Systems.UI
{
    [With(typeof(ButtonComponent), typeof(TextComponent), typeof(Collider))]
    [Without(typeof(Transform2))]
    public class ButtonDrawSystem : AEntitySetSystem<SpriteBatchState>
    {
        public ButtonDrawSystem(World world, bool useBuffer = false) : base(world, useBuffer)
        {
        }

        protected override void Update(SpriteBatchState state, in Entity entity)
        {
            ref TextComponent text = ref entity.Get<TextComponent>();
            ref Collider collider = ref entity.Get<Collider>();

            var bounds = collider.BoundingBox;
            var size = text.Size;

            var position = bounds.Position + ((Vector2)bounds.Size - size) / 2;
            text.Font.Draw(
                state.SpriteBatch,
                text.Text,
                position,
                text.TextColor);
        }
    }
}
