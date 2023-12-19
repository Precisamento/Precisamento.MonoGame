using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public static partial class CollisionChecks
    {
        public static bool PolygonToPolygon(PolygonCollider first, PolygonCollider second)
        {
            var isIntersecting = true;
            var firstEdges = first.EdgeNormals;
            var secondEdges = second.EdgeNormals;
            var polygonOffset = first.Position - first.Center - second.Position - second.Center;
            Vector2 axis;

            // Loop through all the edges of both polygons
            for (var edgeIndex = 0; edgeIndex < firstEdges.Length + secondEdges.Length; edgeIndex++)
            {
                // 1. Find if the polygons are currently intersecting
                // Polygons have the normalized axis perpendicular to the current edge cached for us
                if (edgeIndex < firstEdges.Length)
                    axis = firstEdges[edgeIndex];
                else
                    axis = secondEdges[edgeIndex - firstEdges.Length];

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                GetInterval(axis, first, ref minA, ref maxA);
                GetInterval(axis, second, ref minB, ref maxB);

                // get our interval to be space of the second Polygon. Offset by the difference in Position projected on the axis.
                Vector2.Dot(ref polygonOffset, ref axis, out float relativeIntervalOffset);
                minA += relativeIntervalOffset;
                maxA += relativeIntervalOffset;

                // check if the polygon projections are currentlty intersecting
                float intervalDist = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDist > 0)
                    isIntersecting = false;


                // If the polygons are not intersecting and won't intersect, exit the loop
                if (!isIntersecting)
                    return false;

                // If the intervalDist == 0, the two objects are just touching each other, not colliding.
                // Todo: fully test.
                if (intervalDist == 0)
                    return false;
            }

            return true;
        }

        public static bool PolygonToPolygon(PolygonCollider first, PolygonCollider second, out CollisionResult result)
        {
            result = new CollisionResult();
            var isIntersecting = true;

            var firstEdges = first.EdgeNormals;
            var secondEdges = second.EdgeNormals;
            var minIntervalDistance = float.PositiveInfinity;
            var translationAxis = new Vector2();
            var polygonOffset = first.Position - first.Center - second.Position - second.Center;
            Vector2 axis;

            // Loop through all the edges of both polygons
            for (var edgeIndex = 0; edgeIndex < firstEdges.Length + secondEdges.Length; edgeIndex++)
            {
                // 1. Find if the polygons are currently intersecting
                // Polygons have the normalized axis perpendicular to the current edge cached for us
                if (edgeIndex < firstEdges.Length)
                    axis = firstEdges[edgeIndex];
                else
                    axis = secondEdges[edgeIndex - firstEdges.Length];

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                GetInterval(axis, first, ref minA, ref maxA);
                GetInterval(axis, second, ref minB, ref maxB);

                // get our interval to be space of the second Polygon. Offset by the difference in Position projected on the axis.
                Vector2.Dot(ref polygonOffset, ref axis, out float relativeIntervalOffset);
                minA += relativeIntervalOffset;
                maxA += relativeIntervalOffset;

                // check if the polygon projections are currentlty intersecting
                float intervalDist = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDist > 0)
                    isIntersecting = false;


                // If the polygons are not intersecting and won't intersect, exit the loop
                if (!isIntersecting)
                    return false;

                // Check if the current interval distance is the minimum one. If so store the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDist = System.Math.Abs(intervalDist);
                if (intervalDist < minIntervalDistance)
                {
                    // If the intervalDist == 0, the two objects are just touching each other, not colliding.
                    //Todo: Fully test.

                    if (intervalDist == 0)
                        return false;

                    minIntervalDistance = intervalDist;
                    translationAxis = axis;

                    if (Vector2.Dot(translationAxis, polygonOffset) < 0)
                        translationAxis = -translationAxis;
                }
            }

            // The minimum translation vector can be used to push the polygons appart.
            result.Normal = translationAxis;
            result.MinimumTranslationVector = -translationAxis * minIntervalDistance;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
                return minB - maxA;
            return minA - maxB;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void GetInterval(Vector2 axis, PolygonCollider polygon, ref float min, ref float max)
        {
            // To project a point on an axis use the dot product
            Vector2.Dot(ref polygon.Points[0], ref axis, out float dot);
            min = max = dot;

            for (var i = 1; i < polygon.Points.Length; i++)
            {
                Vector2.Dot(ref polygon.Points[i], ref axis, out dot);
                if (dot < min)
                    min = dot;
                else if (dot > max)
                    max = dot;
            }
        }
    }
}
