using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Systems.Collisions
{
    [With(typeof(VelocityComponent), typeof(Collider))]
    [Without(typeof(TransientComponent), typeof(StaticBodyComponent))]
    public class CollisionResolutionSystem : AEntitySetSystem<float>
    {
        private ICollisionWorld _collisionWorld;
        private HashSet<Collider> _cache = new HashSet<Collider>();

        public CollisionResolutionSystem(ICollisionWorld collisionWorld, EntitySet set, bool useBuffer)
            : base(set, useBuffer)
        {
            _collisionWorld = collisionWorld;
        }

        public CollisionResolutionSystem(ICollisionWorld collisionWorld, EntitySet set)
            : base(set)
        {
            _collisionWorld = collisionWorld;
        }

        public CollisionResolutionSystem(ICollisionWorld collisionWorld, World world, Func<object, World, EntitySet> factory, bool useBuffer)
            : base(world, factory, useBuffer)
        {
            _collisionWorld = collisionWorld;
        }

        public CollisionResolutionSystem(ICollisionWorld collisionWorld, World world, bool useBuffer)
            : base(world, useBuffer)
        {
            _collisionWorld = collisionWorld;
        }

        public CollisionResolutionSystem(ICollisionWorld collisionWorld, World world)
            : base(world)
        {
            _collisionWorld = collisionWorld;
        }

        protected override void Update(float deltaTime, in Entity entity)
        {
            if (entity.Has<Collider>())
            {
                ref var collider = ref entity.Get<Collider>();

                if (_collisionWorld.Collisions(collider, _cache))
                {

                    foreach (var other in _cache)
                    {
                        if (other.Tag is Entity otherEntity && otherEntity.Has<TransientComponent>())
                            continue;


                        collider.CollidesWithShape(other, out var collision, out var ray);

                        var mtv = Vector2.Zero;

                        if (collision.MinimumTranslationVector != Vector2.Zero)
                        {
                            mtv = collision.MinimumTranslationVector;
                        }
                        else if (ray.Distance != 0)
                        {
                            // Todo: Validate ray collision handler.
                            mtv = ray.Point;
                        }

                        if (mtv == Vector2.Zero)
                            continue;
                        _collisionWorld.Move(collider, -mtv);
                    }
                }

                _cache.Clear();
            }
        }
    }
}
