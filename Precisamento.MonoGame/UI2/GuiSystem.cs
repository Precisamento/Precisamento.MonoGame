using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.Scenes;
using Precisamento.MonoGame.UI2.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public class GuiSystem
    {
        private Camera<Vector2> _camera;
        private InputManager _input;
        private IActionManager? _actions;

        private bool _layoutDirty = true;
        private bool _controlsDirty = true;

        private Control? _previouslyFocusedControl;
        private Control? _focusedControl;

        private bool _darkenControlsUnderModal = false;
        private Color _darkenControlColor = new Color(0, 0, 0, 50);

        private List<Control> _childrenCopy = new();

        public ObservableCollection<Control> Controls { get; } = new ObservableCollection<Control>();

        public Control? Root
        {
            get => Controls.FirstOrDefault();
            set
            {
                if (value == Root)
                    return;

                HideContextMenu();
                HideTooltip();
                Controls.Clear();
                if (value != null)
                {
                    Controls.Add(value);
                }
            }
        }

        public Rectangle Bounds
        {
            get => _camera.BoundingRectangle.ToRectangle();
        }

        public Rectangle LayoutBounds
        {
            get => new Rectangle(Point.Zero, Bounds.Size);
        }

        public Control? ContextMenu { get; set; }
        public Control? Tooltip { get; set; }

        public Control? FocusedControl
        {
            get => _focusedControl;
            set
            {
                if (value == _focusedControl) 
                    return;

                var oldControl = _focusedControl;
                if (oldControl != null)
                {
                    var losingFocus = new ControlLosingFocusEventArgs(oldControl);

                    ControlLosingFocus?.Invoke(this, losingFocus);
                    if (oldControl.IsPlaced && losingFocus.Cancel)
                    {
                        return;
                    }
                }

                _focusedControl = value;

                oldControl?.OnLostFocus();

                if (_focusedControl != null)
                {
                    _focusedControl.OnGotFocus();
                    ControlGotFocus?.Invoke(this, new ControlGotFocusEventArgs(_focusedControl));
                }
            }
        }

        public float Opacity { get; set; } = 1;

        public float Scale
        {
            get => _camera.Zoom;
            set
            {
                if (value == _camera.Zoom)
                    return;

                _camera.Zoom = value;
                InvalidateTransform();
            }
        }

        public Vector2 Origin
        {
            get => _camera.Origin;
            set
            {
                if (value == _camera.Origin)
                    return;

                _camera.Origin = value;
                InvalidateTransform();
            }
        }

        public float Rotation
        {
            get => _camera.Rotation;
            set
            {
                if (value == _camera.Rotation)
                    return;

                _camera.Rotation = value;
                InvalidateTransform();
            }
        }

        public Matrix Transform => _camera.GetViewMatrix();

        public bool IsMouseOverGui
        {
            get => IsScreenPointOverGui(_input.MousePosition.ToVector2());
        }

        public bool IsTouchOverGui
        {
            get
            {
                foreach(TouchLocation touch in _input.TouchCurrent)
                {
                    if (touch.State != TouchLocationState.Released)
                    {
                        if (IsScreenPointOverGui(touch.Position))
                            return true;
                    }
                }

                return false;
            }
        }

        public InputManager Input { get; }

        public bool IsShiftDown => _input.KeyCheck(Keys.LeftShift) || _input.KeyCheck(Keys.RightShift);

        public bool IsControlDown => _input.KeyCheck(Keys.LeftControl) || _input.KeyCheck(Keys.RightControl);

        public bool IsAltDown => _input.KeyCheck(Keys.LeftAlt) || _input.KeyCheck(Keys.RightAlt);

        public bool HasModal
        {
            get
            {
                var copy = ChildrenCopy;
                return copy.Any(c => c.Visible && c.Enabled && c.IsModal);
            }
        }

        public bool DebugDraw { get; set; }

        public event EventHandler<ContextMenuClosingEventArgs>? ContextMenuClosing;

        public event EventHandler<ContextMenuClosedEventArgs>? ContextMenuClosed;

        public event EventHandler<ControlLosingFocusEventArgs>? ControlLosingFocus;

        public event EventHandler<ControlGotFocusEventArgs>? ControlGotFocus;

        private List<Control> ChildrenCopy
        {
            get
            {
                UpdateChildrenCopy();
                return _childrenCopy;
            }
        }

        public GuiSystem (Game game, InputManager input, Camera<Vector2> camera)
        {
            _input = input;
            _camera = camera;

            Controls.CollectionChanged += ControlsChanged;
        }

        public void Update(float delta)
        {
            UpdateLayout();

            UpdateInput();


            // Do another layout run, since an input event could cause the layout change
            UpdateLayout();
        }

        private void UpdateInput()
        {
        }

        public void Draw(SpriteBatchState state)
        {
            //state.TransformMatrix = _camera.GetViewMatrix();

            //state.Begin();

            foreach(var child in ChildrenCopy)
            {
                if (child.Visible)
                {
                    if (child.IsModal && _darkenControlsUnderModal)
                    {
                        state.SpriteBatch.FillRectangle(LayoutBounds, _darkenControlColor);
                    }

                    child.Draw(state);
                }
            }

            //state.TransformMatrix = null;
        }

        public void HideContextMenu()
        {
            if (ContextMenu is null)
                return;

            Controls.Remove(ContextMenu);
            ContextMenu.Visible = false;

            ContextMenuClosed?.Invoke(this, new ContextMenuClosedEventArgs(ContextMenu));

            ContextMenu = null;
            if (_previouslyFocusedControl != null)
            {
                FocusedControl = _previouslyFocusedControl;
                _previouslyFocusedControl = null;
            }
        }

        public void ShowContextMenu(Control menu, Point position)
        {
            HideContextMenu();

            ContextMenu = menu;
            if (menu is null)
                return;

            FixOverControlPosition(menu, position);

            ContextMenu.Visible = true;
            Controls.Add(menu);

            if (ContextMenu.AcceptsFocus)
            {
                _previouslyFocusedControl = FocusedControl;
                FocusedControl = ContextMenu;
            }
        }

        public void HideTooltip()
        {
            if (Tooltip is null)
                return;

            Controls.Remove(Tooltip);
            Tooltip.Visible = false;
            Tooltip = null;
        }

        public void ShowTooltip(Control control, Point position)
        {
            if (string.IsNullOrEmpty(control.Tooltip))
                return;

            HideTooltip();

            throw new NotImplementedException();

            //  Todo: Tooltip = CreateTooltipHere()

            FixOverControlPosition(Tooltip, position);

            Tooltip.Visible = true;
            Controls.Add(Tooltip);
        }

        public bool IsScreenPointOverGui(Vector2 point)
        {
            var worldPos = _camera.ScreenToWorld(point);
            return IsPointOverGui(worldPos);
        }

        public bool IsPointOverGui(Vector2 point)
        {
            foreach (var control in ChildrenCopy)
            {
                var result = control.HitTest(point);
                if (result != null)
                    return true;
            }

            return false;
        }

        private void OnTouchDown()
        {
            if (ContextMenu?.IsTouchInside ?? false)
            {
                var menuClosing = new ContextMenuClosingEventArgs(ContextMenu);
                ContextMenuClosing?.Invoke(this, menuClosing);

                if (menuClosing.Cancel)
                    return;

                HideContextMenu();
            }

            HideTooltip();
        }

        private void UpdateChildrenCopy()
        {
            if (!_controlsDirty)
                return;

            _childrenCopy.Clear();
            _childrenCopy.AddRange(Controls);

            // Todo: Sort controls by z-index

            _controlsDirty = false;
        }

        private void InvalidateTransform()
        {
            foreach(var child in ChildrenCopy)
            {
                child.InvalidateTransform();
            }
        }

        public void InvalidateLayout()
        {
            _layoutDirty = true;
        }

        public void UpdateLayout()
        {
            if (!_layoutDirty)
                return;

            var layout = LayoutBounds;

            foreach(var child in ChildrenCopy)
            {
                if (child.Visible)
                {
                    child.Arrange(layout);
                }
            }

            _layoutDirty = false;
        }

        private void ControlsChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            switch(args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(Control control in args.NewItems!)
                    {
                        control.GuiSystem = this;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach(Control control in args.OldItems)
                    {
                        control.GuiSystem = null;
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach(var child in ChildrenCopy)
                    {
                        child.GuiSystem = null;
                    }
                    break;
            }

            InvalidateLayout();
            _controlsDirty = true;
        }

        private void FixOverControlPosition(Control control, Point position)
        {
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.VerticalAlignment = VerticalAlignment.Top;

            var bounds = LayoutBounds;

            var measure = control.Measure(bounds.Size);

            if (position.X + measure.X > bounds.Right)
            {
                position.X = bounds.Right - measure.X;
            }

            if (position.Y + measure.Y > bounds.Bottom)
            {
                position.Y = bounds.Bottom - measure.Y;
            }

            control.Left = position.X;
            control.Top = position.Y;
        }
    }
}
