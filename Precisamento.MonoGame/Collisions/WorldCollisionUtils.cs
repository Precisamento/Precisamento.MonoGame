using DefaultEcs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public static class WorldCollisionUtils
    {
        /// <summary>
        /// Subscribes handlers to the component events on a <see cref="World"/> to automate adding and removing <see cref="Collider"/>s to a <see cref="ICollisionWorld"/>.
        /// </summary>
        /// <remarks>
        /// The colliders will still have to be moved through the <see cref="ICollisionWorld"/> methods.
        /// </remarks>
        public static void AddCollisions(this World world, ICollisionWorld collisionWorld)
        {
            world.SubscribeComponentAdded((in Entity entity, in Collider collider) =>
            {
                collider.Tag = entity;
                collisionWorld.Add(collider);
            });

            world.SubscribeComponentChanged((in Entity entity, in Collider oldCollider, in Collider newCollider) =>
            {
                newCollider.Tag = entity;
                collisionWorld.Remove(oldCollider);
                collisionWorld.Add(newCollider);
            });

            world.SubscribeComponentRemoved((in Entity _, in Collider collider) => collisionWorld.Remove(collider));

            world.SubscribeComponentDisabled((in Entity _, in Collider collider) => collisionWorld.Remove(collider));

            world.SubscribeComponentEnabled((in Entity _, in Collider collider) => collisionWorld.Add(collider));
        }
    }
}
