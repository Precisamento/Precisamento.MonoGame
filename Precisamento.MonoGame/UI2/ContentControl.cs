using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public abstract class ContentControl : ParentControl
    {
        private Control? _content;

        public virtual Control? Content
        {
            get => _content;
            set
            {
                if (value == _content)
                    return;

                if (value != null)
                    ValidateContent(value);

                if (_content != null)
                    RemoveContent(_content);

                _content = value;

                if (_content != null)
                    SetContent(_content);

                InvalidateMeasure();
            }
        }

        public override IEnumerable<Control> Children
        {
            get
            {
                if (Content is null)
                    return Array.Empty<Control>();

                return new[] { Content };
            }
        }

        public IContentLayout? ContentLayout { get; set; }

        protected virtual void SetContent(Control control)
        {
            control.GuiSystem = GuiSystem;
            control.Parent = this;
        }

        protected virtual void RemoveContent(Control control)
        {
            control.GuiSystem = null;
            control.Parent = null;
        }

        public override void OnEnabledChanged()
        {
            if (Content != null)
                Content.Enabled = Enabled;

            base.OnEnabledChanged();
        }

        protected override void OnMouseCursorChanged()
        {
            if (Content != null)
                Content.MouseCursor = MouseCursor;
        }

        protected override void OnGuiSystemChanged()
        {
            if (Content != null)
                Content.GuiSystem = GuiSystem;
        }

        protected override void InternalUpdate(float delta)
        {
            Content?.Update(delta);
        }

        protected override void InternalDraw(SpriteBatchState state)
        {
            Content?.Draw(state);
        }

        protected override void InternalArrange()
        {
            if (ContentLayout is null || Content is null)
                return;

            ContentLayout.Arrange(Content, ActualBounds);
        }

        protected override Point InternalMeasure(Point size)
        {
            if (ContentLayout is null || Content is null)
                return Point.Zero;

            return ContentLayout.Measure(Content, size);
        }

        protected virtual void ValidateContent(Control control) { }
    }
}
