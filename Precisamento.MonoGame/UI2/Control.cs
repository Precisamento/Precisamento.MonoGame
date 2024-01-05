using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.MathHelpers;
using Precisamento.MonoGame.UI2.Events;
using Precisamento.MonoGame.UI2.Styling.Brushes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public class Control : INotifyAttachedPropertyChanged
    {
        private static BoxCollider _collisionTest = new BoxCollider(0, 0);
        private static object _collisionTestLock = new object();

        private MouseCursor? _mouseCursor;
        private Vector2? _startPosition;
        private Point _startLeftTop;
        private Thickness _margin;
        private Thickness _borderThickness;
        private Thickness _padding;
        private Point _position;
        private Point _size;
        private Point _minSize;
        private Point _maxSize;
        private int _zIndex;
        private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;
        private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;
        private bool _measureDirty = true;
        private bool _arrangeDirty = true;
        private bool _transformDirty = true;
        private GuiSystem? _gui;
        private string? _id = null;

        private Point _lastMeasureSize;
        private Point _lastMeasureAvailableSize;

        private Rectangle _containerBounds;
        private Rectangle _layoutBounds;

        private bool _visible;
        private float _opacity = 1;

        private bool _enabled;
        private bool _focused;

        private Vector2 _scale = Vector2.One;
        private Vector2 _origin = Vector2.Zero;
        private float _rotation = 0;

        private Matrix _transform;

        public string? Id
        {
            get => _id;
            set
            {
                if (value == _id)
                    return;

                _id = value;
                OnIdChanged();
            }
        }

        public Dictionary<int, object?> AttachedPropertyValues { get; } = new Dictionary<int, object?>();

        public int Top
        {
            get => _position.Y;
            set
            {
                if (value == _position.Y)
                    return;

                _position.Y = value;
                InvalidateTransform();
                FireLocationChanged();
            }
        }

        public int Left
        {
            get => _position.X;
            set
            {
                if (value == _position.X)
                    return;

                _position.X = value;
                InvalidateTransform();
                FireLocationChanged();
            }
        }

        public int Bottom
        {
            get => _position.Y + _size.Y;
            set => Height = value - _position.Y;
        }

        public int Right
        {
            get => _position.X + _size.Y;
            set => Width = value - _position.X;
        }

        public int Width
        {
            get => _size.X;
            set
            {
                if (value == _size.X)
                    return;

                _size.X = value;
                InvalidateMeasure();
                FireSizeChanged();
            }
        }

        public int Height
        {
            get => _size.Y;
            set
            {
                if (value == _size.Y)
                    return;

                _size.Y = value;
                InvalidateMeasure();
                FireSizeChanged();
            }
        }

        public int MinWidth
        {
            get => _minSize.X;
            set
            {
                if (value == _minSize.X)
                    return;

                _minSize.X = value;
                InvalidateMeasure();
                FireSizeChanged();
            }
        }

        public int MinHeight
        {
            get => _minSize.Y;
            set
            {
                if (value == _minSize.Y)
                    return;

                _minSize.Y = value;
                InvalidateMeasure();
                FireSizeChanged();
            }
        }

        public int MaxWidth
        {
            get => _maxSize.X;
            set
            {
                if (value == _maxSize.X)
                    return;

                _maxSize.X = value;
                InvalidateMeasure();
                FireSizeChanged();
            }
        }

        public int MaxHeight
        {
            get => _maxSize.Y;
            set
            {
                if (value == _maxSize.Y)
                    return;

                _maxSize.Y = value;
                InvalidateMeasure();
                FireSizeChanged();
            }
        }
        
        public Thickness Margin
        {
            get => _margin;
            set
            {
                if (value.Equals(_margin))
                    return;

                _margin = value;
                InvalidateMeasure();
            }
        }

        public Thickness BorderThickness
        {
            get => _borderThickness;
            set
            {
                if (value.Equals(_borderThickness))
                    return;

                _borderThickness = value;
                InvalidateMeasure();
            }
        }

        public Thickness Padding
        {
            get => _padding;
            set
            {
                if (value.Equals(_padding))
                    return;

                _padding = value;
                InvalidateMeasure();
            }
        }

        public HorizontalAlignment HorizontalAlignment 
        {
            get => _horizontalAlignment;
            set
            {
                if (value == _horizontalAlignment)
                    return;

                _horizontalAlignment = value;
                InvalidateMeasure();
            }
        }
        public VerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                if (value == _verticalAlignment)
                    return;

                _verticalAlignment = value;
                InvalidateMeasure();
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value == _enabled)
                    return;

                _enabled = value;
                OnEnabledChanged();
            }
        }

        public bool Visible
        {
            get => _visible;
            set
            {
                if (value == _visible)
                    return;

                _visible = value;

                // Todo: Reset position data.

                OnVisibleChanged();
            }
        }

        // Todo: Drag stuff...

        public int ZIndex
        {
            get => _zIndex;
            set
            {
                if (value == _zIndex)
                    return;

                _zIndex = value;
                InvalidateMeasure();
            }
        }

        public MouseCursor? MouseCursor
        {
            get => _mouseCursor;
            set
            {
                if (value == _mouseCursor)
                    return;

                _mouseCursor = value;
                OnMouseCursorChanged();
            }
        }

        public string? Tooltip { get; set; }

        public Vector2 Scale
        {
            get => _scale;
            set
            {
                if (value == _scale)
                    return;

                _scale = value;
                InvalidateTransform();
            }
        }

        public Vector2 ScaleWithParent
        {
            get
            {
                return Scale * Parent?.ScaleWithParent ?? Vector2.One;
            }
        }

        public Vector2 Origin
        {
            get => _origin;
            set
            {
                if (_origin == value)
                    return;

                _origin = value;
                InvalidateTransform();
            }
        }

        public float Rotation
        {
            get => _rotation;
            set
            {
                if (value == _rotation)
                    return;

                _rotation = value;
                InvalidateTransform();
            }
        }

        public float RotationWithParent
        {
            get => Rotation + Parent?.RotationWithParent ?? 0;
        }

        public Matrix Transform
        {
            get
            {
                if (_transformDirty)
                {
                    _transform =
                        Matrix.CreateTranslation(new Vector3(-_origin, 0f))
                        * Matrix.CreateRotationZ(_rotation)
                        * Matrix.CreateScale(new Vector3(_scale, 1f))
                        * Matrix.CreateTranslation(new Vector3(_origin, 0f));

                    _transformDirty = false;
                }

                return _transform;
            }
        }

        public bool IsPlaced
        {
            get => GuiSystem != null;
        }

        public GuiSystem? GuiSystem
        {
            get => _gui;
            set
            {
                if (_gui != null && value is null)
                {
                    if (_gui.FocusedControl == this)
                    {
                        _gui.FocusedControl = null;
                    }

                    if (_gui.Tooltip != null)
                    {
                        // Todo: Determine if the tooltip needs to be cleared.
                    }
                }

                // Todo: Reset input data.

                _gui = value;

                if (_gui != null)
                {
                    InvalidateMeasure();
                }

                // Todo: SubscribeOnTouchMoved

                OnPlacedChanged();
            }
        }

        public bool IsModal { get; set; }

        public float Opacity
        {
            get => _opacity;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _opacity = value;
            }
        }

        public float OpacityWithParent
        {
            get => Opacity * Parent?.OpacityWithParent ?? 1;
        }

        public IBrush? Background { get; set; }

        public IBrush? HoverBackground { get; set; }

        public IBrush? DisabledBackground { get; set; }

        public IBrush? FocusedBackground { get; set; }

        public IBrush? Border { get; set; }

        public IBrush? HoverBorder { get; set; }

        public IBrush? DisabledBorder { get; set; }

        public IBrush? FocusedBorder { get; set; }

        public virtual bool ClipToBounds { get; set; }

        public ParentControl? Parent { get; set; }

        public object? Tag { get; set; }

        public Rectangle Bounds => _layoutBounds;

        public Rectangle ActualBounds => Bounds.Subtract(_margin).Subtract(_borderThickness).Subtract(_padding);

        public Rectangle BorderBounds => Bounds.Subtract(_margin);

        public Rectangle BackgroundBounds => BorderBounds.Subtract(_borderThickness);

        public Rectangle ContainerBounds => _containerBounds;

        public int MBPWidth => Margin.Width + BorderThickness.Width + Padding.Width;

        public int MBPHeight => Margin.Height + BorderThickness.Height + Padding.Height;

        public bool AcceptsFocus { get; set; }

        public bool Focused
        {
            get => _focused;
            set
            {
                if (value == _focused)
                    return;

                _focused = value;
                OnFocusChanged();
            }
        }

        public bool Hovering
        {
            get; set;
        }

        public bool IsTouchInside { get; }

        public event EventHandler<RenderEventArgs>? BeforeRender;

        public event EventHandler<RenderEventArgs>? AfterRender;

        public event EventHandler? PlacedChanged;
        public event EventHandler? VisibleChanged;
        public event EventHandler? EnabledChanged;

        public event EventHandler? LocationChanged;
        public event EventHandler? SizeChanged;
        public event EventHandler? ArrangeUpdated;

        public Control()
        {
            Visible = true;
            Enabled = true;
        }

        //public void BringToFront()
        //{
        //    var controls = Parent?.Children ?? GuiSystem?.Controls;
        //    if (controls is null || controls.Count == 0)
        //        return;

        //    if (controls[^1] == this)
        //        return;

        //    controls.Remove(this);
        //    controls.Add(this);
        //}

        //public void BringToBack()
        //{
        //    var controls = Parent?.Children ?? GuiSystem?.Controls;
        //    if (controls is null || controls.Count == 0)
        //        return;

        //    if (controls[0] == this)
        //        return;

        //    controls.Remove(this);
        //    controls.Insert(0, this);
        //}

        public void Update(float delta)
        {
            UpdateArrange();
            InternalUpdate(delta);
        }

        public virtual IBrush? GetCurrentBackground()
        {
            var result = Background;

            if (!Enabled && DisabledBackground != null)
            {
                result = DisabledBackground;
            }
            else if (Focused && FocusedBackground != null)
            {
                result = FocusedBackground;
            }
            else if (Hovering)
            {
                result = HoverBackground;
            }

            return result;
        }

        public virtual IBrush? GetCurrentBorder()
        {
            var result = Border;

            if (!Enabled && DisabledBorder != null)
            {
                result = DisabledBorder;
            }
            else if (Focused && FocusedBorder != null)
            {
                result = FocusedBorder;
            }
            else if (Hovering)
            {
                result = HoverBorder;
            }

            return result;
        }

        public void Draw(SpriteBatchState state)
        {
            // Todo: Cache this
            var args = new RenderEventArgs(state);

            var opacity = OpacityWithParent;
            var color = new Color(1, 1, 1, opacity);

            var background = GetCurrentBackground();
            background?.Draw(state, BackgroundBounds, color);

            var border = GetCurrentBorder();
            if (border != null)
            {
                BrushUtils.Draw(state, border, BorderBounds, BorderThickness, color);
            }

            BeforeRender?.Invoke(this, args);
            InternalDraw(state);
            AfterRender?.Invoke(this, args);

            if (GuiSystem?.DebugDraw ?? false)
            {
                state.SpriteBatch.DrawRectangle(Bounds, Color.Red);
            }
        }

        public void OnLostFocus()
        {
        }

        public void OnGotFocus()
        {
        }

        public Point Measure(Point availableSize)
        {
            if (Width != 0 && availableSize.X > Width)
            {
                availableSize.X = Width;
            }
            else if (MaxWidth != 0 && availableSize.X > MaxWidth)
            {
                availableSize.X = MaxWidth;
            }

            if (Height != 0 && availableSize.Y > Height)
            {
                availableSize.Y = Height;
            }
            else if (MaxHeight != 0 && availableSize.Y > MaxHeight)
            {
                availableSize.Y = MaxHeight;
            }

            availableSize.X -= MBPWidth;
            availableSize.Y -= MBPHeight;

            if (!_measureDirty && _lastMeasureAvailableSize == availableSize)
                return _lastMeasureSize;

            var result = InternalMeasure(availableSize);

            result.X += MBPWidth;
            result.Y += MBPHeight;

            if (Width != 0)
            {
                result.X = Width;
            }
            else if (MinWidth != 0 && result.X < MinWidth)
            {
                result.X = MinWidth;
            }
            else if (MaxWidth != 0 && result.X > MaxWidth)
            {
                result.X = MaxWidth;
            }

            if (Height != 0)
            {
                result.Y = Height;
            }
            else if (MinHeight != 0 && result.Y < MinHeight)
            { 
                result.Y = MinHeight; 
            }
            else if (MaxHeight != 0  && result.Y > MaxHeight)
            {
                result.Y = MaxHeight;
            }

            _lastMeasureSize = result;
            _lastMeasureAvailableSize = availableSize;
            _measureDirty = false;

            return result;
        }

        public void Arrange(Rectangle bounds)
        {
            if (!_arrangeDirty && _containerBounds == bounds)
                return;

            _arrangeDirty = true;
            _containerBounds = bounds;
            UpdateArrange();
        }

        public void UpdateArrange()
        {
            if (!_arrangeDirty)
                return;

            Point size;

            if (HorizontalAlignment != HorizontalAlignment.Stretch
                || VerticalAlignment != VerticalAlignment.Stretch)
            {
                size = Measure(_containerBounds.Size);
            }
            else
            {
                size = _containerBounds.Size;
            }

            if (size.X > _containerBounds.Width)
                size.X = _containerBounds.Width;

            if (size.Y > _containerBounds.Height)
                size.Y = _containerBounds.Height;

            var containerSize = _containerBounds.Size;
            if (HorizontalAlignment == HorizontalAlignment.Stretch && Width != 0 && Width < _containerBounds.X)
                containerSize.X = Width;

            if (VerticalAlignment == VerticalAlignment.Stretch && Height != 0 && Height < _containerBounds.Y)
                containerSize.Y = Height;

            var layoutBounds = LayoutUtils.Align(containerSize, size, HorizontalAlignment, VerticalAlignment);
            layoutBounds.Offset(_containerBounds.Location);

            _layoutBounds = layoutBounds;
            InvalidateTransform();

            InternalArrange();

            _arrangeDirty = false;
        }

        public void InvalidateArrange()
        {
            _arrangeDirty = true;
        }

        public virtual void InvalidateTransform()
        {
            _transformDirty = true;
        }

        public virtual void InvalidateMeasure()
        {
            _measureDirty = true;

            InvalidateArrange();

            if (Parent != null)
            {
                Parent.InvalidateMeasure();
            }
            else if (GuiSystem != null)
            {
                GuiSystem.InvalidateLayout();
            }
        }

        public bool ContainsPoint(Vector2 point)
        {
            //lock (_collisionTestLock)
            //{
            //    _collisionTest.Position = _layoutBounds.Location.ToVector2();
            //    _collisionTest.Size = _layoutBounds.Size;
            //    _collisionTest.Rotation = RotationWithParent;
            //    _collisionTest.OriginalCenter = Origin;
            //    _collisionTest.Scale = ScaleWithParent.X;

            //    return _collisionTest.ContainsPoint(point);
            //}

            // Todo: Need to invert matrix???
            point = Vector2.Transform(point, Transform);
            return _layoutBounds.Contains(point);
        }

        public virtual Control? HitTest(Vector2 point)
        {
            if (GuiSystem is null || !Visible || !ContainsPoint(point))
            {
                return null;
            }

            if (!InputFallsThrough(point))
                return this;

            return null;
        }

        public virtual bool InputFallsThrough(Vector2 point)
        {
            return false;
        }

        public void FireLocationChanged()
        {
            LocationChanged?.Invoke(this, EventArgs.Empty);
        }

        public void FireSizeChanged()
        {
            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnEnabledChanged()
        {
            EnabledChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnVisibleChanged()
        {
            InvalidateMeasure();
            VisibleChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMouseCursorChanged()
        {
        }

        protected virtual void OnPlacedChanged()
        {
            PlacedChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFocusChanged()
        {

        }

        protected virtual Point InternalMeasure(Point size)
        {
            return Point.Zero;
        }

        protected virtual void InternalArrange()
        {

        }

        protected virtual void InternalDraw(SpriteBatchState state)
        {

        }

        protected virtual void InternalUpdate(float delta)
        {

        }

        protected virtual void OnGuiSystemChanged()
        {
        }

        protected virtual void OnIdChanged()
        {
        }

        public virtual void OnAttachedPropertyChanged(AttachedProperty property)
        {
        }
    }
}
