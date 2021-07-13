using Microsoft.Xna.Framework;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public static partial class Collisions
    {
        public static bool PointToCircle(Vector2 point, CircleCollider circle)
        {
            var distanceSquared = Vector2.DistanceSquared(point, circle.Position);
            var sumOfRadii = 1 + circle.Radius;
            return distanceSquared < sumOfRadii * sumOfRadii;
        }

        public static bool PointToCircle(Vector2 point, CircleCollider circle, out CollisionResult result)
        {
            result = new CollisionResult();

            // avoid the square root until we actually need it
            var distanceSquared = Vector2.DistanceSquared(point, circle.Position);
            var sumOfRadii = 1 + circle.Radius;
            var collided = distanceSquared < sumOfRadii * sumOfRadii;
            if (collided)
            {
                result.Normal = Vector2.Normalize(point - circle.Position);
                var depth = sumOfRadii - MathF.Sqrt(distanceSquared);
                result.MinimumTranslationVector = -depth * result.Normal;
                result.Point = circle.Position + result.Normal * circle.Radius;

                return true;
            }

            return false;
        }

        public static bool PointToBox(Vector2 point, BoxCollider box, out CollisionResult result)
        {
            result = new CollisionResult();

            if (box.ContainsPoint(point))
            {
                result.Point = box.BoundingBox.GetClosestPointOnBorderToPoint(point, out result.Normal);
                result.MinimumTranslationVector = point - result.Point;

                return true;
            }

            return false;
        }

        public static bool PointToPoly(Vector2 point, PolygonCollider poly)
        {
            return poly.ContainsPoint(point);
        }

        public static bool PointToPoly(Vector2 point, PolygonCollider poly, out CollisionResult result)
        {
            result = new CollisionResult();

            if (poly.ContainsPoint(point))
            {
                var closestPoint = PolygonCollider.GetClosestPointOnPolygonToPoint(poly.Points, point - poly.Position, out float distanceSquared, out result.Normal);

                result.MinimumTranslationVector = result.Normal * MathF.Sqrt(distanceSquared);
                result.Point = closestPoint + poly.Position;

                return true;
            }

            return false;
        }

        public static bool PointToLine(PointCollider point, LineCollider line)
            => PointToLine(point.Position, line.Start + line.Position, line.End + line.Position);

        public static bool PointToLine(Vector2 point, LineCollider line)
            => PointToLine(point, line.Start + line.Position, line.End + line.Position);

        public static bool PointToLine(PointCollider point, Vector2 lineStart, Vector2 lineEnd)
            => PointToLine(point.Position, lineStart, lineEnd);

        public static bool PointToLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd) 
            => Vector2.Distance(lineStart, point) + Vector2.Distance(point, lineEnd) == Vector2.Distance(lineStart, lineEnd);
    }
}
