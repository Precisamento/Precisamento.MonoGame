using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.MathHelpers;
using Precisamento.MonoGame.Systems.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Scenes.CollisionTest
{
    [With(typeof(PlayerMoveComponent), typeof(Collider), typeof(VelocityComponent))]
    public class MoveSystem : AEntitySetSystem<float>
    {
        private const int SPEED = 200;

        private IActionManager _actions;

        public MoveSystem(Game game, World world)
            : base(world)
        {
            _actions = game.Services.GetService<IActionManager>();
        }

        protected override void Update(float deltaTime, in Entity entity)
        {
            ref var collider = ref entity.Get<Collider>();
            ref var velocity = ref entity.Get<VelocityComponent>();

            var rotateDelta = 0f;

            if(_actions.ActionCheck((int)Actions.RotateLeft))
                rotateDelta = MathHelper.ToRadians(-10);

            if (_actions.ActionCheck((int)Actions.RotateRight))
                rotateDelta = MathHelper.ToRadians(10);

            if (rotateDelta != 0)
                collider.Rotation += rotateDelta;

            var direction = Directions.None;

            if (_actions.ActionCheck((int)Actions.Up))
                direction |= Directions.North;

            if (_actions.ActionCheck((int)Actions.Right))
                direction |= Directions.East;

            if (_actions.ActionCheck((int)Actions.Down))
                direction |= Directions.South;

            if (_actions.ActionCheck((int)Actions.Left))
                direction |= Directions.West;

            if (direction == Directions.None)
            {
                velocity.Delta.Position = Vector2.Zero;
            }
            else
            {
                var delta = MathExt.LengthDir(SPEED * deltaTime, direction.ToRadians());
                delta.Round();

                velocity.Delta.Position = delta;
            }
        }
    }
}
