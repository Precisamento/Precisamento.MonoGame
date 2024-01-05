﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public static class LayoutUtils
    {
        public static Rectangle Align(Point containerSize, Point controlSize, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            var result = new Rectangle(Point.Zero, controlSize);

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    result.X = (containerSize.X - controlSize.X) / 2;
                    break;
                case HorizontalAlignment.Right:
                    result.X = containerSize.X - controlSize.X;
                    break;
                case HorizontalAlignment.Stretch:
                    result.Width = containerSize.X;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Center:
                    result.Y = (containerSize.Y - controlSize.Y) / 2;
                    break;
                case VerticalAlignment.Bottom:
                    result.Y = containerSize.Y - controlSize.Y;
                    break;
                case VerticalAlignment.Stretch:
                    result.Height = containerSize.Y;
                    break;
            }

            return result;
        }
    }
}
