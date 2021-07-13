using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public delegate bool CollisionFilter<T>(T left, Collider right);

    public interface ICollisionWorld
    {
        /// <summary>
        /// Adds a collider to the collision world at its current location.
        /// </summary>
        /// <param name="collider">The collider to add.</param>
        void Add(Collider collider);

        /// <summary>
        /// Removes all items from the collision world.
        /// </summary>
        void Clear();

        /// <summary>
        /// Removes an object from the game world based on its current position and bounds.
        /// </summary>
        /// <param name="collider">The collider to remove.</param>
        void Remove(Collider collider);

        /// <summary>
        /// Removes a collider from the collision world by brute force instead of relying on its current position. 
        /// </summary>
        /// <param name="collider">The collider to remove.</param>
        /// <remarks>
        /// This method can be much slower than just removing normally. It should only be used in cases where 
        /// the collider has had its position manually updated for some reason so it's position in the collision world is not known.
        /// </remarks>
        void RemoveWithBruteForce(Collider collider);

        /// <summary>
        /// Updates the position of the collider both on the instance and in the CollisionWorld. Do not mix-and-match with manually setting <see cref="Collider.Position"/>.
        /// </summary>
        /// <param name="collider">The collider to move.</param>
        /// <param name="delta">The distance to move it.</param>
        void Move(Collider collider, Vector2 delta);

        void SetPosition(Collider collider, Vector2 position);

        /// <summary>
        /// Rotates a collider by a certain amount in radians from its current rotation. Do not mix-and-match with manually setting <see cref="Collider.Rotation"/>.
        /// </summary>
        /// <param name="collider">The collider to rotate.</param>
        /// <param name="deltaRotation">The offset from the colliders current rotation in radians.</param>
        void Rotate(Collider collider, float deltaRotation);

        /// <summary>
        /// Sets the rotation of a collider. Do not mix-and-match with manually setting <see cref="Collider.Rotation"/>.
        /// </summary>
        /// <param name="collider">The collider to rotate.</param>
        /// <param name="rotation">The rotation of the collider in radians.</param>
        void SetRotatation(Collider collider, float rotation);

        /// <summary>
        /// Sets the scale of a collider. Do not mix-and-match with manually setting <see cref="Collider.Scale"/>.
        /// </summary>
        /// <param name="collider">The collider to scale.</param>
        /// <param name="scale">The value to scale the collider by. Must be greater than 0.</param>
        void SetScale(Collider collider, float scale);

        /// <summary>
        /// Determines if the point collides with any colliders.
        /// </summary>
        /// <param name="point">The position to check.</param>
        bool CollidesWithAny(Vector2 point);

        /// <summary>
        /// Determines if the point collides with any colliders that match a predicate.
        /// </summary>
        /// <param name="point">The position to check.</param>
        /// <param name="predicate">The predicate used to filter out any undesirable colliders.</param>
        bool CollidesWithAny(Vector2 point, CollisionFilter<Vector2> predicate);

        /// <summary>
        /// Determines if the rectangle intersects with any colliders.
        /// </summary>
        /// <param name="rect">The rectangular section to check.</param>
        bool CollidesWithAny(RectangleF rect);

        /// <summary>
        /// Determines if the rectangle intersects with any colliders that match a predicate.
        /// </summary>
        /// <param name="rect">The rectangular section to check.</param>
        /// <param name="predicate">The predicate used to filter out any undesirable colliders.</param>
        bool CollidesWithAny(RectangleF rect, CollisionFilter<RectangleF> predicate);

        /// <summary>
        /// Determines if the collider intersects with any other colliders.
        /// </summary>
        /// <param name="collider">The collider to check.</param>
        bool CollidesWithAny(Collider collider);

        /// <summary>
        /// Determines if the collider intersects with any other colliders that match a predicate.
        /// </summary>
        /// <param name="collider">The collider to check.</param>
        /// <param name="predicate">The predicate used to filter out any undesirable colliders.</param>
        bool CollidesWithAny(Collider collider, CollisionFilter<Collider> predicate);

        /// <summary>
        /// Fills a set with any colliders that collide with a point.
        /// </summary>
        /// <param name="point">The point to check for colliders.</param>
        /// <param name="results">The set that's filled with the results.</param>
        /// <returns>True if any colliders were added. False otherwise</returns>
        bool Collisions(Vector2 point, ISet<Collider> results);

        /// <summary>
        /// Fills a set with any colliders that collide with a point and match a predicate.
        /// </summary>
        /// <param name="point">The point to check for colliders.</param>
        /// <param name="results">The set that's filled with the results.</param>
        /// <param name="predicate">The predicate used to filter out any undesirable colliders.</param>
        /// <returns>True if any colliders were added. False otherwise</returns>
        bool Collisions(Vector2 point, ISet<Collider> results, CollisionFilter<Vector2> predicate);

        /// <summary>
        /// Fills a set with any colliders that intersect with a rectangle.
        /// </summary>
        /// <param name="rect">The rectangular section to check.</param>
        /// <param name="results">The set that's filled with the results.</param>
        /// <returns>True if any colliders were added. False otherwise</returns>
        bool Collisions(RectangleF rect, ISet<Collider> results);

        /// <summary>
        /// Fills a set with any colliders that intersect with a rectangle and match a predicate.
        /// </summary>
        /// <param name="rect">The rectangular section to check.</param>
        /// <param name="results">The set that's filled with the results.</param>
        /// <param name="predicate">The predicate used to filter out any undesirable colliders.</param>
        /// <returns>True if any colliders were added. False otherwise</returns>
        bool Collisions(RectangleF rect, ISet<Collider> results, CollisionFilter<RectangleF> predicate);

        /// <summary>
        /// Fills a set with any other colliders that intersect a collider.
        /// </summary>
        /// <param name="collider">The collider to check.</param>
        /// <param name="results">The set that's filled with the results.</param>
        /// <returns>True if any colliders were added. False otherwise</returns>
        bool Collisions(Collider collider, ISet<Collider> results);

        /// <summary>
        /// Fills a set with any other colliders that intersect a collider and match a predicate.
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="results">The set that's filled with the results.</param>
        /// <param name="predicate">The predicate used to filter out any undesirable colliders.</param>
        /// <returns>True if any colliders were added. False otherwise</returns>
        bool Collisions(Collider collider, ISet<Collider> results, CollisionFilter<Collider> predicate);

        /// <summary>
        /// Gets the first collider that collides with a point.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>The first found collider or null if none were found.</returns>
        Collider FirstOrDefault(Vector2 point);

        /// <summary>
        /// Gets the first collider that collides with a point and matches a predicate.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="predicate">The predicate used to filter out any undesirable colliders.</param>
        /// <returns>The first found collider or null if none were found.</returns>
        Collider FirstOrDefault(Vector2 point, CollisionFilter<Vector2> predicate);

        /// <summary>
        /// Gets the first collider that intersects with a rectangular section.
        /// </summary>
        /// <param name="rect">The rectangular section to check.</param>
        /// <returns>The first found collider or null if none were found.</returns>
        Collider FirstOrDefault(RectangleF rect);

        /// <summary>
        /// Gets the first collider that intersects with a rectangular section and matches a predicate.
        /// </summary>
        /// <param name="rect">The rectangular section to check.</param>
        /// <param name="predicate">The predicate used to filter out any undesirable colliders.</param>
        /// <returns>The first found collider or null if none were found.</returns>
        Collider FirstOrDefault(RectangleF rect, CollisionFilter<RectangleF> predicate);

        /// <summary>
        /// Gets the first collider that collides with the specified collider.
        /// </summary>
        /// <param name="collider">The collider to check.</param>
        /// <returns>The first found collider or null if none were found.</returns>
        Collider FirstOrDefault(Collider collider);

        /// <summary>
        /// Gets the first collider that collides with the specified collider and matches a predicate.
        /// </summary>
        /// <param name="collider">The collider to check.</param>
        /// <param name="predicate">The predicate used to filter out any undesirable colliders.</param>
        /// <returns>The first found collider or null if none were found.</returns>
        Collider FirstOrDefault(Collider collider, CollisionFilter<Collider> predicate);

        /// <summary>
        /// Performs an implementation broadphase search that fills a set with all colliders that are near a point.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="results">The set that's filled with the results.</param>
        void Broadphase(Vector2 point, ISet<Collider> results);

        /// <summary>
        /// Performs an implementation broadphase search that fills a set with all colliders that are near a rectangle.
        /// </summary>
        /// <param name="rect">The rectangular section to check.</param>
        /// <param name="results">The set that's filled with the results.</param>
        void Broadphase(RectangleF rect, ISet<Collider> results);

        /// <summary>
        /// Performs an implementation broadphase search that fills a set with all colliders that are near another collider.
        /// </summary>
        /// <param name="collider">The collider to check.</param>
        /// <param name="results">The set that's filled with the results.</param>
        void Broadphase(Collider collider, ISet<Collider> results);

        /// <summary>
        /// Fills a set with all of the colliders currently in the CollisionWorld.
        /// </summary>
        /// <param name="results">The set that's filled with the results.</param>
        void All(ISet<Collider> results);

        /// <summary>
        /// Fills a set with all of the colliders currently in the CollisionWorld and that match a predicate.
        /// </summary>
        /// <param name="results">The set that's filled with the results.</param>
        /// <param name="predicate"></param>
        void All(ISet<Collider> results, Predicate<Collider> predicate);

    }
}
