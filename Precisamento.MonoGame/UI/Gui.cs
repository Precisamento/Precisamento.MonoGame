using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI
{
    public class Gui
    {
        public int ThreadId { get; set; }
        public Camera<Vector2> Camera { get; set; }
        public InputManager Input { get; }

        public Rectangle GetVisibleRect() => throw new NotImplementedException();
        public void ThreadGuard() => throw new NotImplementedException();

        public void UpdateGuiMouseOver()
        {
            throw new NotImplementedException();
        }

        public void WarpMouse(Point point)
        {
            throw new NotImplementedException();
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
    }
}
