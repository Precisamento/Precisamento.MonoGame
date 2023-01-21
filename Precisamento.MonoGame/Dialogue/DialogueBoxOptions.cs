using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueBoxOptions
    {
        public Func<bool> ContinuePressed { get; set; }
        public Func<bool> FastForwardPressed { get; set; }
        public Action Dismiss { get; set; }
        public Func<int> Scroll { get; set; }
        public int TextSpeed { get; set; }
        public TextureRegion2D Background { get; set; }
        public Rectangle Bounds { get; set; }
        public Thickness Padding { get; set; }
        public IFont Font { get; set; }
        public Color TextColor { get; set; }
        public ISentenceSplitter SentenceSplitter { get; set; }
    }
}
