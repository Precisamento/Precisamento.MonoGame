using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Precisamento.MonoGame.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public class Gui
    {
        private bool _layoutDirty = true;
        private bool _controlsDirty = false;
        private List<Control> _controlsCopy = new List<Control>();

        public int ThreadId { get; }
        public Camera<Vector2> Camera { get; }
        public InputManager Input { get; }
        public Game Game { get; }
        public ObservableCollection<Control> Widgets { get; } = new ObservableCollection<Control>();
        public Control? FocusedControl { get; private set; }

        private List<Control> ControlsCopy
        {
            get
            {
                UpdateControlsCopy();
                return _controlsCopy;
            }
        }

        public Control? Root
        {
            get
            {
                if (Widgets.Count == 0)
                    return null;

                return Widgets[0];
            }
            set
            {
                if (value == Root)
                    return;

                Widgets.Clear();

                if (value != null)
                {
                    Widgets.Add(value);
                }
            }
        }

        public Gui(Camera<Vector2> camera, InputManager input, Game game)
            : this(camera, input, game, Environment.CurrentManagedThreadId)
        {
        }

        public Gui(Camera<Vector2> camera, InputManager input, Game game, int threadId)
        {
            Widgets.CollectionChanged += WidgetsCollectionChanged;
            Camera = camera;
            Input = input;
            Game = game;
            ThreadId = threadId;
        }

        public Rectangle GetVisibleRect()
        {
            return Camera.BoundingRectangle.ToRectangle();
        }

        public void ThreadGuard()
        {
            if (Environment.CurrentManagedThreadId != ThreadId)
                throw new GuiException("Can only update GUI properties on the UI thread");
        }

        public void UpdateGuiMouseOver()
        {
            throw new NotImplementedException();
        }

        public void WarpMouse(Point point)
        {
            Mouse.SetPosition(point.X, point.Y);
        }

        public Control? GetFocusedControl()
        {
            return null;
        }

        public void GrabFocus(Control? control)
        {
            throw new NotImplementedException();
        }

        public void GrabClickFocus(Control? control)
        {
            throw new NotImplementedException();
        }

        public void ReleaseFocus()
        {
            throw new NotImplementedException();
        }

        public void UpdateCameraPosition(Vector2 position)
        {
            Camera.Position = position;
            InvalidateLayout();
        }

        public void UpdateCameraRotation(float rotation)
        {
            Camera.Rotation = rotation;
            InvalidateLayout();
        }

        public void UpdateCameraZoom(float zoom)
        {
            Camera.Zoom = zoom;
            InvalidateLayout();
        }

        public void UpdateCameraOrigin(Vector2 origin)
        {
            Camera.Origin = origin;
            InvalidateLayout();
        }

        public void InvalidateLayout()
        {
            _layoutDirty = true;
        }

        public void UpdateLayout()
        {
            var bounds = GetVisibleRect();
            if (bounds.IsEmpty)
                return;

            if (!_layoutDirty)
                return;

            foreach(var child in ControlsCopy)
            {
                if (child.Visible)
                {
                    child.Arrange();
                }
            }

            _layoutDirty = false;
        }

        internal void UpdateMouseCursorState()
        {
        }

        private void WidgetsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(Control control in args.NewItems!)
                    {
                        control.GuiSystem = this;
                        if (control is Container container)
                        {
                            foreach(var child in container.Children)
                            {
                                child.GuiSystem = this;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach(Control control in args.OldItems!)
                    {
                        control.GuiSystem = null;
                        if (control is Container container)
                        {
                            foreach(var child in container.Children)
                            {
                                child.GuiSystem = null;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var control in ControlsCopy)
                    //foreach (Control control in args.OldItems!)
                    {
                        control.GuiSystem = null;
                        if (control is Container container)
                        {
                            foreach (var child in container.Children)
                            {
                                child.GuiSystem = null;
                            }
                        }
                    }
                    break;
            }

            InvalidateLayout();
            _controlsDirty = true;
        }

        private void UpdateControlsCopy()
        {
            if (!_controlsDirty)
                return;

            _controlsCopy.Clear();
            _controlsCopy.AddRange(Widgets);
        }
    }
}
