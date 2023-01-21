using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.MathHelpers
{
    [Flags]
    public enum Directions
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8,
        NorthEast = North | East,
        SouthEast = South | East,
        SouthWest = South | West,
        NorthWest = North | West
    }

    public static class DirectionsUtils
    {
        public static int ToDegrees(this Directions directions)
        {
            switch(directions)
            {
                case Directions.North: return 90;
                case Directions.East: return 0;
                case Directions.South: return 270;
                case Directions.West: return 180;
                case Directions.NorthEast: return 45;
                case Directions.SouthEast: return 315;
                case Directions.SouthWest: return 225;
                case Directions.NorthWest: return 135;
                default: return 0;
            }
        }

        public static float ToRadians(this Directions direction)
        {
            return MathExt.Deg2Rad * ToDegrees(direction);
        }
    }
}
