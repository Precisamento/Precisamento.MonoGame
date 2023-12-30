using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public class Control
    {
        private bool _initialized;

        private Control? _parent;
        private GameWindow? _window;
        private bool _inTree;


        private Action? _forwardDrag;
        private Action? _forwardCanDrop;
        private Action? _forwardDrop;


        // Positioning and Sizing

        private LayoutMode _storedLayoutMode = LayoutMode.Position;
        private bool _storedUseCustomAnchors = false;
        private Margin _margin = new Margin(0, 0, 0, 0);
        private Anchor _anchor = new Anchor(Anchor.Begin, Anchor.Begin, Anchor.Begin, Anchor.Begin);
        private FocusMode _focusMode = FocusMode.None;
        private GrowDirection _growHorizontal = GrowDirection.End;
        private GrowDirection _growVertical = GrowDirection.End;
        private float _rotation = 0;
        private Vector2 _scale = new Vector2(1);
        private Vector2 _pivotOffset;

        private Point _posCache;
        private Point _sizeCache;
        private Point _minimumSizeCache;
        private bool _minimumSizeValid = false;

        private Point _lastMinimumSize;
        private bool _updatingLastMinimumSize = false;
        private bool _blockMinimumSizeAdjust;

        private bool _sizeWarning;


        // Container sizing

        private SizeFlags _sizeFlagsHorizontal = SizeFlags.Fill;
        private SizeFlags _sizeFlagsVertical = SizeFlags.Fill;
        private float _expand = 1;
        private Point _customMinimumSize;


        // Input and rendering


        private MouseFilter _mouseFilter = MouseFilter.Stop;
        private bool _forcePassScrollEvents = true;

        private bool _clipContents = false;
        private bool _disableVisibilityClip = false;

        private CursorShape _defaultCursorShape = CursorShape.Arrow;


        // Focus

        private Control? _shortcutContext;


        // Theming

        private ThemeOwner? _themeOwner;
        private Theme? _theme;
        private string _themeType;

        private bool _bulkThemeOverride;
        private ThemeData _themeOverride;
        private ThemeData _themeCache;


        // i18n

        private LayoutDirection _layoutDirection = LayoutDirection.Inherited;
        private bool _isRtlDirty = true;
        private bool _isRtl = false;

        private bool _autoTranslate = true;
        private bool _localizeNumeralSystem = true;

        private string _tooltipText;


        private Vector2 _minimumSize;
        private Vector2 _globalPosition;
        private Vector2 _position;
        private float _rotationDegrees;
        private Vector2 _size;
        private float _sizeFlagsStretchRatio;

        public Gui? GuiSystem { get; set; }

        public Control? NeighborTop { get; set; }
        public Control? NeighborLeft { get; set; }
        public Control? NeighborRight { get; set; }
        public Control? NeighborBottom { get; set; }

        public Control? FocusNext { get; set; }
        public Control? FocusPrevious { get; set; }

        public bool InTree
        {
            get => GuiSystem is not null;
        }

        private void QueueRedraw()
        {

        }

        private void NotifyPropertyListChanged()
        {

        }

        private void NotifyTransform()
        {

        }

        private bool VisibleInTree()
        {
            throw new NotImplementedException();
        }

        private Transform2 GetGlobalTransform()
        {
            throw new NotImplementedException();
        }

        private Transform2 GetScreenTransform()
        {
            throw new NotImplementedException();
        }

        private void UpdateCanvasItemTransform()
        {
            throw new NotImplementedException();
        }

        private Transform2 GetInternalTransform()
        {
            //Transform2 rotScale = new Transform2(rotation: _rotation, scale: _scale);
            //Transform2 offset = new Transform2(position: -_pivotOffset);

            return new Transform2(-_pivotOffset, _rotation, _scale);
        }

        private void UpdateConfigurationWarnings()
        {

        }

        private void SetAnchor(Anchor anchor, bool keepOffset, bool pushOppositeAnchor)
        {

        }

        private void SetPosition(Point position)
        {

        }

        private void SetGlobalPosition(Point position)
        {

        }

        private void SetSize(Point Size)
        {

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
            }
        }

        private void UpdateLayoutMode()
        {
            LayoutMode computedLayout = GetLayoutMode();
            if (_storedLayoutMode != computedLayout)
            {
                _storedLayoutMode = computedLayout;
                NotifyPropertyListChanged();
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
                if (sizeChanged)
                {
                    // Todo: Notification(NOTIFICATION_RESIZED);
                }

                if (posChanged || sizeChanged)
                {
                    // Todo: ItemRectChanged(sizeChanged);
                    NotifyTransform();
                }

                if (posChanged && !sizeChanged)
                {
                    UpdateCanvasItemTransform();
                }
            }
            else if (posChanged)
            {
                NotifyTransform();
            }
        }

        private void ClearSizeWarning()
        {
            _sizeWarning = false;
        }

        private void CallGuiEvent(InputEvent inputEvent)
        {
            // Todo: Check if inputEvent is an internal event

            if (!InTree /* Todo: || is_input_handled() */)
            {
                return;
            }

            //  
        }

        private void WindowFindFocusNeighbor(Vector2 dir, object at, Point points, float min, ref float closest_dist, out Control closest)
        {
            throw new NotImplementedException();
        }

        private Control GetFocusNeighbor(Side side, int count = 0)
        {
            throw new NotImplementedException();
        }

        private void ThemeChanged()
        {
            throw new NotImplementedException();
        }

        private void NotifyThemeOverrideChanged()
        {
            throw new NotImplementedException();
        }

        private void InvalidateThemeCache()
        {
            throw new NotImplementedException();
        }

        private string GetTooltipText()
        {
            throw new NotImplementedException();
        }

        protected virtual void UpdateThemeItemCache()
        {
            throw new NotImplementedException();
        }

        // TODO: structured_text_parser

        protected void Notification(int notification)
        {
            throw new NotImplementedException();
        }

        protected bool HasPoint(Vector2 point)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetTooltip()
        {
            throw new NotImplementedException();
        }

        protected virtual object GetDragData(Vector2 position)
        {
            throw new NotImplementedException();
        }

        protected bool CanDropData(Vector2 position, object context)
        {
            throw new NotImplementedException();
        }

        protected virtual void DropData(Vector2 position, object context)
        {
            throw new NotImplementedException();
        }

        public virtual void Reparent(Control parent, bool keepGlobalTransform)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsTextField()
        {
            throw new NotImplementedException();
        }

        public bool IsTopLevelControl()
        {
            return InTree && _parent is null;
        }

        public Control? GetParentControl()
        {
            return _parent;
        }

        public GameWindow? GetParentWindow()
        {
            return _window;
        }

        public Control GetRootParentControl()
        {
            var node = this;
            while (node._parent != null)
            {
                node = node._parent;
            }

            return node;
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
        
        public virtual Transform2 GetTransform()
        {
            var transform = GetInternalTransform();
            transform.Position += GetPosition().ToVector2();
            return transform;
        }

        public void SetAnchor(Side side, float anchor, bool keepOffset = true, bool pushOppositeAnchor = true) 
        {
            GuiSystem?.ThreadGuard();

            var parentRect = GetParentAnchorableRect();
            var parentRange = (side == Side.Left || side == Side.Right) ? parentRect.Width : parentRect.Height;

            float x, y, offsetX, offsetY;

            switch(side)
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
            switch(preset)
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
            throw new NotImplementedException();
        }

        public void SetGrowDirectionPreset(LayoutPreset preset)
        {
            GuiSystem.ThreadGuard();

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

        public void SetGlobalPosition(Point position, bool keepOffsets = false)
        {
            throw new NotImplementedException();
        }

        public Point GetPosition()
        {
            return _posCache;
        }

        public Point GetGlobalPosition()
        {
            throw new NotImplementedException();
        }

        public Point GetScreenPosition()
        {
            throw new NotImplementedException();
        }

        public void SetSize(Point size, bool keepOffsets = false)
        {
            GuiSystem?.ThreadGuard();

            var newSize = size;
            var minSize = GetCombinedMinimumSize();

            if (newSize.X < minSize.X)
                newSize.X = minSize.X;

            if (newSize.Y  < minSize.Y)
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
            return new Rectangle(transform.Position.ToPoint(), (transform.Scale * GetSize().ToVector2()).ToPoint());
        }

        public Rectangle GetGlobalRect()
        {
            var transform = GetGlobalTransform();
            return new Rectangle(transform.Position.ToPoint(), (transform.Scale * GetSize().ToVector2()).ToPoint());
        }

        public Rectangle GetScreenRect()
        {
            if (!InTree)
                throw new InvalidOperationException();

            var transform = GetScreenTransform();
            return new Rectangle(transform.Position.ToPoint(), (transform.Scale * GetSize().ToVector2()).ToPoint());
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

        public virtual void GuiInput(InputEvent inputEvent)
        {
            throw new NotImplementedException();
        }

        public void AcceptEvent()
        {
            throw new NotImplementedException();
        }

        public virtual bool HasPoint(Point point)
        {
            throw new NotImplementedException();
        }

        public void SetMouseFilter(MouseFilter filter) 
        { 
            throw new NotImplementedException();
        }

        public MouseFilter GetMouseFilter()
        {
            throw new NotImplementedException();
        }

        public void SetForcePassScrollEvents(bool forcePassScrollEvents)
        {
            throw new NotImplementedException();
        }

        public bool GetForcePassScrollEvents()
        {
            throw new NotImplementedException();
        }

        public void WarpMouse(Point position)
        {
            throw new NotImplementedException();
        }

        public bool IsFocusOwnerInShortcutContext()
        {
            throw new NotImplementedException();
        }

        public void SetShortcutContext(Control context)
        {
            throw new NotImplementedException();
        }

        public Control GetShortcutContext()
        {
            throw new NotImplementedException();
        }

        public virtual void SetDragForwarding(Action drag, Action canDrop, Action drop)
        {
            throw new NotImplementedException();
        }

        public virtual object GetDragData(Point point)
        {
            throw new NotImplementedException();
        }

        public virtual bool CanDropData(Point point, object data)
        {
            throw new NotImplementedException();
        }

        public virtual void DropData(Point point, object data)
        {
            throw new NotImplementedException();
        }

        public void SetDragPreview(Control control)
        {
            throw new NotImplementedException();
        }

        public void ForceDrag(object data, Control control)
        {
            throw new NotImplementedException();
        }

        public bool IsDragSuccessful()
        {
            throw new NotImplementedException();
        }

        public void SetFocusMode(FocusMode focusMode)
        {
            throw new NotImplementedException();
        }

        public FocusMode GetFocusMode()
        {
            throw new NotImplementedException();
        }

        public bool HasFocus()
        {
            throw new NotImplementedException();
        }

        public void GrabFocus()
        {
            throw new NotImplementedException();
        }

        public void GrabClickFocus()
        {
            throw new NotImplementedException();
        }

        public void ReleaseFocus()
        {
            throw new NotImplementedException();
        }

        public Control FindNextValidFocus()
        {
            throw new NotImplementedException();
        }

        public Control FindPreviousValidFocus()
        {
            throw new NotImplementedException();
        }

        public Control FindValidFocusNeighbor(Side side)
        {
            throw new NotImplementedException();
        }

        public void SetFocusNeighbor(Side side, Control control)
        {
            throw new NotImplementedException();
        }

        public Control GetFocusNeighbor(Side side)
        {
            throw new NotImplementedException();
        }

        public void SetFocusNext(Control control)
        {
            throw new NotImplementedException();
        }

        public Control GetFocusNext()
        {
            throw new NotImplementedException();
        }

        public void SetFocusPrevious(Control control)
        {
            throw new NotImplementedException();
        }

        public Control GetFocusPrevious()
        {
            throw new NotImplementedException();
        }

        public void SetDefaultCursorShape(CursorShape shape)
        {
            throw new NotImplementedException();
        }

        public CursorShape GetDefaultCursorShape()
        {
            throw new NotImplementedException();
        }

        public virtual CursorShape GetCursorShape()
        {
            throw new NotImplementedException();
        }

        public void SetClipContents(bool clip)
        {
            throw new NotImplementedException();
        }

        public bool GetClipContents()
        {
            throw new NotImplementedException();
        }

        public void SetDisableVisibilityClip(bool ignore)
        {
            throw new NotImplementedException();
        }

        public bool GetDisableVisibilityClip()
        {
            throw new NotImplementedException();
        }

        public void SetThemeOwner(object obj)
        {
            throw new NotImplementedException();
        }

        public object GetThemeOwner()
        {
            throw new NotImplementedException();
        }

        public bool HasThemeOwner()
        {
            throw new NotImplementedException();
        }

        public void SetThemeContext(ThemeContext context, bool propogate)
        {
            throw new NotImplementedException();
        }

        public void SetTheme(Theme theme)
        {
            throw new NotImplementedException();
        }

        public Theme GetTheme()
        {
            throw new NotImplementedException();
        }

        public void SetThemeVariation(string themeType)
        {
            throw new NotImplementedException();
        }

        public string GetThemeVariation()
        {
            throw new NotImplementedException();
        }

        public void BeginBulkThemeOverride()
        {
            throw new NotImplementedException();
        }

        public void EndBulkThemeOverride()
        {
            throw new NotImplementedException();
        }

        public void AddThemeIconOverride(string name, TextureRegion2D texture)
        {
            throw new NotImplementedException();
        }

        public void AddThemeStyleOverride(string name, StyleBox style)
        {
            throw new NotImplementedException();
        }

        public void AddThemeFontOverride(string name, IFont font)
        {
            throw new NotImplementedException();
        }

        public void AddThemeFontSizeOverride(string name, int fontSize)
        {
            throw new NotImplementedException();
        }

        public void AddThemeColorOverride(string name, Color color)
        {
            throw new NotImplementedException();
        }

        public void AddThemeConstantOverride(string name, int value)
        {
            throw new NotImplementedException();
        }

        public void RemoveThemeIconOverride(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveThemeStyleOverride(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveThemeFontOverride(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveThemeFontSizeOverride(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveThemeColorOverride(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveThemeConstantOverride(string name)
        {
            throw new NotImplementedException();
        }

        public TextureRegion2D GetThemeIcon(string name)
        {
            throw new NotImplementedException();
        }

        public StyleBox GetThemeStyle(string name)
        {
            throw new NotImplementedException();
        }

        public IFont GetThemeFont(string name)
        {
            throw new NotImplementedException();
        }

        public int GetThemeFontSize(string name)
        {
            throw new NotImplementedException();
        }

        public Color GetThemeColor(string name)
        {
            throw new NotImplementedException();
        }

        public int GetThemeConstant(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeIconOverride(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeStyleOverride(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeFontOverride(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeFontSizeOverride(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeColorOverride(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeConstantOverride(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeIcon(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeStyle(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeFont(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeFontSize(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeColor(string name)
        {
            throw new NotImplementedException();
        }

        public bool HasThemeConstant(string name)
        {
            throw new NotImplementedException();
        }

        public float GetThemeDefaultBaseScale()
        {
            throw new NotImplementedException();
        }

        public IFont GetThemeDefaultFont()
        {
            throw new NotImplementedException();
        }

        public int GetThemeDefaultFontSize()
        {
            throw new NotImplementedException();
        }

        public void SetLayoutDirection(LayoutDirection direction)
        {
            throw new NotImplementedException();
        }

        public LayoutDirection GetLayoutDirection()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsRtl()
        {
            throw new NotImplementedException();
        }

        public void SetLocalizeNumeralSystem(bool localizeNumeralSystem)
        {
            throw new NotImplementedException();
        }

        public bool GetLocalizeNumeralSystem()
        {
            throw new NotImplementedException();
        }

        public void SetAutoTranslate(bool translate)
        {
            throw new NotImplementedException();
        }

        public bool GetAutoTranslate()
        {
            throw new NotImplementedException();
        }

        public string Translate(string str)
        {
            throw new NotImplementedException();
        }

        public void SetTooltipText(string text)
        {
            throw new NotImplementedException();
        }

        public virtual string GetTooltip(Point point)
        {
            throw new NotImplementedException();
        }

        public virtual Control MakeCustomTooltip(string text)
        {
            throw new NotImplementedException();
        }
    }
}
