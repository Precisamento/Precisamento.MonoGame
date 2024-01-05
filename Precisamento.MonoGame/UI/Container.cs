using Microsoft.Xna.Framework;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public class Container : Control
    {
        private bool _childrenDirty = true;

        public ObservableCollection<Control> Children { get; } = new ObservableCollection<Control>();

        public virtual SizeFlags AllowedSizeFlagsHorizontal => SizeFlags.All;
        public virtual SizeFlags AllowedSizeFlagsVertical => SizeFlags.All;

        public Container()
        {
            SetMouseFilter(MouseFilter.Pass);
            Children.CollectionChanged += ChildrenChanged;
        }

        private void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(Control child in e.NewItems ?? Array.Empty<Control>())
                    {
                        ChildAdded(child);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach(Control child in e.OldItems ?? Array.Empty<Control>())
                    {
                        ChildRemoved(child);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // Todo: Call ChildRemoved for each obj
                    break;
            }

            InvalidateLayout();
            _childrenDirty = true;
        }

        public void FitChildInRect(Control child, Rectangle rect)
        {
            var rtl = IsRtl();
            var minSize = child.GetCombinedMinimumSize();
            var result = rect;

            var hFlags = child.GetHorizontalSizeFlags();
            var vFlags = child.GetVerticalSizeFlags();

            if (!hFlags.HasFlag(SizeFlags.Fill))
            {
                result.Width = minSize.X;
                if (hFlags.HasFlag(SizeFlags.ShrinkEnd))
                {
                    result.X += rtl ? 0 : (rect.Width - minSize.X);
                }
                else if (hFlags.HasFlag(SizeFlags.ShrinkCenter))
                {
                    result.X += MathExt.FloorToInt((rect.Width - minSize.X) / 2f);
                }
                else
                {
                    result.X += rtl ? (rect.Width - minSize.X) : 0;
                }
            }

            if (!vFlags.HasFlag(SizeFlags.Fill))
            {
                result.Height = minSize.Y;
                if (vFlags.HasFlag(SizeFlags.ShrinkEnd))
                {
                    result.Y += rect.Height - minSize.Y;
                }
                else if (vFlags.HasFlag(SizeFlags.ShrinkCenter))
                {
                    result.Y += MathExt.FloorToInt((rect.Height - minSize.Y) / 2f);
                }
            }

            child.SetRect(result);
            child.SetRotation(0);
            child.SetScale(Vector2.One);
        }

        protected virtual void ChildAdded(Control child)
        {
            InvalidateLayout();
            child.SetParent(this);
        }

        protected virtual void ChildMoved(Control child)
        {
            InvalidateLayout();
        }

        protected virtual void ChildRemoved(Control child)
        {
            InvalidateLayout();
            child.RemoveParent();
        }
    }
}
