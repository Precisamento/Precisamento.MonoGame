using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public static partial class CollisionChecks
    {
        public static bool BoxToBox(BoxCollider first, BoxCollider second, out CollisionResult result)
            => BoxToBox(first.BoundingBox, second.BoundingBox, out result);

        public static bool BoxToBox(BoxCollider first, RectangleF second, out CollisionResult result)
            => BoxToBox(first.BoundingBox, second, out result);

        public static bool BoxToBox(RectangleF first, RectangleF second, out CollisionResult result)
        {
            result = new CollisionResult();

            var diff = MinkowskiDifference(first, second);
            if (diff.Contains(new Point2(0f, 0f)))
            {
                result.MinimumTranslationVector = diff.GetClosestPointOnBoundsToOrigin();

                if (result.MinimumTranslationVector == Vector2.Zero)
                    return false;

                result.Normal = -result.MinimumTranslationVector;
                result.Normal.Normalize();

                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static RectangleF MinkowskiDifference(RectangleF first, RectangleF second)
        {
            var topLeft = first.Position - second.BottomRight;
            var fullSize = first.Size + second.Size;

            return new RectangleF(topLeft, fullSize);
        }
    }
}
