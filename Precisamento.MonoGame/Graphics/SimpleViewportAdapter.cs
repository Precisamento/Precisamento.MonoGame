using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Graphics
{
    public class SimpleViewportAdapter : ViewportAdapter
    {
        public SimpleViewportAdapter(GraphicsDevice graphicsDevice, int width, int height)
            : base(graphicsDevice)
        {
            VirtualWidth = width;
            VirtualHeight = height;
            ViewportWidth = width;
            ViewportHeight = height;
        }

        public SimpleViewportAdapter(GraphicsDevice graphicsDevice, int viewWidth, int viewHeight, int viewportWidth, int viewportHeight)
            : base(graphicsDevice)
        {
            VirtualWidth = viewWidth;
            VirtualHeight = viewHeight;
            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;
        }

        public override int VirtualWidth { get; }

        public override int VirtualHeight { get; }

        public override int ViewportWidth { get; }

        public override int ViewportHeight { get; }

        public override Matrix GetScaleMatrix()
        {
            float xScale = (float)ViewportWidth / (float)VirtualWidth;
            float yScale = (float)ViewportHeight / (float)VirtualHeight;
            return Matrix.CreateScale(xScale, yScale, 1f);
        }
    }
}
