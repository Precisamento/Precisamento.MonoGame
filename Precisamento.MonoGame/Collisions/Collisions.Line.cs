using Microsoft.Xna.Framework;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public static partial class Collisions
    {
        public static bool LineToPoly(LineCollider line, PolygonCollider polygon)
            => LineToPoly(line.Start, line.End, polygon);

        public static bool LineToPoly(Vector2 start, Vector2 end, PolygonCollider polygon)
        {
            for (int j = polygon.Points.Length - 1, i = 0; i < polygon.Points.Length; j = i, i++)
            {
                var edge1 = polygon.Position + polygon.Points[j];
                var edge2 = polygon.Position + polygon.Points[i];
                if (LineToLine(edge1, edge2, start, end))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool LineToPoly(LineCollider line, PolygonCollider polygon, out RaycastHit hit)
            => LineToPoly(line.Start, line.End, polygon, out hit);

        public static bool LineToPoly(Vector2 start, Vector2 end, PolygonCollider polygon, out RaycastHit hit)
        {
            hit = new RaycastHit();
            var normal = Vector2.Zero;
            var intersectionPoint = Vector2.Zero;
            var fraction = float.MaxValue;
            var hasIntersection = false;

            for (int j = polygon.Points.Length - 1, i = 0; i < polygon.Points.Length; j = i, i++)
            {
                var edge1 = polygon.Position + polygon.Points[j];
                var edge2 = polygon.Position + polygon.Points[i];
                if (LineToLine(edge1, edge2, start, end, out Vector2 intersection))
                {
                    hasIntersection = true;

                    // TODO: is this the correct and most efficient way to get the fraction?
                    // check x fraction first. if it is NaN use y instead
                    var distanceFraction = (intersection.X - start.X) / (end.X - start.X);
                    if (float.IsNaN(distanceFraction))
                        distanceFraction = (intersection.Y - start.Y) / (end.Y - start.Y);

                    if (distanceFraction < fraction)
                    {
                        var edge = edge2 - edge1;
                        normal = new Vector2(edge.Y, -edge.X);
                        fraction = distanceFraction;
                        intersectionPoint = intersection;
                    }
                }
            }

            if (hasIntersection)
            {
                normal.Normalize();
                Vector2.Distance(ref start, ref intersectionPoint, out float distance);
                hit.SetValues(fraction, distance, intersectionPoint, normal);
                return true;
            }

            return false;
        }

        public static bool LineToCircle(LineCollider line, CircleCollider circle)
            => LineToCircle(line.Start, line.End, circle);

        public static bool LineToCircle(Vector2 start, Vector2 end, CircleCollider circle)
        {
            // calculate the length here and normalize d separately since we will need it to get the fraction if we have a hit
            var lineLength = Vector2.Distance(start, end);
            var d = (end - start) / lineLength;
            var m = start - circle.Position;
            var b = Vector2.Dot(m, d);
            var c = Vector2.Dot(m, m) - circle.Radius * circle.Radius;

            // exit if r's origin outside of s (c > 0) and r pointing away from s (b > 0)
            if (c > 0f && b > 0f)
                return false;

            var discr = b * b - c;

            // a negative descriminant means the line misses the circle
            if (discr < 0)
                return false;

            return true;
        }

        public static bool LineToCircle(LineCollider line, CircleCollider circle, out RaycastHit hit)
            => LineToCircle(line.Start, line.End, circle, out hit);

        public static bool LineToCircle(Vector2 start, Vector2 end, CircleCollider circle, out RaycastHit hit)
        {
            hit = new RaycastHit();

            // calculate the length here and normalize d separately since we will need it to get the fraction if we have a hit
            var lineLength = Vector2.Distance(start, end);
            var d = (end - start) / lineLength;
            var m = start - circle.Position;
            var b = Vector2.Dot(m, d);
            var c = Vector2.Dot(m, m) - circle.Radius * circle.Radius;

            // exit if r's origin outside of s (c > 0) and r pointing away from s (b > 0)
            if (c > 0f && b > 0f)
                return false;

            var discr = b * b - c;

            // a negative descriminant means the line misses the circle
            if (discr < 0)
                return false;

            // ray intersects circle. calculate details now.
            hit.Fraction = -b - MathF.Sqrt(discr);

            // if fraction is negative, ray started inside circle so clamp fraction to 0
            if (hit.Fraction < 0)
                hit.Fraction = 0;

            hit.Point = start + hit.Fraction * d;
            Vector2.Distance(ref start, ref hit.Point, out hit.Distance);
            hit.Normal = Vector2.Normalize(hit.Point - circle.Position);
            hit.Fraction = hit.Distance / lineLength;

            return true;
        }

        public static bool LineToLine(LineCollider line, Vector2 b1, Vector2 b2)
            => LineToLine(line.AdjustedStart, line.AdjustedEnd, b1, b2);

        public static bool LineToLine(Vector2 a1, Vector2 a2, LineCollider line)
            => LineToLine(a1, a2, line.AdjustedStart, line.AdjustedEnd);

        public static bool LineToLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            var b = a2 - a1;
            var d = b2 - b1;
            var bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            var c = b1 - a1;
            var t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            var u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            return true;
        }

        public static bool LineToLine(LineCollider line, Vector2 b1, Vector2 b2, out Vector2 intersection)
            => LineToLine(line.AdjustedStart, line.AdjustedEnd, b1, b2, out intersection);

        public static bool LineToLine(Vector2 a1, Vector2 a2, LineCollider line, out Vector2 intersection)
            => LineToLine(a1, a2, line.AdjustedStart, line.AdjustedEnd, out intersection);

        public static bool LineToLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
        {
            intersection = Vector2.Zero;

            var b = a2 - a1;
            var d = b2 - b1;
            var bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            var c = b1 - a1;
            var t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            var u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            intersection = a1 + t * b;

            return true;
        }

        // Todo: Should the collision result be a raycast hit instead?

        public static bool LineToLine(LineCollider line, Vector2 b1, Vector2 b2, out CollisionResult result)
            => LineToLine(line.AdjustedStart, line.AdjustedEnd, b1, b2, out result);

        public static bool LineToLine(Vector2 a1, Vector2 a2, LineCollider line, out CollisionResult result)
            => LineToLine(a1, a2, line.AdjustedStart, line.AdjustedEnd, out result);

        public static bool LineToLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out CollisionResult result)
        {
            if (!LineToLine(a1, a2, b1, b2, out Vector2 intersection))
            {
                result = default;
                return false;
            }

            Vector2 minVector = a1;
            Vector2.DistanceSquared(ref intersection, ref a1, out var minDistance);

            Vector2.DistanceSquared(ref intersection, ref a2, out var nextDistance);
            if(nextDistance < minDistance)
            {
                minDistance = nextDistance;
                minVector = a2;
            }

            Vector2.DistanceSquared(ref intersection, ref b1, out nextDistance);
            if (nextDistance < minDistance)
            {
                minDistance = nextDistance;
                minVector = b1;
            }

            Vector2.DistanceSquared(ref intersection, ref b2, out nextDistance);
            if (nextDistance < minDistance)
            {
                minDistance = nextDistance;
                minVector = b2;
            }

            Vector2 mtv = minVector - intersection;
            result = new CollisionResult()
            {
                MinimumTranslationVector = mtv,
                Point = intersection
            };
            Vector2.Normalize(ref mtv, out result.Normal);

            return true;
        }
    }
}
