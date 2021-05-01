using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Input
{
    public static class MouseStateExtensions
    {
        public static bool IsButtonDown(this MouseState state, MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left: return state.LeftButton == ButtonState.Pressed;
                case MouseButtons.Middle: return state.MiddleButton == ButtonState.Pressed;
                case MouseButtons.Right: return state.RightButton == ButtonState.Pressed;
                case MouseButtons.X1: return state.XButton1 == ButtonState.Pressed;
                case MouseButtons.X2: return state.XButton2 == ButtonState.Pressed;
                default: return false;
            }
        }

        public static bool IsButtonUp(this MouseState state, MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left: return state.LeftButton == ButtonState.Released;
                case MouseButtons.Middle: return state.MiddleButton == ButtonState.Released;
                case MouseButtons.Right: return state.RightButton == ButtonState.Released;
                case MouseButtons.X1: return state.XButton1 == ButtonState.Released;
                case MouseButtons.X2: return state.XButton2 == ButtonState.Released;
                default: return false;
            }
        }
    }
}
