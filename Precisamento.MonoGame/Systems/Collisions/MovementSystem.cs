using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Precisamento.MonoGame.Systems.Collisions
{
    [With(typeof(VelocityComponent))]
    [WithEither(typeof(Transform2), typeof(Collider))]
    public class MovementSystem : AEntitySetSystem<float>
    {
        private ICollisionWorld _collisionWorld;
        private HashSet<Collider> _cache = new HashSet<Collider>();

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
            if (velocity.Velocity == Vector2.Zero)
                return;

            var movementDelta = velocity.Velocity;

            if(entity.Has<Transform2>())
            {
                ref var transform = ref entity.Get<Transform2>();
                transform.Position += movementDelta;
            }

            if(entity.Has<Collider>())
            {
                ref var collider = ref entity.Get<Collider>();

                _collisionWorld.Move(collider, movementDelta);

                if(_collisionWorld.Collisions(collider, _cache))
                {
                    var mtv = Vector2.Zero;
                    float longestDistance = 0;

                    foreach(var other in _cache)
                    {
                        collider.CollidesWithShape(other, out var collision, out var ray);

                        if(collision.MinimumTranslationVector != Vector2.Zero)
                        {
                            var distance = collision.MinimumTranslationVector.LengthSquared();
                            if(distance > longestDistance)
                            {
                                mtv = collision.MinimumTranslationVector;
                                longestDistance = distance;
                            }
                        }
                        else if(ray.Distance != 0)
                        {
                            // Todo: Validate ray collision handler.
                            if(ray.Distance * ray.Distance > longestDistance)
                            {
                                mtv = ray.Point;
                                longestDistance = ray.Distance * ray.Distance;
                            }
                        }
                    }

                    _collisionWorld.Move(collider, -mtv);
                }

                _cache.Clear();
            }
        }
    }
}
