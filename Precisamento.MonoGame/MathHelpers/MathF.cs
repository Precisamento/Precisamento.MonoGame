using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Precisamento.MonoGame.MathHelpers
{
    public static class MathF
    {
        public const float Epsilon = 0.00001f;
        public const float Deg2Rad = 0.0174532924f;
        public const float Rad2Deg = 57.29578f;

        public static Vector2 LengthDir(float length, float direction)
        {
            return new Vector2((float)(length * Math.Cos(direction)), (float)(-length * Math.Sin(direction)));
        }

        public static float LengthDirX(float length, float direction)
        {
            return (float)(length * Math.Cos(MathHelper.ToRadians(direction)));
        }

        public static float LengthDirY(float length, float direction)
        {
            return (float)(-length * Math.Sin(MathHelper.ToRadians(direction)));
        }

        public static float DistanceSquared(Vector2 p1, Vector2 p2)
        {
            return ((p2.X - p1.X) * (p2.X - p1.X)) + ((p2.Y - p1.Y) * (p2.Y - p1.Y));
        }

        public static float DistanceSquared(float x1, float y1, float x2, float y2)
        {
            return ((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1));
        }

        public static float Direction(float x1, float y1, float x2, float y2)
        {
            var dir = MathHelper.ToDegrees((float)Math.Atan2(y1 - y2, x2 - x1));
            if (dir < 0f)
                dir = 360f + dir;
            return dir;
        }

        public static float Direction(Vector2 p1, Vector2 p2)
        {
            var dir = MathHelper.ToDegrees((float)Math.Atan2(p1.Y - p2.Y, p2.X - p1.X));
            if (dir < 0f)
                dir = 360f + dir;
            return dir;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(float f)
        {
            return (float)Math.Floor(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(float value, float n)
        {
            return (float)Math.Floor(value / n) * n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(float f)
        {
            return (int)Math.Floor(f);
        }

        /// <summary>
        /// floors the float to the nearest int value below x. note that this only works for values in the range of short
        /// </summary>
        /// <returns>The floor to int.</returns>
        /// <param name="x">The x coordinate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastFloorToInt(float x)
        {
            // we shift to guaranteed positive before casting then shift back after
            return (int)(x + 32768f) - 32768;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceil(float f)
        {
            return (float)Math.Ceiling(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceil(float value, float n)
        {
            return (float)Math.Ceiling(value / n) * n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(float f)
        {
            return (int)Math.Ceiling(f);
        }

        /// <summary>
        /// ceils the float to the nearest int value above y. note that this only works for values in the range of short
        /// </summary>
        /// <returns>The ceil to int.</returns>
        /// <param name="y">F.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastCeilToInt(float y)
        {
            return 32768 - (int)(32768f - y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float f)
        {
            return (float)Math.Round(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float value, float n)
        {
            return (float)Math.Round(value / n) * n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundToInt(float f)
        {
            return (int)Math.Round(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;

            return value;
        }

        /// <summary>
        /// clamps value between 0 and 1
        /// </summary>
        /// <param name="value">Value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value)
        {
            if (value < 0f)
                return 0f;

            if (value > 1f)
                return 1f;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float from, float to, float t)
        {
            return from + (to - from) * MathF.Clamp01(t);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerp(float from, float to, float t)
        {
            if (from < to)
            {
                if (t < from)
                    return 0.0f;
                else if (t > to)
                    return 1.0f;
            }
            else
            {
                if (t < to)
                    return 1.0f;
                else if (t > from)
                    return 0.0f;
            }

            return (t - from) / (to - from);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float UnclampedLerp(float from, float to, float t)
        {
            return from + (to - from) * t;
        }

        /// <summary>
        /// moves start towards end by shift amount clamping the result. start can be less than or greater than end.
        /// example: start is 2, end is 10, shift is 4 results in 6
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="shift">Shift.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Approach(float start, float end, float shift)
        {
            if (start < end)
                return Math.Min(start + shift, end);
            return Math.Max(start - shift, end);
        }

        /// <summary>
        /// returns the minimum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinOf(float a, float b, float c)
        {
            return Math.Min(a, Math.Min(b, c));
        }


        /// <summary>
        /// returns the maximum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxOf(float a, float b, float c)
        {
            return Math.Max(a, Math.Max(b, c));
        }


        /// <summary>
        /// returns the minimum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        /// <param name="d">D.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinOf(float a, float b, float c, float d)
        {
            return Math.Min(a, Math.Min(b, Math.Min(c, d)));
        }


        /// <summary>
        /// returns the minimum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        /// <param name="d">D.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinOf(float a, float b, float c, float d, float e)
        {
            return Math.Min(a, Math.Min(b, Math.Min(c, Math.Min(d, e))));
        }


        /// <summary>
        /// returns the maximum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        /// <param name="d">D.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxOf(float a, float b, float c, float d)
        {
            return Math.Max(a, Math.Max(b, Math.Max(c, d)));
        }


        /// <summary>
        /// returns the maximum of the passed in values
        /// </summary>
        /// <returns>The of.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        /// <param name="d">D.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxOf(float a, float b, float c, float d, float e)
        {
            return Math.Max(a, Math.Max(b, Math.Max(c, Math.Max(d, e))));
        }

        /// <summary>
        /// Returns the square root
        /// </summary>
        /// <param name="val">Value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float val)
        {
            return (float)Math.Sqrt(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }

        /// <summary>
        /// Returns the sine of angle in radians
        /// </summary>
        /// <param name="f">F.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float f)
        {
            return (float)Math.Sin(f);
        }

        /// <summary>
        /// Returns the cosine of angle in radians
        /// </summary>
        /// <param name="f">F.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float f)
        {
            return (float)Math.Cos(f);
        }

        /// <summary>
        /// Returns the arc-cosine of f: the angle in radians whose cosine is f
        /// </summary>
        /// <param name="f">F.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Acos(float f)
        {
            return (float)Math.Acos(f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Exp(float power)
        {
            return (float)Math.Exp(power);
        }

        /// <summary>
        /// returns the angle whose tangent is the quotient (y/x) of two specified numbers
        /// </summary>
        /// <param name="y">The y coordinate.</param>
        /// <param name="x">The x coordinate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }
    }
}
