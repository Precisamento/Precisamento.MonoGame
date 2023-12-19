using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Precisamento.MonoGame.Systems.Collisions
{
    [With(typeof(VelocityComponent))]
    [WithEither(typeof(Transform2), typeof(Collider))]
    public class MovementSystem : AEntitySetSystem<float>
    {
        private ICollisionWorld _collisionWorld;

        public MovementSystem(ICollisionWorld collisionWorld, EntitySet set, bool useBuffer)
            : base(set, useBuffer)
        {
            _collisionWorld = collisionWorld;
        }

        public MovementSystem(ICollisionWorld collisionWorld, EntitySet set)
            : base(set)
        {
            _collisionWorld = collisionWorld;
        }

        public MovementSystem(ICollisionWorld collisionWorld, World world, Func<object, World, EntitySet> factory, bool useBuffer)
            : base(world, factory, useBuffer)
        {
            _collisionWorld = collisionWorld;
        }

        public MovementSystem(ICollisionWorld collisionWorld, World world, bool useBuffer)
            : base(world, useBuffer)
        {
            _collisionWorld = collisionWorld;
        }

        public MovementSystem(ICollisionWorld collisionWorld, World world)
            : base(world)
        {
            _collisionWorld = collisionWorld;
        }

        protected override void Update(float deltaTime, in Entity entity)
        {
            ref var velocity = ref entity.Get<VelocityComponent>();
            if (velocity.Delta.Position == Vector2.Zero
                && velocity.Delta.Rotation == 0
                && velocity.Delta.Scale == Vector2.Zero)
            {
                return;
            }

            if(entity.Has<Transform2>())
            {
                ref var transform = ref entity.Get<Transform2>();
                transform.Position += velocity.Delta.Position;
                transform.Rotation += velocity.Delta.Rotation;
                transform.Scale += velocity.Delta.Scale;
            }

            if(entity.Has<Collider>())
            {
                ref var collider = ref entity.Get<Collider>();
                _collisionWorld.MoveTransform(collider, velocity.Delta);
            }
        }
    }
}
