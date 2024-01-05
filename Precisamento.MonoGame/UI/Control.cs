using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public partial class Control
    {
        private bool _initialized;

        protected internal Container? _parent;
        private GameWindow? _window;
        private bool _inTree;
        private bool _visible = true;


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

        private bool _layoutDirty = true;

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

        public bool Visible => _visible;

        private void QueueRedraw()
        {

        }

        private void NotifyPropertyListChanged()
        {

        }

        private void NotifyTransform()
        {

        }

        protected virtual void PostInitialize()
        {
            _initialized = true;

            InvalidateThemeCache();
            UpdateThemeItemCache();
        }

        public virtual void SetParent(Container parent)
        {
            _parent = parent;
            GuiSystem = parent.GuiSystem;

            // _themeOwner.AssignThemeOnParented(this);

            UpdateLayoutMode();
        }
        
        public virtual void RemoveParent()
        {
            _parent = null;
            GuiSystem = null;

            // _themeOwner.ClearThemeOnUnparented(this);
        }

        public bool IsAncestorOf(Control control)
        {
            while (control._parent != null)
            {
                if (control._parent == this)
                    return true;

                control = control._parent;
            }

            return false;
        }

        private bool VisibleInTree()
        {
            return InTree && _visible;
        }

        private void UpdateConfigurationWarnings()
        {

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

        private string GetTooltipText()
        {
            return _tooltipText;
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

        public virtual void Reparent(Container? parent, bool keepGlobalTransform)
        {
            GuiSystem?.ThreadGuard();

            if (parent == _parent)
                return;

            _parent?.Children.Remove(this);
            parent?.Children.Add(this);

            //if (keepGlobalTransform)
            //    SetPosition();
        }

        public virtual bool IsTextField()
        {
            return false;
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

        public virtual void GuiInput(InputEvent inputEvent)
        {
            throw new NotImplementedException();
        }

        public void AcceptEvent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines if the control contains the point that is relative to the control position.
        /// </summary>
        public virtual bool HasPoint(Point point)
        {
            return new Rectangle(Point.Zero, GetSize()).Contains(point);
        }

        public void SetMouseFilter(MouseFilter filter) 
        {
            GuiSystem?.ThreadGuard();

            if (_mouseFilter == filter)
                return;

            GuiSystem?.UpdateGuiMouseOver();
        }

        public MouseFilter GetMouseFilter()
        {
            return _mouseFilter;
        }

        public void SetForcePassScrollEvents(bool forcePassScrollEvents)
        {
            GuiSystem?.ThreadGuard();

            _forcePassScrollEvents = forcePassScrollEvents;
        }

        public bool GetForcePassScrollEvents()
        {
            return _forcePassScrollEvents;
        }

        /// <summary>
        /// Warps the mouse relative to this controls position.
        /// </summary>
        /// <param name="position"></param>
        /// <exception cref="InvalidOperationException">Thrown if the control is not in the world yet.</exception>
        public void WarpMouse(Point position)
        {
            if (!InTree)
                throw new InvalidOperationException();

            var relativePosition = GetPosition();

            GuiSystem!.ThreadGuard();
            GuiSystem!.WarpMouse(relativePosition + position);
        }

        public bool IsFocusOwnerInShortcutContext()
        {
            if (_shortcutContext is null)
                return true;

            var focusedControl = GuiSystem?.GetFocusedControl();
            if (focusedControl is null)
                return false;

            return focusedControl == _shortcutContext || _shortcutContext.IsAncestorOf(focusedControl);
        }

        public void SetShortcutContext(Control? context)
        {
            GuiSystem?.ThreadGuard();

            _shortcutContext = context;
        }

        public Control? GetShortcutContext()
        {
            return _shortcutContext;
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

        public void SetDefaultCursorShape(CursorShape shape)
        {
            GuiSystem?.ThreadGuard();

            if (_defaultCursorShape == shape)
                return;

            _defaultCursorShape = shape;

            if (!InTree)
                return;

            if (!GetScreenRect().Contains(GuiSystem!.Input.MousePosition))
            {
                return;
            }

            GuiSystem!.UpdateMouseCursorState();
        }

        public CursorShape GetDefaultCursorShape()
        {
            return _defaultCursorShape;
        }

        public virtual CursorShape GetCursorShape()
        {
            return _defaultCursorShape;
        }

        public void SetClipContents(bool clip)
        {
            GuiSystem?.ThreadGuard();

            if (_clipContents == clip)
                return;

            _clipContents = clip;
            QueueRedraw();
        }

        public bool GetClipContents()
        {
            return _clipContents;
        }

        public void SetDisableVisibilityClip(bool ignore)
        {
            GuiSystem?.ThreadGuard();

            if (_disableVisibilityClip = ignore)
                return;

            _disableVisibilityClip = ignore;
            QueueRedraw();
        }

        public bool GetDisableVisibilityClip()
        {
            return _disableVisibilityClip;
        }

        public void SetLayoutDirection(LayoutDirection direction)
        {
            GuiSystem?.ThreadGuard();

            if (_layoutDirection == direction)
                return;

            _layoutDirection = direction;
            _isRtlDirty = true;

            LayoutDirectionChanged();
        }

        public LayoutDirection GetLayoutDirection()
        {
            return _layoutDirection;
        }

        public virtual bool IsRtl()
        {
            return _isRtl;
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
            // Todo: Translation

            return str;
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
