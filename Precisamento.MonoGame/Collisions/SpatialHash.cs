using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public class SpatialHash : CollisionWorld
    {
        private float _inverseCellSize;
        private SpatialStore _cells = new SpatialStore();
        private HashSet<Collider> _cache = new HashSet<Collider>();

        public SpatialHash(int cellSize)
        {
            _inverseCellSize = 1f / cellSize;
        }

        public override void Add(Collider collider)
        {
            foreach (var set in GetOverlappingSets(collider.BoundingBox))
                set.Add(collider);
        }

        public override void All(ISet<Collider> results)
        {
            _cells.GetAllObjects(results);
        }

        public override void All(ISet<Collider> results, Predicate<Collider> predicate)
        {
            _cache.Clear();
            _cells.GetAllObjects(_cache);
            foreach(var collider in _cache)
            {
                if (predicate(collider))
                    results.Add(collider);
            }
        }

        public override void Broadphase(Vector2 position, ISet<Collider> results)
        {
            var ex = MathExt.FastFloorToInt(position.X * _inverseCellSize);
            var ey = MathExt.FastFloorToInt(position.Y * _inverseCellSize);
            if (!_cells.TryGetValue(ex, ey, out var set))
                return;

            results.UnionWith(set);
        }

        public override void Broadphase(RectangleF rect, ISet<Collider> results)
        {
            foreach (var set in GetOverlappingSets(rect))
                results.UnionWith(set);
        }

        public override void Broadphase(Collider collider, ISet<Collider> results)
        {
            foreach (var set in GetOverlappingSets(collider.BoundingBox))
                results.UnionWith(set);

            results.Remove(collider);
        }

        public override void Clear()
        {
            _cells.Clear();
        }

        public override bool CollidesWithAny(Vector2 position)
        {
            var ex = MathExt.FastFloorToInt(position.X * _inverseCellSize);
            var ey = MathExt.FastFloorToInt(position.Y * _inverseCellSize);
            if (_cells.TryGetValue(ex, ey, out var set))
            {
                foreach (var collider in set)
                {
                    if (collider.ContainsPoint(position))
                        return true;
                }
            }

            return false;
        }

        public override bool CollidesWithAny(Vector2 position, CollisionFilter<Vector2> predicate)
        {
            var ex = MathExt.FastFloorToInt(position.X * _inverseCellSize);
            var ey = MathExt.FastFloorToInt(position.Y * _inverseCellSize);
            if (_cells.TryGetValue(ex, ey, out var set))
            {
                foreach (var collider in set)
                {
                    if (collider.ContainsPoint(position) && predicate(position, collider))
                        return true;
                }
            }

            return false;
        }

        public override bool CollidesWithAny(RectangleF rect)
        {
            foreach(var set in GetOverlappingSets(rect))
            {
                foreach (var collider in set)
                {
                    if (collider.CollidesWithRect(rect))
                        return true;
                }
            }

            return false;
        }

        public override bool CollidesWithAny(RectangleF rect, CollisionFilter<RectangleF> predicate)
        {
            foreach (var set in GetOverlappingSets(rect))
            {
                foreach (var collider in set)
                {
                    if (collider.CollidesWithRect(rect) && predicate(rect, collider))
                        return true;
                }
            }

            return false;
        }

        public override bool CollidesWithAny(Collider collider)
        {
            foreach (var set in GetOverlappingSets(collider.BoundingBox))
            {
                foreach (var other in set)
                {
                    if (collider != other && collider.Overlaps(other))
                        return true;
                }
            }

            return false;
        }

        public override bool CollidesWithAny(Collider collider, CollisionFilter<Collider> predicate)
        {
            foreach (var set in GetOverlappingSets(collider.BoundingBox))
            {
                foreach (var other in set)
                {
                    if (collider != other && collider.Overlaps(other) && predicate(collider, other))
                        return true;
                }
            }

            return false;
        }

        public override bool Collisions(Vector2 position, ISet<Collider> results)
        {
            var result = false;

            var ex = MathExt.FastFloorToInt(position.X * _inverseCellSize);
            var ey = MathExt.FastFloorToInt(position.Y * _inverseCellSize);
            if (_cells.TryGetValue(ex, ey, out var set))
            {
                foreach (var collider in set)
                {
                    if (collider.ContainsPoint(position))
                    {
                        results.Add(collider);
                        result = true;
                    }
                }
            }

            return result;
        }

        public override bool Collisions(Vector2 position, ISet<Collider> results, CollisionFilter<Vector2> predicate)
        {
            var result = false;

            var ex = MathExt.FastFloorToInt(position.X * _inverseCellSize);
            var ey = MathExt.FastFloorToInt(position.Y * _inverseCellSize);
            if (_cells.TryGetValue(ex, ey, out var set))
            {
                foreach (var collider in set)
                {
                    if (collider.ContainsPoint(position) && predicate(position, collider))
                    {
                        results.Add(collider);
                        result = true;
                    }
                }
            }

            return result;
        }

        public override bool Collisions(RectangleF rect, ISet<Collider> results)
        {
            var result = false;

            foreach (var set in GetOverlappingSets(rect))
            {
                foreach (var collider in set)
                {
                    if (collider.CollidesWithRect(rect))
                    {
                        results.Add(collider);
                        result = true;
                    }
                }
            }

            return result;
        }

        public override bool Collisions(RectangleF rect, ISet<Collider> results, CollisionFilter<RectangleF> predicate)
        {
            var result = false;

            foreach (var set in GetOverlappingSets(rect))
            {
                foreach (var collider in set)
                {
                    if (collider.CollidesWithRect(rect) && predicate(rect, collider))
                    {
                        results.Add(collider);
                        result = true;
                    }
                }
            }

            return result;
        }

        public override bool Collisions(Collider collider, ISet<Collider> results)
        {
            var result = false;

            foreach (var set in GetOverlappingSets(collider.BoundingBox))
            {
                foreach (var other in set)
                {
                    if (collider != other && collider.Overlaps(other))
                    {
                        results.Add(other);
                        result = true;
                    }
                }
            }

            return result;
        }

        public override bool Collisions(Collider collider, ISet<Collider> results, CollisionFilter<Collider> predicate)
        {
            var result = false;

            foreach (var set in GetOverlappingSets(collider.BoundingBox))
            {
                foreach (var other in set)
                {
                    if (collider != other && collider.Overlaps(other) && predicate(collider, other))
                    {
                        results.Add(other);
                        result = true;
                    }
                }
            }

            return result;
        }

        public override Collider FirstOrDefault(Vector2 position)
        {
            var ex = MathExt.FastFloorToInt(position.X * _inverseCellSize);
            var ey = MathExt.FastFloorToInt(position.Y * _inverseCellSize);
            if (_cells.TryGetValue(ex, ey, out var set))
            {
                foreach (var collider in set)
                {
                    if (collider.ContainsPoint(position))
                        return collider;
                }
            }

            return default;
        }

        public override Collider FirstOrDefault(Vector2 position, CollisionFilter<Vector2> predicate)
        {
            var ex = MathExt.FastFloorToInt(position.X * _inverseCellSize);
            var ey = MathExt.FastFloorToInt(position.Y * _inverseCellSize);
            if (_cells.TryGetValue(ex, ey, out var set))
            {
                foreach (var collider in set)
                {
                    if (collider.ContainsPoint(position) && predicate(position, collider))
                        return collider;
                }
            }

            return default;
        }

        public override Collider FirstOrDefault(RectangleF rect)
        {
            foreach (var set in GetOverlappingSets(rect))
            {
                foreach (var collider in set)
                {
                    if (collider.CollidesWithRect(rect))
                        return collider;
                }
            }

            return default;
        }

        public override Collider FirstOrDefault(RectangleF rect, CollisionFilter<RectangleF> predicate)
        {
            foreach (var set in GetOverlappingSets(rect))
            {
                foreach (var collider in set)
                {
                    if (collider.CollidesWithRect(rect) && predicate(rect, collider))
                        return collider;
                }
            }

            return default;
        }

        public override Collider FirstOrDefault(Collider collider)
        {
            foreach (var set in GetOverlappingSets(collider.BoundingBox))
            {
                foreach (var other in set)
                {
                    if (collider != other && collider.Overlaps(other))
                        return other;
                }
            }

            return default;
        }

        public override Collider FirstOrDefault(Collider collider, CollisionFilter<Collider> predicate)
        {
            foreach (var set in GetOverlappingSets(collider.BoundingBox))
            {
                foreach (var other in set)
                {
                    if (collider != other && collider.Overlaps(other) && predicate(collider, other))
                        return other;
                }
            }

            return default;
        }

        public override void Remove(Collider collider)
        {
            foreach (var set in GetOverlappingSets(collider.BoundingBox))
                set.Remove(collider);
        }

        public override void RemoveWithBruteForce(Collider collider)
        {
            _cells.Remove(collider);
        }

        private IEnumerable<HashSet<Collider>> GetOverlappingSets(RectangleF box)
        {
            var minX = MathExt.FastFloorToInt(box.Left * _inverseCellSize);
            var minY = MathExt.FastFloorToInt(box.Top * _inverseCellSize);
            var maxX = MathExt.FastFloorToInt(box.Right * _inverseCellSize) + 1;
            var maxY = MathExt.FastFloorToInt(box.Bottom * _inverseCellSize) + 1;

            for (var w = minX; w < maxX; ++w)
            {
                for (var h = minY; h < maxY; ++h)
                {
                    if (!_cells.TryGetValue(w, h, out var set))
                    {
                        set = new HashSet<Collider>();
                        _cells.Add(w, h, set);
                    }
                    yield return set;
                }
            }
        }
    }

    internal class SpatialStore
    {
        // Todo: Compare performance of HashSet vs List

        private Dictionary<long, HashSet<Collider>> _cells = new Dictionary<long, HashSet<Collider>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long GetKey(int x, int y)
        {
            return (long)x << 32 | (uint)y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int x, int y, HashSet<Collider> set)
        {
            _cells.Add(GetKey(x, y), set);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Collider entity)
        {
            foreach (var set in _cells.Values)
            {
                if (set.Contains(entity))
                    set.Remove(entity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(int x, int y, out HashSet<Collider> set)
        {
            return _cells.TryGetValue(GetKey(x, y), out set);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetAllObjects(ISet<Collider> results)
        {
            foreach (var set in _cells.Values)
                results.UnionWith(set);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _cells.Clear();
        }
    }
}
