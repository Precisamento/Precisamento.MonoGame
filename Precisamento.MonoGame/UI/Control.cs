using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
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

        private void ComputeOffsets(Rectangle rect, Anchor anchor, out Margin margin)
        {
            throw new NotImplementedException();
        }

        private void ComputeAnchors(Rectangle rect, Margin margin, out Anchor anchor)
        {
            throw new NotImplementedException();
        }

        private void SetLayoutMode(LayoutMode layoutMode)
        {
            throw new NotImplementedException();
        }

        private void UpdateLayoutMode(LayoutMode layoutMode)
        {
            throw new NotImplementedException();
        }

        private LayoutMode GetLayoutMode()
        {
            throw new NotImplementedException();
        }

        private LayoutMode GetDefaultLayoutMode()
        {
            throw new NotImplementedException();
        }

        private void SetAnchorsLayoutPreset(int preset)
        {
            throw new NotImplementedException();
        }

        private int GetAnchorsLayoutPreset()
        {
            throw new NotImplementedException();
        }

        private void UpdateMinimumSizeCache()
        {
            throw new NotImplementedException();
        }

        private void UpdateMinimumSize()
        {
            throw new NotImplementedException();
        }

        private void SizeChanged()
        {
            throw new NotImplementedException();
        }

        private void ClearSizeWarning()
        {
            throw new NotImplementedException();
        }

        private void CallGuiEvent(InputEvent inputEvent)
        {
            throw new NotImplementedException();
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

        protected virtual Vector2 GetMinimumSize()
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

        protected virtual object MakeCustomTooltip(string text)
        {
            throw new NotImplementedException();
        }

        protected virtual void GuiInput(InputEvent inputEvent)
        {
            throw new NotImplementedException();
        }

        protected virtual void Reparent(Control parent, bool keepGlobalTransform)
        {
            throw new NotImplementedException();
        }
    }
}
