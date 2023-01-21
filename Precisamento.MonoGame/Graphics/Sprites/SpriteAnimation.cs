using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Graphics
{
    public class SpriteAnimation
    {
        public string Name { get; set; }
        public SpriteUpdateMode UpdateMode { get; set; }
        public float FramesPerSecond { get; set; }
        public Vector2 Origin { get; set; }
        public List<TextureRegion2D> Frames { get; set; }
        public int StartFrameIndex { get; set; }
    }
}
