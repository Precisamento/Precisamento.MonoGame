using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.MathHelpers
{
    public static class RectFExt
    {
        public static Vector2 GetClosestPointOnBoundsToOrigin(this RectangleF rect)
        {
            var max = rect.BottomRight;
            var minDist = Math.Abs(rect.Position.X);
            var boundsPoint = new Vector2(rect.Position.X, 0);

            if (Math.Abs(max.X) < minDist)
            {
                minDist = Math.Abs(max.X);
                boundsPoint.X = max.X;
                boundsPoint.Y = 0f;
            }

            if (Math.Abs(max.Y) < minDist)
            {
                minDist = Math.Abs(max.Y);
                boundsPoint.X = 0f;
                boundsPoint.Y = max.Y;
            }

            if (Math.Abs(rect.Position.Y) < minDist)
            {
                minDist = Math.Abs(rect.Position.Y);
                boundsPoint.X = 0;
                boundsPoint.Y = rect.Position.Y;
            }

            return boundsPoint;
        }

        public static Vector2 GetClosestPointOnRectToPoint(this RectangleF rect, Vector2 point)
        {
            var result = new Vector2()
            {
                X = MathHelper.Clamp(point.X, rect.Left, rect.Right),
                Y = MathHelper.Clamp(point.Y, rect.Top, rect.Bottom)
            };
            return result;
        }

        public static Vector2 GetClosestPointOnBorderToPoint(this RectangleF rect, Vector2 point)
        {
            var result = new Vector2()
            {
                X = MathHelper.Clamp(point.X, rect.Left, rect.Right),
                Y = MathHelper.Clamp(point.Y, rect.Top, rect.Bottom)
            };

            if (rect.Contains(result))
            {
                var dl = result.X - rect.Left;
                var dr = rect.Right - result.X;
                var dt = result.Y - rect.Top;
                var db = rect.Bottom - result.Y;

                var min = MathF.MinOf(dl, dr, dt, db);
                if (min == dt)
                    result.Y = rect.Top;
                else if (min == db)
                    result.Y = rect.Bottom;
                else if (min == dl)
                    result.X = rect.Left;
                else
                    result.X = rect.Right;
            }

            return result;
        }

        public static Vector2 GetClosestPointOnBorderToPoint(this RectangleF rect, Vector2 point, out Vector2 edgeNormal)
        {
            edgeNormal = Vector2.Zero;

            var result = new Vector2()
            {
                X = MathHelper.Clamp(point.X, rect.Left, rect.Right),
                Y = MathHelper.Clamp(point.Y, rect.Top, rect.Bottom)
            };

            if (rect.Contains(result))
            {
                var dl = result.X - rect.Left;
                var dr = rect.Right - result.X;
                var dt = result.Y - rect.Top;
                var db = rect.Bottom - result.Y;

                var min = MathF.MinOf(dl, dr, dt, db);
                if (min == dt)
                {
                    result.Y = rect.Top;
                    edgeNormal.Y = -1;
                }
                else if (min == db)
                {
                    result.Y = rect.Bottom;
                    edgeNormal.Y = 1;
                }
                else if (min == dl)
                {

                    result.X = rect.Left;
                    edgeNormal.X = -1;
                }
                else
                {
                    result.X = rect.Right;
                    edgeNormal.X = 1;
                }
            }
            else
            {
                if (result.X == rect.Left)
                    edgeNormal.X = -1;
                if (result.X == rect.Right)
                    edgeNormal.X = 1;
                if (result.Y == rect.Top)
                    edgeNormal.Y = -1;
                if (result.Y == rect.Bottom)
                    edgeNormal.Y = 1;
            }

            return result;
        }
    }
}
