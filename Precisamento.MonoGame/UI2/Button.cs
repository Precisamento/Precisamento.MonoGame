using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2
{
    public class Button : ContentControl
    {
        private bool _isPressed;
        private bool _isClicked;

        public Button()
        {
            ContentLayout = new SingleItemLayout();
        }
    }
}
