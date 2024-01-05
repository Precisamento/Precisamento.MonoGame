using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public class Container : ParentControl
    {
        private bool _childrenDirty = true;
        private List<Control> _childrenCopy = new List<Control>();

        public virtual ObservableCollection<Control> ChildrenList { get; } = new ObservableCollection<Control>();

        public override IEnumerable<Control> Children => ChildrenList;

        public IContainerLayout? ChildrenLayout { get; set; }

        protected IEnumerable<Control> ChildrenCopy
        {
            get
            {
                UpdateChildren();
                return _childrenCopy;
            }
        }

        public Container()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            ChildrenList.CollectionChanged += ChildrenChanged;
        }

        private void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(Control child in args.NewItems!)
                    {
                        OnChildAdded(child);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Control child in args.OldItems!)
                    {
                        OnChildRemoved(child);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach(Control child in ChildrenCopy)
                    {
                        OnChildRemoved(child);
                    }
                    break;
            }

            InvalidateChildren();
        }

        protected virtual void OnChildAdded(Control child)
        {
            child.GuiSystem = GuiSystem;
            child.Parent = this;
        }

        protected virtual void OnChildRemoved(Control child)
        {
            child.GuiSystem = null;
            child.Parent = null;
        }

        private void UpdateChildren()
        {
            if (!_childrenDirty)
                return;

            _childrenCopy.Clear();

            _childrenCopy.AddRange(ChildrenList);

            SortingHelper.SortControlsByZIndex(_childrenCopy);

            _childrenDirty = false;
        }

        private void InvalidateChildren()
        {
            InvalidateMeasure();
            _childrenDirty = true;
        }

        public override void OnEnabledChanged()
        {
            foreach(var child in ChildrenCopy)
            {
                child.Enabled = Enabled;
            }

            base.OnEnabledChanged();
        }

        protected override void OnMouseCursorChanged()
        {
            foreach (var child in ChildrenCopy)
            {
                child.MouseCursor = MouseCursor;
            }
        }

        protected override void OnGuiSystemChanged()
        {
            foreach (var child in ChildrenCopy)
            {
                child.GuiSystem = GuiSystem;
            }
        }

        protected override void InternalUpdate(float delta)
        {
            foreach(var child in ChildrenCopy)
            {
                child.Update(delta);
            }
        }

        protected override void InternalDraw(SpriteBatchState state)
        {
            foreach(var child in ChildrenCopy)
            {
                child.Draw(state);
            }
        }

        protected override void InternalArrange()
        {
            ChildrenLayout?.Arrange(ChildrenCopy, ActualBounds);
        }

        protected override Point InternalMeasure(Point size)
        {
            return ChildrenLayout?.Measure(ChildrenCopy, size) ?? Point.Zero;
        }

        public override void InvalidateTransform()
        {
            base.InvalidateTransform();

            foreach(var child in ChildrenCopy)
            {
                child.InvalidateTransform();
            }
        }
    }
}
