using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public partial class Control
    {
        private void WindowFindFocusNeighbor(Vector2 dir, Control? control, Vector2[] points, float min, ref float closest_dist, ref Control? closest)
        {
            if (control is null)
            {
                closest = null;
                return;
            }

            if (control.GetFocusMode() == FocusMode.All && control.VisibleInTree())
            {
                var rect = control.GetRect();

                var otherPoints = new Vector2[]
                {
                    rect.Location.ToVector2(),
                    new Vector2(rect.X + rect.Width),
                    (rect.Location + rect.Size).ToVector2(),
                    new Vector2(rect.X, rect.Y + rect.Height)
                };

                var otherMin = 1e7;

                for (var i = 0; i < 4; i++)
                {
                    var d = dir.Dot(otherPoints[i]);
                    if (d < otherMin)
                        otherMin = d;
                }

                if (otherMin > (min - MathExt.Epsilon))
                {
                    for (var i = 0; i < 4; i++)
                    {
                        var la = points[i];
                        var lb = points[(i + 1) % 4];

                        for (var j = 0; j < 4; j++)
                        {
                            var fa = otherPoints[j];
                            var fb = otherPoints[(j + 1) % 4];
                            var d = Collisions.CollisionChecks.GetClosestPointsBetweenLines(la, lb, fa, fb, out _, out _);

                            if (d < closest_dist)
                            {
                                closest_dist = d;
                                closest = control;
                            }
                        }
                    }
                }
            }

            if (control is Container container)
            {
                foreach (var child in container.Children)
                {
                    WindowFindFocusNeighbor(dir, child, points, min, ref closest_dist, ref closest);
                }
            }
        }

        public void SetFocusMode(FocusMode focusMode)
        {
            GuiSystem?.ThreadGuard();

            if (InTree && focusMode == FocusMode.None && _focusMode != FocusMode.None && HasFocus())
            {
                ReleaseFocus();
            }

            _focusMode = focusMode;
        }

        public FocusMode GetFocusMode()
        {
            return _focusMode;
        }

        public bool HasFocus()
        {
            return GuiSystem?.GetFocusedControl() == this;
        }

        public void GrabFocus()
        {
            GuiSystem?.ThreadGuard();

            if (_focusMode == FocusMode.None)
            {
                return;
            }

            GuiSystem?.GrabFocus(this);
        }

        public void GrabClickFocus()
        {
            GuiSystem?.ThreadGuard();

            GuiSystem?.GrabClickFocus(this);
        }

        public void ReleaseFocus()
        {
            GuiSystem?.ThreadGuard();

            if (!HasFocus())
                return;

            GuiSystem?.ReleaseFocus();
        }

        private static Control? NextControl(Control from)
        {
            if (from._parent is null)
                return null;

            var parent = from._parent;
            var next = parent.Children.IndexOf(from);

            if (next == -1)
                throw new InvalidOperationException();

            for (var i = next + 1; i < parent.Children.Count; i++)
            {
                var control = parent.Children[i];
                if (!control.VisibleInTree() || control.IsTopLevelControl())
                {
                    continue;
                }

                return control;
            }

            // No next parent, try the same in parent.

            return NextControl(parent);
        }

        public Control? FindNextValidFocus()
        {
            var control = this;

            while (true)
            {
                if (control.FocusNext != null)
                {
                    if (control.FocusNext.VisibleInTree() && control.FocusNext.GetFocusMode() != FocusMode.None)
                    {
                        return control.FocusNext;
                    }
                }

                Control? nextChild = null;
                if (control is Container container)
                {
                    foreach (var child in container.Children)
                    {
                        if (!child.VisibleInTree() || child.IsTopLevelControl())
                        {
                            continue;
                        }

                        nextChild = child;
                    }
                }

                if (nextChild is null)
                {
                    nextChild = NextControl(control);
                }

                if (nextChild is null)
                {
                    while (nextChild != null && !nextChild.IsTopLevelControl())
                    {
                        nextChild = nextChild._parent;
                    }
                }

                if (nextChild == control || nextChild == this)
                {
                    return GetFocusMode() == FocusMode.All ? nextChild : null;
                }

                if (nextChild != null)
                {
                    if (nextChild.GetFocusMode() == FocusMode.All)
                    {
                        return nextChild;
                    }
                    control = nextChild;
                }
                else
                {
                    break;
                }
            }

            return null;
        }

        private static Control PrevControl(Control from)
        {
            Control? child = null;
            if (from is Container container)
            {
                for (var i = container.Children.Count - 1; i >= 0; i--)
                {
                    var control = container.Children[i];

                    if (!container.VisibleInTree() || control.IsTopLevelControl())
                    {
                        continue;
                    }

                    child = control;
                    break;
                }
            }

            if (child is null)
                return from;


            // No prev in parent, try the same in parent.
            return PrevControl(child);
        }

        public Control? FindPreviousValidFocus()
        {
            var control = this;
            while (true)
            {
                if (control.FocusPrevious != null)
                {
                    if (control.FocusPrevious.VisibleInTree() && control.FocusPrevious.GetFocusMode() != FocusMode.None)
                    {
                        return control.FocusPrevious;
                    }
                }

                Control? prevChild = null;

                if (control.IsTopLevelControl())
                {
                    prevChild = PrevControl(control);
                }
                else
                {
                    var index = _parent!.Children.IndexOf(this);

                    for (var i = index - 1; i >= 0; i--)
                    {
                        var child = _parent.Children[i];
                        if (!child.VisibleInTree() || child.IsTopLevelControl())
                        {
                            continue;
                        }

                        prevChild = child;
                        break;
                    }

                    if (prevChild is null)
                    {
                        prevChild = _parent;
                    }
                    else
                    {
                        prevChild = PrevControl(prevChild);
                    }
                }

                if (prevChild == control || prevChild == this)
                {
                    return GetFocusMode() == FocusMode.All ? prevChild : null;
                }

                if (prevChild != null)
                {
                    if (prevChild.GetFocusMode() == FocusMode.All)
                    {
                        return prevChild;
                    }

                    control = prevChild;
                }
                else
                {
                    break;
                }
            }

            return null;
        }

        private const int MAX_NEIGHBOR_SEARCH_COUNT = 512;

        private Control? FindFocusNeighbor(Side side, int count = 0)
        {
            if (count >= MAX_NEIGHBOR_SEARCH_COUNT)
                return null;

            var neighbor = GetFocusNeighbor(side);

            if (neighbor != null)
            {
                if (!neighbor.VisibleInTree() || neighbor.GetFocusMode() == FocusMode.None)
                {
                    return neighbor.FindFocusNeighbor(side, count + 1);
                }

                return neighbor;
            }

            float distance = 1e7f;
            Control? result = null;

            var rect = GetRect();
            var points = new Vector2[]
            {
                rect.Location.ToVector2(),
                new Vector2(rect.X + rect.Size.X, rect.Y),
                (rect.Location + rect.Size).ToVector2(),
                new Vector2(rect.X, rect.Y + rect.Size.Y)
            };

            var dir = side switch
            {
                Side.Top => new Vector2(0, -1),
                Side.Left => new Vector2(-1, 0),
                Side.Bottom => new Vector2(0, 1),
                _ => new Vector2(1, 0)
            };

            float maxDistance = -1e7f;
            for (var i = 0; i < 4; i++)
            {
                var d = dir.Dot(points[i]);
                if (d > maxDistance)
                    maxDistance = d;
            }

            var control = this;
            while (control._parent != null)
            {
                control = control._parent;
            }

            WindowFindFocusNeighbor(dir, control, points, maxDistance, ref distance, ref result);

            return result;
        }

        public Control? FindValidFocusNeighbor(Side side)
        {
            return FindFocusNeighbor(side, 0);
        }

        public void SetFocusNeighbor(Side side, Control? control)
        {
            GuiSystem?.ThreadGuard();
            switch (side)
            {
                case Side.Top:
                    NeighborTop = control;
                    break;
                case Side.Left:
                    NeighborLeft = control;
                    break;
                case Side.Bottom:
                    NeighborBottom = control;
                    break;
                case Side.Right:
                    NeighborRight = control;
                    break;
            }
        }

        public Control? GetFocusNeighbor(Side side)
        {
            return side switch
            {
                Side.Top => NeighborTop,
                Side.Left => NeighborLeft,
                Side.Bottom => NeighborBottom,
                Side.Right => NeighborRight,
                _ => null
            };
        }

        public void SetFocusNext(Control? control)
        {
            GuiSystem?.ThreadGuard();

            FocusNext = control;
        }

        public Control? GetFocusNext()
        {
            return FocusNext;
        }

        public void SetFocusPrevious(Control? control)
        {
            GuiSystem?.ThreadGuard();

            FocusPrevious = control;
        }

        public Control? GetFocusPrevious()
        {
            return FocusPrevious;
        }
    }
}
