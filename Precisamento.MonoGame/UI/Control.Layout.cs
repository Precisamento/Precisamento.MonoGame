using Microsoft.Xna.Framework;
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
        protected virtual void Resized()
        {
        }

        protected virtual void LayoutDirectionChanged()
        {
        }

        protected virtual void InvalidateLayout()
        {
            _layoutDirty = true;

            if (_parent != null)
            {
                _parent.InvalidateLayout();
            }
            else
            {
                GuiSystem?.InvalidateLayout();
            }
        }

        public void Arrange()
        {
            if (!_layoutDirty)
                return;

            _layoutDirty = true;
            UpdateArrange();
        }

        public void UpdateArrange()
        {
            if (!_layoutDirty)
                return;

            SizeChanged();
            UpdateMinimumSizeCache();

            _layoutDirty = false;
        }

        private Matrix GetInternalTransform()
        {
            return
                Matrix.CreateTranslation(new Vector3(-_pivotOffset, 0f))
                * Matrix.CreateRotationZ(_rotation)
                * Matrix.CreateScale(new Vector3(_scale, 1f))
                * Matrix.CreateTranslation(new Vector3(_pivotOffset, 0f));

        }

        private Margin ComputeOffsets(Rectangle rect, Anchor anchor)
        {
            var parentRectSize = GetParentAnchorableRect().Size;

            float x = rect.X;
            if (IsRtl())
            {
                x = parentRectSize.X - x - rect.Width;
            }

            var margin = new Margin(
                x - (anchor.Left * parentRectSize.X),
                rect.Y - (anchor.Top * parentRectSize.Y),
                x + rect.Width - (anchor.Right * parentRectSize.X),
                rect.Y + rect.Height - (anchor.Bottom * parentRectSize.Y));

            return margin;
        }

        private Anchor ComputeAnchors(Rectangle rect, Margin margin)
        {
            var parentRectSize = GetParentAnchorableRect().Size;
            if (parentRectSize.X == 0 || parentRectSize.Y == 0)
                throw new InvalidOperationException();

            float x = rect.X;
            if (IsRtl())
            {
                x = parentRectSize.X - x - rect.Width;
            }

            var anchor = new Anchor(
                (x - margin.Left) / parentRectSize.X,
                (rect.Y - margin.Top) / parentRectSize.Y,
                (x + rect.Width - margin.Right) / parentRectSize.X,
                (rect.Y + rect.Height - margin.Bottom) / parentRectSize.Y);

            return anchor;
        }

        private void SetLayoutMode(LayoutMode layoutMode)
        {
            var listChanged = false;

            if (_storedLayoutMode != layoutMode)
            {
                listChanged = true;
                _storedLayoutMode = layoutMode;
            }

            if (_storedLayoutMode == LayoutMode.Position)
            {
                _storedUseCustomAnchors = false;
                SetAnchorsAndOffsetsPreset(LayoutPreset.TopLeft, LayoutPresetMode.KeepSize);
                SetGrowDirectionPreset(LayoutPreset.TopLeft);
            }

            if (listChanged)
            {
                NotifyPropertyListChanged();
                InvalidateLayout();
            }
        }

        private void UpdateLayoutMode()
        {
            LayoutMode computedLayout = GetLayoutMode();
            if (_storedLayoutMode != computedLayout)
            {
                _storedLayoutMode = computedLayout;
                NotifyPropertyListChanged();
                InvalidateLayout();
            }
        }

        private LayoutMode GetLayoutMode()
        {
            var parent = GetParentControl();

            if (parent is null)
            {
                return LayoutMode.Uncontrolled;
            }
            else if (parent is Container)
            {
                return LayoutMode.Container;
            }

            if (GetAnchorsLayoutPreset() != LayoutPreset.TopLeft)
            {
                return LayoutMode.Anchors;
            }

            return _storedLayoutMode;
        }

        private LayoutMode GetDefaultLayoutMode()
        {
            var parent = GetParentControl();

            if (parent is null)
            {
                return LayoutMode.Uncontrolled;
            }
            else if (parent is Container)
            {
                return LayoutMode.Container;
            }

            return LayoutMode.Position;
        }

        private void SetAnchorsLayoutPreset(LayoutPreset preset)
        {
            if (_storedLayoutMode != LayoutMode.Uncontrolled && _storedLayoutMode != LayoutMode.Anchors)
            {
                // In other modes the anchor preset is non-operational and shouldn't be set to anything.
                return;
            }

            if (preset == LayoutPreset.CustomAnchors)
            {
                if (!_storedUseCustomAnchors)
                {
                    _storedUseCustomAnchors = true;
                    NotifyPropertyListChanged();
                }
                return;
            }

            var listChanged = false;

            if (_storedUseCustomAnchors)
            {
                listChanged = true;
                _storedUseCustomAnchors = false;
            }

            SetAnchorsPreset(preset);

            switch (preset)
            {
                case LayoutPreset.TopLeft:
                case LayoutPreset.TopRight:
                case LayoutPreset.BottomLeft:
                case LayoutPreset.BottomRight:
                case LayoutPreset.CenterLeft:
                case LayoutPreset.CenterRight:
                case LayoutPreset.CenterTop:
                case LayoutPreset.CenterBottom:
                case LayoutPreset.Center:
                    SetOffsetsPreset(preset, LayoutPresetMode.KeepSize);
                    break;
                case LayoutPreset.LeftWide:
                case LayoutPreset.RightWide:
                case LayoutPreset.TopWide:
                case LayoutPreset.BottomWide:
                case LayoutPreset.VerticalCenterWide:
                case LayoutPreset.HorizontalCenterWide:
                case LayoutPreset.FullRect:
                    SetOffsetsPreset(preset, LayoutPresetMode.MinSize);
                    break;
            }

            SetGrowDirectionPreset(preset);

            if (listChanged)
            {
                NotifyPropertyListChanged();
                InvalidateLayout();
            }
        }

        private LayoutPreset GetAnchorsLayoutPreset()
        {
            if (_storedLayoutMode != LayoutMode.Uncontrolled && _storedLayoutMode != LayoutMode.Anchors)
            {
                return LayoutPreset.TopLeft;
            }

            if (_storedUseCustomAnchors)
            {
                return LayoutPreset.CustomAnchors;
            }

            if (_anchor.Left == Anchor.Begin && _anchor.Right == Anchor.Begin && _anchor.Top == Anchor.Begin && _anchor.Bottom == Anchor.Begin)
            {
                return LayoutPreset.TopLeft;
            }

            if (_anchor.Left == Anchor.End && _anchor.Right == Anchor.End && _anchor.Top == Anchor.Begin && _anchor.Bottom == Anchor.Begin)
            {
                return LayoutPreset.TopRight;
            }

            if (_anchor.Left == Anchor.Begin && _anchor.Right == Anchor.Begin && _anchor.Top == Anchor.End && _anchor.Bottom == Anchor.End)
            {
                return LayoutPreset.BottomLeft;
            }

            if (_anchor.Left == Anchor.End && _anchor.Right == Anchor.End && _anchor.Top == Anchor.End && _anchor.Bottom == Anchor.End)
            {
                return LayoutPreset.BottomRight;
            }

            if (_anchor.Left == Anchor.Begin && _anchor.Right == Anchor.Begin && _anchor.Top == Anchor.Center && _anchor.Bottom == Anchor.Center)
            {
                return LayoutPreset.CenterLeft;
            }

            if (_anchor.Left == Anchor.End && _anchor.Right == Anchor.End && _anchor.Top == Anchor.Center && _anchor.Bottom == Anchor.Center)
            {
                return LayoutPreset.CenterRight;
            }

            if (_anchor.Left == Anchor.Center && _anchor.Right == Anchor.Center && _anchor.Top == Anchor.Begin && _anchor.Bottom == Anchor.Begin)
            {
                return LayoutPreset.CenterTop;
            }

            if (_anchor.Left == Anchor.Center && _anchor.Right == Anchor.Center && _anchor.Top == Anchor.End && _anchor.Bottom == Anchor.End)
            {
                return LayoutPreset.CenterBottom;
            }

            if (_anchor.Left == Anchor.Center && _anchor.Right == Anchor.Center && _anchor.Top == Anchor.Center && _anchor.Bottom == Anchor.Center)
            {
                return LayoutPreset.Center;
            }

            if (_anchor.Left == Anchor.Begin && _anchor.Right == Anchor.Begin && _anchor.Top == Anchor.Begin && _anchor.Bottom == Anchor.End)
            {
                return LayoutPreset.LeftWide;
            }

            if (_anchor.Left == Anchor.End && _anchor.Right == Anchor.End && _anchor.Top == Anchor.Begin && _anchor.Bottom == Anchor.End)
            {
                return LayoutPreset.RightWide;
            }

            if (_anchor.Left == Anchor.Begin && _anchor.Right == Anchor.End && _anchor.Top == Anchor.Begin && _anchor.Bottom == Anchor.Begin)
            {
                return LayoutPreset.TopWide;
            }

            if (_anchor.Left == Anchor.Begin && _anchor.Right == Anchor.End && _anchor.Top == Anchor.End && _anchor.Bottom == Anchor.End)
            {
                return LayoutPreset.BottomWide;
            }

            if (_anchor.Left == Anchor.Center && _anchor.Right == Anchor.Center && _anchor.Top == Anchor.Begin && _anchor.Bottom == Anchor.End)
            {
                return LayoutPreset.VerticalCenterWide;
            }

            if (_anchor.Left == Anchor.Begin && _anchor.Right == Anchor.End && _anchor.Top == Anchor.Center && _anchor.Bottom == Anchor.Center)
            {
                return LayoutPreset.HorizontalCenterWide;
            }

            if (_anchor.Left == Anchor.Begin && _anchor.Right == Anchor.End && _anchor.Top == Anchor.Begin && _anchor.Bottom == Anchor.End)
            {
                return LayoutPreset.FullRect;
            }

            // Does not match any preset, return "Custom"
            return LayoutPreset.CustomAnchors;
        }

        private void UpdateMinimumSizeCache()
        {
            var minSize = GetMinimumSize();
            minSize.X = Math.Max(minSize.X, _customMinimumSize.X);
            minSize.Y = Math.Max(minSize.Y, _customMinimumSize.Y);

            _minimumSizeCache = minSize;
            _minimumSizeValid = true;
        }

        private void SizeChanged()
        {
            var parentRect = GetParentAnchorableRect();

            var newPosCache = new Vector2(
                _margin.Left + (_anchor.Left * parentRect.Width),
                _margin.Top + (_anchor.Top * parentRect.Height));

            var newSizeCache = new Vector2(
                _margin.Right + (_anchor.Right * parentRect.Width),
                _margin.Bottom + (_anchor.Bottom * parentRect.Height));

            var minSize = GetCombinedMinimumSize();

            if (minSize.X > newSizeCache.X)
            {
                if (_growHorizontal == GrowDirection.Begin)
                {
                    newPosCache.X += newSizeCache.X - minSize.X;
                }
                else if (_growHorizontal == GrowDirection.Both)
                {
                    newPosCache.X += 0.5f * (newSizeCache.X - minSize.X);
                }

                newSizeCache.X = minSize.X;
            }

            if (IsRtl())
            {
                newPosCache.X = parentRect.Width + 2 * parentRect.X - newPosCache.X - newSizeCache.X;
            }

            if (minSize.Y > newSizeCache.Y)
            {
                if (_growVertical == GrowDirection.Begin)
                {
                    newPosCache.Y += newSizeCache.Y - minSize.Y;
                }
                else if (_growVertical == GrowDirection.Both)
                {
                    newPosCache.Y += 0.5f * (newSizeCache.Y - minSize.Y);
                }

                newSizeCache.Y = minSize.Y;
            }

            var posChanged = newPosCache.ToPoint() != _posCache;
            var sizeChanged = newSizeCache.ToPoint() != _sizeCache;

            _posCache = newPosCache.ToPoint();
            _sizeCache = newSizeCache.ToPoint();

            if (InTree)
            {
                if (posChanged || sizeChanged)
                {
                    // Todo: ItemRectChanged(sizeChanged);
                    NotifyTransform();
                    InvalidateLayout();
                }
            }
        }

        public Point GetParentAreaSize()
        {
            return GetParentAnchorableRect().Size;
        }

        public Rectangle GetParentAnchorableRect()
        {
            if (!InTree)
            {
                return Rectangle.Empty;
            }

            Rectangle parentRect;
            if (_parent != null)
            {
                parentRect = _parent.GetAnchorableRect();
            }
            else
            {
                parentRect = GuiSystem!.GetVisibleRect();
            }

            return parentRect;
        }

        public virtual Matrix GetTransform()
        {
            return GetInternalTransform() * Matrix.CreateTranslation(new Vector3(GetPosition().ToVector2(), 0f));
        }

        public void SetAnchor(Side side, float anchor, bool keepOffset = true, bool pushOppositeAnchor = true)
        {
            GuiSystem?.ThreadGuard();

            var parentRect = GetParentAnchorableRect();
            var parentRange = (side == Side.Left || side == Side.Right) ? parentRect.Width : parentRect.Height;

            float x, y, offsetX, offsetY;

            switch (side)
            {
                case Side.Top:
                    x = _anchor.Top;
                    y = _anchor.Bottom;
                    offsetX = _margin.Top;
                    offsetY = _margin.Bottom;
                    SetAnchorImpl(anchor, ref x, ref y, ref offsetX, ref offsetY, parentRange, false, keepOffset, pushOppositeAnchor);
                    _anchor.Top = x;
                    _anchor.Bottom = y;
                    _margin.Top = offsetX;
                    _margin.Bottom = offsetY;
                    break;
                case Side.Left:
                    x = _anchor.Left;
                    y = _anchor.Right;
                    offsetX = _margin.Left;
                    offsetY = _margin.Right;
                    SetAnchorImpl(anchor, ref x, ref y, ref offsetX, ref offsetY, parentRange, false, keepOffset, pushOppositeAnchor);
                    _anchor.Left = x;
                    _anchor.Right = y;
                    _margin.Left = offsetX;
                    _margin.Right = offsetY;
                    break;
                case Side.Bottom:
                    x = _anchor.Bottom;
                    y = _anchor.Top;
                    offsetX = _margin.Bottom;
                    offsetY = _margin.Top;
                    SetAnchorImpl(anchor, ref x, ref y, ref offsetX, ref offsetY, parentRange, true, keepOffset, pushOppositeAnchor);
                    _anchor.Bottom = x;
                    _anchor.Top = y;
                    _margin.Bottom = offsetX;
                    _margin.Top = offsetY;
                    break;
                case Side.Right:
                    x = _anchor.Right;
                    y = _anchor.Left;
                    offsetX = _margin.Right;
                    offsetY = _margin.Left;
                    SetAnchorImpl(anchor, ref x, ref y, ref offsetX, ref offsetY, parentRange, true, keepOffset, pushOppositeAnchor);
                    _anchor.Right = x;
                    _anchor.Left = y;
                    _margin.Right = offsetX;
                    _margin.Left = offsetY;
                    break;
            }
        }

        private void SetAnchorImpl(float anchor, ref float x, ref float y, ref float offsetX, ref float offsetY, float parentRange, bool compareLessThan, bool keepOffset, bool pushOppositeAnchor)
        {
            var prevPos = x;
            var prevOppositePos = y;
            x = anchor;
            if (compareLessThan ? x < y : x > y)
            {
                if (pushOppositeAnchor)
                {
                    y = x;
                }
                else
                {
                    x = y;
                }
            }

            if (!keepOffset)
            {
                offsetX = prevPos - x * parentRange;
                if (pushOppositeAnchor)
                {
                    offsetY = prevOppositePos - y * parentRange;
                }
            }

            if (InTree)
            {
                SizeChanged();
            }

            QueueRedraw();
        }

        public float GetAnchor(Side side)
        {
            return side switch
            {
                Side.Top => _anchor.Top,
                Side.Left => _anchor.Left,
                Side.Bottom => _anchor.Bottom,
                Side.Right => _anchor.Right,
                _ => throw new IndexOutOfRangeException()
            };
        }

        public void SetOffset(Side side, float value)
        {
            GuiSystem?.ThreadGuard();
            switch (side)
            {
                case Side.Top:
                    if (_margin.Top == value)
                        return;
                    _margin.Top = value;
                    break;
                case Side.Left:
                    if (_margin.Left == value)
                        return;
                    _margin.Left = value;
                    break;
                case Side.Bottom:
                    if (_margin.Bottom == value)
                        return;
                    _margin.Bottom = value;
                    break;
                case Side.Right:
                    if (_margin.Right == value)
                        return;
                    _margin.Right = value;
                    break;
            }

            SizeChanged();
        }

        public float GetOffset(Side side)
        {
            return side switch
            {
                Side.Top => _margin.Top,
                Side.Left => _margin.Left,
                Side.Bottom => _margin.Bottom,
                Side.Right => _margin.Right,
                _ => throw new IndexOutOfRangeException()
            };
        }

        public void SetAnchorAndOffset(Side side, float anchor, float margin, bool pushOppositeAnchor = true)
        {
            SetAnchor(side, anchor, false, pushOppositeAnchor);
            SetOffset(side, margin);
        }

        public void SetBegin(Point point)
        {
            GuiSystem?.ThreadGuard();
            if (_margin.Left == point.X && _margin.Top == point.Y)
                return;

            _margin.Left = point.X;
            _margin.Top = point.Y;

            SizeChanged();
        }

        public Point GetBegin()
        {
            return new Point((int)_margin.Left, (int)_margin.Top);
        }

        public void SetEnd(Point point)
        {
            GuiSystem?.ThreadGuard();
            if (_margin.Right == point.X && _margin.Bottom == point.Y)
                return;

            _margin.Right = point.X;
            _margin.Bottom = point.Y;

            SizeChanged();
        }

        public Point GetEnd()
        {
            return new Point((int)_margin.Right, (int)_margin.Bottom);
        }

        public void SetHorizontalGrowDirection(GrowDirection growDirection)
        {
            if (_growHorizontal == growDirection)
                return;

            _growHorizontal = growDirection;
            SizeChanged();
        }

        public GrowDirection GetHorizontalGrowDirection()
        {
            return _growHorizontal;
        }

        public void SetVerticalGrowDirection(GrowDirection growDirection)
        {
            if (_growVertical == growDirection)
                return;

            _growVertical = growDirection;
            SizeChanged();
        }

        public GrowDirection GetVerticalGrowDirection()
        {
            return _growVertical;
        }

        public void SetAnchorsPreset(LayoutPreset preset, bool keepOffsets = true)
        {
            GuiSystem?.ThreadGuard();

            // Left
            switch (preset)
            {
                case LayoutPreset.TopLeft:
                case LayoutPreset.BottomLeft:
                case LayoutPreset.CenterLeft:
                case LayoutPreset.TopWide:
                case LayoutPreset.LeftWide:
                case LayoutPreset.HorizontalCenterWide:
                case LayoutPreset.FullRect:
                    SetAnchor(Side.Left, Anchor.Begin, keepOffsets);
                    break;
                case LayoutPreset.CenterTop:
                case LayoutPreset.CenterBottom:
                case LayoutPreset.Center:
                case LayoutPreset.VerticalCenterWide:
                    SetAnchor(Side.Left, Anchor.Center, keepOffsets);
                    break;

                case LayoutPreset.TopRight:
                case LayoutPreset.BottomRight:
                case LayoutPreset.CenterRight:
                case LayoutPreset.RightWide:
                    SetAnchor(Side.Left, Anchor.End, keepOffsets);
                    break;
            }

            // Top
            switch (preset)
            {
                case LayoutPreset.TopLeft:
                case LayoutPreset.TopRight:
                case LayoutPreset.CenterTop:
                case LayoutPreset.LeftWide:
                case LayoutPreset.RightWide:
                case LayoutPreset.TopWide:
                case LayoutPreset.VerticalCenterWide:
                case LayoutPreset.FullRect:
                    SetAnchor(Side.Top, Anchor.Begin, keepOffsets);
                    break;

                case LayoutPreset.CenterLeft:
                case LayoutPreset.CenterRight:
                case LayoutPreset.Center:
                case LayoutPreset.HorizontalCenterWide:
                    SetAnchor(Side.Top, Anchor.Center, keepOffsets);
                    break;

                case LayoutPreset.BottomLeft:
                case LayoutPreset.BottomRight:
                case LayoutPreset.CenterBottom:
                case LayoutPreset.BottomWide:
                    SetAnchor(Side.Top, Anchor.End, keepOffsets);
                    break;
            }

            // Right
            switch (preset)
            {
                case LayoutPreset.TopLeft:
                case LayoutPreset.BottomLeft:
                case LayoutPreset.CenterLeft:
                case LayoutPreset.LeftWide:
                    SetAnchor(Side.Right, Anchor.Begin, keepOffsets);
                    break;

                case LayoutPreset.CenterTop:
                case LayoutPreset.CenterBottom:
                case LayoutPreset.Center:
                case LayoutPreset.VerticalCenterWide:
                    SetAnchor(Side.Right, Anchor.Center, keepOffsets);
                    break;

                case LayoutPreset.TopRight:
                case LayoutPreset.BottomRight:
                case LayoutPreset.CenterRight:
                case LayoutPreset.TopWide:
                case LayoutPreset.RightWide:
                case LayoutPreset.BottomWide:
                case LayoutPreset.HorizontalCenterWide:
                case LayoutPreset.FullRect:
                    SetAnchor(Side.Right, Anchor.End, keepOffsets);
                    break;
            }

            // Bottom
            switch (preset)
            {
                case LayoutPreset.TopLeft:
                case LayoutPreset.TopRight:
                case LayoutPreset.CenterTop:
                case LayoutPreset.TopWide:
                    SetAnchor(Side.Bottom, Anchor.Begin, keepOffsets);
                    break;

                case LayoutPreset.CenterLeft:
                case LayoutPreset.CenterRight:
                case LayoutPreset.Center:
                case LayoutPreset.HorizontalCenterWide:
                    SetAnchor(Side.Bottom, Anchor.Center, keepOffsets);
                    break;

                case LayoutPreset.BottomLeft:
                case LayoutPreset.BottomRight:
                case LayoutPreset.CenterBottom:
                case LayoutPreset.LeftWide:
                case LayoutPreset.RightWide:
                case LayoutPreset.BottomWide:
                case LayoutPreset.VerticalCenterWide:
                case LayoutPreset.FullRect:
                    SetAnchor(Side.Bottom, Anchor.End, keepOffsets);
                    break;
            }
        }

        public void SetOffsetsPreset(LayoutPreset preset, LayoutPresetMode resizeMode = LayoutPresetMode.MinSize, int margin = 0)
        {
            GuiSystem?.ThreadGuard();

            var minSize = GetMinimumSize();
            var newSize = GetSize();

            if (resizeMode == LayoutPresetMode.MinSize || resizeMode == LayoutPresetMode.KeepHeight)
            {
                newSize.X = minSize.X;
            }

            if (resizeMode == LayoutPresetMode.MinSize || resizeMode == LayoutPresetMode.KeepWidth)
            {
                newSize.Y = minSize.Y;
            }

            var parentRect = GetParentAnchorableRect();

            float x = parentRect.Width;
            if (IsRtl())
            {
                x = parentRect.Width - x - newSize.X;
            }

            //Left
            switch (preset)
            {
                case LayoutPreset.TopLeft:
                case LayoutPreset.BottomLeft:
                case LayoutPreset.CenterLeft:
                case LayoutPreset.TopWide:
                case LayoutPreset.BottomWide:
                case LayoutPreset.LeftWide:
                case LayoutPreset.HorizontalCenterWide:
                case LayoutPreset.FullRect:
                    _margin.Left = x * (0.0f - _anchor.Left) + margin + parentRect.X;
                    break;

                case LayoutPreset.CenterTop:
                case LayoutPreset.CenterBottom:
                case LayoutPreset.Center:
                case LayoutPreset.VerticalCenterWide:
                    _margin.Left = x * (0.5f - _anchor.Left) - newSize.X / 2 + parentRect.X;
                    break;

                case LayoutPreset.TopRight:
                case LayoutPreset.BottomRight:
                case LayoutPreset.CenterRight:
                case LayoutPreset.RightWide:
                    _margin.Left = x * (1.0f - _anchor.Left) - newSize.X - margin + parentRect.X;
                    break;
            }

            // Top
            switch (preset)
            {
                case LayoutPreset.TopLeft:
                case LayoutPreset.TopRight:
                case LayoutPreset.CenterTop:
                case LayoutPreset.LeftWide:
                case LayoutPreset.RightWide:
                case LayoutPreset.TopWide:
                case LayoutPreset.VerticalCenterWide:
                case LayoutPreset.FullRect:
                    _margin.Top = parentRect.Height * (0.0f - _anchor.Top) + margin + parentRect.Y;
                    break;

                case LayoutPreset.CenterLeft:
                case LayoutPreset.CenterRight:
                case LayoutPreset.Center:
                case LayoutPreset.HorizontalCenterWide:
                    _margin.Top = parentRect.Height * (0.5f - _anchor.Top) - newSize.Y / 2 + parentRect.Y;
                    break;

                case LayoutPreset.BottomLeft:
                case LayoutPreset.BottomRight:
                case LayoutPreset.CenterBottom:
                case LayoutPreset.BottomWide:
                    _margin.Top = parentRect.Height * (1.0f - _anchor.Top) - newSize.Y - margin + parentRect.Y;
                    break;
            }

            // Right
            switch (preset)
            {
                case LayoutPreset.TopLeft:
                case LayoutPreset.BottomLeft:
                case LayoutPreset.CenterLeft:
                case LayoutPreset.LeftWide:
                    _margin.Right = x * (0.0f - _anchor.Right) + newSize.X + margin + parentRect.X;
                    break;

                case LayoutPreset.CenterTop:
                case LayoutPreset.CenterBottom:
                case LayoutPreset.Center:
                case LayoutPreset.VerticalCenterWide:
                    _margin.Right = x * (0.5f - _anchor.Right) + newSize.X / 2 + parentRect.X;
                    break;

                case LayoutPreset.TopRight:
                case LayoutPreset.BottomRight:
                case LayoutPreset.CenterRight:
                case LayoutPreset.TopWide:
                case LayoutPreset.RightWide:
                case LayoutPreset.BottomWide:
                case LayoutPreset.HorizontalCenterWide:
                case LayoutPreset.FullRect:
                    _margin.Right = x * (1.0f - _anchor.Right) - margin + parentRect.X;
                    break;
            }

            // Bottom
            switch (preset)
            {
                case LayoutPreset.TopLeft:
                case LayoutPreset.TopRight:
                case LayoutPreset.CenterTop:
                case LayoutPreset.TopWide:
                    _margin.Bottom = parentRect.Height * (0.0f - _anchor.Bottom) + newSize.Y + margin + parentRect.Y;
                    break;

                case LayoutPreset.CenterLeft:
                case LayoutPreset.CenterRight:
                case LayoutPreset.Center:
                case LayoutPreset.HorizontalCenterWide:
                    _margin.Bottom = parentRect.Height * (0.5f - _anchor.Bottom) + newSize.Y / 2 + parentRect.Y;
                    break;

                case LayoutPreset.BottomLeft:
                case LayoutPreset.BottomRight:
                case LayoutPreset.CenterBottom:
                case LayoutPreset.LeftWide:
                case LayoutPreset.RightWide:
                case LayoutPreset.BottomWide:
                case LayoutPreset.VerticalCenterWide:
                case LayoutPreset.FullRect:
                    _margin.Bottom = parentRect.Height * (1.0f - _anchor.Bottom) - margin + parentRect.Y;
                    break;
            }

            SizeChanged();
        }

        public void SetAnchorsAndOffsetsPreset(LayoutPreset preset, LayoutPresetMode resizeMode = LayoutPresetMode.MinSize, int margin = 0)
        {
            GuiSystem?.ThreadGuard();

            SetAnchorsPreset(preset);
            SetOffsetsPreset(preset, resizeMode, margin);
        }

        public void SetGrowDirectionPreset(LayoutPreset preset)
        {
            GuiSystem?.ThreadGuard();

            // Select correct horizontal grow direction.
            switch (preset)
            {
                case LayoutPreset.TopLeft:
                case LayoutPreset.BottomLeft:
                case LayoutPreset.CenterLeft:
                case LayoutPreset.LeftWide:
                    SetHorizontalGrowDirection(GrowDirection.End);
                    break;
                case LayoutPreset.TopRight:
                case LayoutPreset.BottomRight:
                case LayoutPreset.CenterRight:
                case LayoutPreset.RightWide:
                    SetHorizontalGrowDirection(GrowDirection.Begin);
                    break;
                case LayoutPreset.CenterTop:
                case LayoutPreset.CenterBottom:
                case LayoutPreset.Center:
                case LayoutPreset.TopWide:
                case LayoutPreset.BottomWide:
                case LayoutPreset.VerticalCenterWide:
                case LayoutPreset.HorizontalCenterWide:
                case LayoutPreset.FullRect:
                    SetHorizontalGrowDirection(GrowDirection.Both);
                    break;
            }

            // Select correct vertical grow direction.
            switch (preset)
            {
                case LayoutPreset.TopLeft:
                case LayoutPreset.TopRight:
                case LayoutPreset.CenterTop:
                case LayoutPreset.TopWide:
                    SetVerticalGrowDirection(GrowDirection.End);
                    break;

                case LayoutPreset.BottomLeft:
                case LayoutPreset.BottomRight:
                case LayoutPreset.CenterBottom:
                case LayoutPreset.BottomWide:
                    SetVerticalGrowDirection(GrowDirection.Begin);
                    break;

                case LayoutPreset.CenterLeft:
                case LayoutPreset.CenterRight:
                case LayoutPreset.Center:
                case LayoutPreset.LeftWide:
                case LayoutPreset.RightWide:
                case LayoutPreset.VerticalCenterWide:
                case LayoutPreset.HorizontalCenterWide:
                case LayoutPreset.FullRect:
                    SetVerticalGrowDirection(GrowDirection.Both);
                    break;
            }
        }

        public void SetPosition(Point position, bool keepOffsets = false)
        {
            GuiSystem?.ThreadGuard();

            if (keepOffsets)
            {
                var anchor = ComputeAnchors(new Rectangle(position, _sizeCache), _margin);
                _anchor = anchor;
            }
            else
            {
                var margin = ComputeOffsets(new Rectangle(position, _sizeCache), _anchor);
                _margin = margin;
            }

            SizeChanged();
        }

        public Point GetPosition()
        {
            return _posCache;
        }

        public Point GetScreenPosition()
        {
            if (!InTree)
                throw new InvalidOperationException();

            return GuiSystem!.Camera.WorldToScreen(GetPosition().ToVector2()).ToPoint();
        }

        public void SetSize(Point size, bool keepOffsets = false)
        {
            GuiSystem?.ThreadGuard();

            var newSize = size;
            var minSize = GetCombinedMinimumSize();

            if (newSize.X < minSize.X)
                newSize.X = minSize.X;

            if (newSize.Y < minSize.Y)
                newSize.Y = minSize.Y;

            if (keepOffsets)
            {
                _anchor = ComputeAnchors(new Rectangle(_posCache, newSize), _margin);
            }
            else
            {
                _margin = ComputeOffsets(new Rectangle(_posCache, newSize), _anchor);
            }

            SizeChanged();
        }

        public Point GetSize()
        {
            return _sizeCache;
        }

        public void ResetSize()
        {
            GuiSystem?.ThreadGuard();

            SetSize(Point.Zero);
        }

        public void SetRect(Rectangle rect)
        {
            GuiSystem?.ThreadGuard();

            _anchor = new Anchor(Anchor.Begin, Anchor.Begin, Anchor.Begin, Anchor.Begin);

            _margin = ComputeOffsets(rect, _anchor);
            if (InTree)
            {
                SizeChanged();
            }
        }

        public Rectangle GetRect()
        {
            var transform = GetTransform();
            var position = Vector2.Zero;
            var size = GetSize().ToVector2();
            Vector2.Transform(ref position, ref transform, out position);
            Vector2.Transform(ref size, ref transform, out size);
            return new Rectangle(position.ToPoint(), size.ToPoint());
        }

        public Rectangle GetScreenRect()
        {
            if (!InTree)
                throw new InvalidOperationException();
            var rect = GetRect();
            var topLeft = rect.Location.ToVector2();
            var bottomRight = topLeft + rect.Size.ToVector2();

            var transformedTopLeft = GuiSystem!.Camera.WorldToScreen(topLeft);
            var transformedBottomRight = GuiSystem!.Camera.WorldToScreen(bottomRight);

            var size = transformedBottomRight - transformedTopLeft;

            return new Rectangle(transformedTopLeft.ToPoint(), size.ToPoint());
        }

        public Rectangle GetAnchorableRect()
        {
            return new Rectangle(Point.Zero, GetSize());
        }

        public void SetScale(Vector2 scale)
        {
            GuiSystem?.ThreadGuard();

            if (_scale == scale)
                return;

            _scale = scale;

            if (scale.X == 0)
                scale.X = MathExt.Epsilon;

            if (scale.Y == 0)
                scale.Y = MathExt.Epsilon;

            QueueRedraw();
            NotifyTransform();
        }

        public Vector2 GetScale()
        {
            return _scale;
        }

        public void SetRotation(float radians)
        {
            GuiSystem?.ThreadGuard();

            if (_rotation == radians)
                return;

            _rotation = radians;
            QueueRedraw();
            NotifyTransform();
        }

        public void SetRotationDegrees(float degrees)
        {
            SetRotation(MathHelper.ToRadians(degrees));
        }

        public float GetRotation()
        {
            return _rotation;
        }

        public float GetRotationDegrees()
        {
            return MathHelper.ToDegrees(_rotation);
        }

        public void SetPivotOffset(Vector2 pivotPoint)
        {
            GuiSystem?.ThreadGuard();

            if (_pivotOffset == pivotPoint)
                return;

            _pivotOffset = pivotPoint;
            QueueRedraw();
            NotifyTransform();
        }

        public Vector2 GetPivotOffset()
        {
            return _pivotOffset;
        }

        private void UpdateMinimumSizeComplete()
        {
            if (!InTree)
            {
                _updatingLastMinimumSize = false;
                return;
            }

            var minSize = GetCombinedMinimumSize();
            _updatingLastMinimumSize = false;

            if (minSize != _lastMinimumSize)
            {
                _lastMinimumSize = minSize;
                SizeChanged();

                // Todo: Emit Signal?
            }
        }

        public void UpdateMinimumSize()
        {
            GuiSystem?.ThreadGuard();

            if (!InTree || _blockMinimumSizeAdjust)
                return;

            Control? invalidate = this;
            while (invalidate != null && invalidate._minimumSizeValid)
            {
                invalidate._minimumSizeValid = false;
                if (invalidate.IsTopLevelControl())
                    break;

                // Todo: Window stuff...

                invalidate = invalidate.GetParentControl();
            }

            if (!VisibleInTree())
                return;

            if (_updatingLastMinimumSize)
                return;

            _updatingLastMinimumSize = true;

            // Todo: push UpdateMinimumSizeComplete
        }

        public void SetBlockMinimumSizeAdjust(bool block)
        {
            GuiSystem?.ThreadGuard();
            _blockMinimumSizeAdjust = block;
        }

        public virtual Point GetMinimumSize()
        {
            return Point.Zero;
        }

        public virtual Point GetCombinedMinimumSize()
        {
            if (!_minimumSizeValid)
            {
                UpdateMinimumSizeCache();
            }

            return _minimumSizeCache;
        }

        public void SetCustomMinimumSize(Point size)
        {
            GuiSystem?.ThreadGuard();
            if (size == _customMinimumSize)
                return;

            _customMinimumSize = size;
            UpdateMinimumSize();
            UpdateConfigurationWarnings();
        }

        public Point GetCustomMinimumSize()
        {
            return _customMinimumSize;
        }

        public void SetHorizontalSizeFlags(SizeFlags flags)
        {
            GuiSystem?.ThreadGuard();
            if (_sizeFlagsHorizontal == flags)
                return;

            _sizeFlagsHorizontal = flags;
            // Todo: Emit size flags changed signal
        }

        public SizeFlags GetHorizontalSizeFlags()
        {
            return _sizeFlagsHorizontal;
        }

        public void SetVerticalSizeFlags(SizeFlags flags)
        {
            GuiSystem?.ThreadGuard();
            if (_sizeFlagsVertical == flags)
                return;

            _sizeFlagsVertical = flags;
            // Todo: Emit size flags changed signal
        }

        public SizeFlags GetVerticalSizeFlags()
        {
            return _sizeFlagsVertical;
        }

        public void SetStretchRatio(float ratio)
        {
            GuiSystem?.ThreadGuard();

            if (_expand == ratio)
                return;

            _expand = ratio;
            // Todo: Emit size flags changed signal
        }

        public float GetStretchRatio()
        {
            return _expand;
        }
    }
}
