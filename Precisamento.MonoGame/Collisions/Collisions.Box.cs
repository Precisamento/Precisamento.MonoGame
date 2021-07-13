using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Precisamento.MonoGame.Collisions
{
    public static partial class Collisions
    {
        public static bool BoxToBox(BoxCollider first, BoxCollider second, out CollisionResult result)
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
        private static RectangleF MinkowskiDifference(BoxCollider first, BoxCollider second)
        {
            var topLeft = first.BoundingBox.Position - second.BoundingBox.BottomRight;
            var fullSize = first.BoundingBox.Size + second.BoundingBox.Size;

            return new RectangleF(topLeft, fullSize);
        }
    }
}
