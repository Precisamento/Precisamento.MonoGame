using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public abstract class CollisionWorld : ICollisionWorld
    {
        public abstract void Add(Collider collider);
        public abstract void All(ISet<Collider> results);
        public abstract void All(ISet<Collider> results, Predicate<Collider> predicate);
        public abstract void Broadphase(Vector2 point, ISet<Collider> results);
        public abstract void Broadphase(RectangleF rect, ISet<Collider> results);
        public abstract void Broadphase(Collider collider, ISet<Collider> results);
        public abstract void Clear();
        public abstract bool CollidesWithAny(Vector2 point);
        public abstract bool CollidesWithAny(Vector2 point, CollisionFilter<Vector2> predicate);
        public abstract bool CollidesWithAny(RectangleF rect);
        public abstract bool CollidesWithAny(RectangleF rect, CollisionFilter<RectangleF> predicate);
        public abstract bool CollidesWithAny(Collider collider);
        public abstract bool CollidesWithAny(Collider collider, CollisionFilter<Collider> predicate);
        public abstract bool Collisions(Vector2 point, ISet<Collider> results);
        public abstract bool Collisions(Vector2 point, ISet<Collider> results, CollisionFilter<Vector2> predicate);
        public abstract bool Collisions(RectangleF rect, ISet<Collider> results);
        public abstract bool Collisions(RectangleF rect, ISet<Collider> results, CollisionFilter<RectangleF> predicate);
        public abstract bool Collisions(Collider collider, ISet<Collider> results);
        public abstract bool Collisions(Collider collider, ISet<Collider> results, CollisionFilter<Collider> predicate);
        public abstract Collider FirstOrDefault(Vector2 point);
        public abstract Collider FirstOrDefault(Vector2 point, CollisionFilter<Vector2> predicate);
        public abstract Collider FirstOrDefault(RectangleF rect);
        public abstract Collider FirstOrDefault(RectangleF rect, CollisionFilter<RectangleF> predicate);
        public abstract Collider FirstOrDefault(Collider collider);
        public abstract Collider FirstOrDefault(Collider collider, CollisionFilter<Collider> predicate);
        public abstract void Remove(Collider collider);
        public abstract void RemoveWithBruteForce(Collider collider);

        public virtual void Move(Collider collider, Vector2 delta)
        {
            Remove(collider);
            collider.Position += delta;
            Add(collider);
        }

        public virtual void SetPosition(Collider collider, Vector2 position)
        {
            Remove(collider);
            collider.Position = position;
            Add(collider);
        }

        public virtual void Rotate(Collider collider, float deltaRotation)
        {
            if (collider.ColliderType == ColliderType.Circle)
            {
                collider.Rotation += deltaRotation;
                return;
            }

            Remove(collider);
            collider.Rotation += deltaRotation;
            Add(collider);
        }

        public virtual void SetRotatation(Collider collider, float rotation)
        {
            if (collider.ColliderType == ColliderType.Circle)
            {
                collider.Rotation = rotation;
                return;
            }

            Remove(collider);
            collider.Rotation = rotation;
            Add(collider);
        }

        public virtual void SetScale(Collider collider, float scale)
        {
            Remove(collider);
            collider.Scale = scale;
            Add(collider);
        }

        public virtual void MoveTransform(Collider collider, Transform2 transform)
        {
            Remove(collider);
            collider.Position += transform.Position;
            collider.Rotation += transform.Rotation;
            collider.Scale += transform.Scale.X;
            Add(collider);
        }

        public virtual void SetTransform(Collider collider, Transform2 transform)
        {
            Remove(collider);
            collider.Position = transform.Position;
            collider.Rotation = transform.Rotation;
            collider.Scale = transform.Scale.X;
            Add(collider);
        }
    }
}
