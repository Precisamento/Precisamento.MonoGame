using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Resources.Sprites
{
    public class SpriteDescription
    {
        public string Name { get; set; } = "";
        public List<SpriteAnimationDescription> Animations { get; set; } = new List<SpriteAnimationDescription>();
    }

    public class SpriteAnimationDescription
    {
        public string Name { get; set; } = "";
        public SpriteUpdateMode UpdateMode { get; set; }
        public Vector2 Origin { get; set; }
        public float FramesPerSecond { get; set; }
        public int StartingFrame { get; set; }
        public List<SpriteFrameDescription> Frames { get; set; } = new List<SpriteFrameDescription>();
    }

    public class SpriteFrameDescription
    {
        public bool UsesRegionNames { get; set; }
        public string TextureName { get; set; } = "";
        public string RegionName { get; set; } = "";
        public Rectangle Bounds { get; set; }
        public Thickness Thickness { get; set; }
    }
}
