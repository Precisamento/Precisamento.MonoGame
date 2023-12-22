using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Dialogue.Characters;
using Precisamento.MonoGame.Dialogue.Options;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Dialogue
{
    public class OptionBoxOptions : ICloneable
    {
        public DialogueOptionRenderLocation RenderLocation { get; set; }
        public Size MaxBounds { get; set; }
        public Size MinBounds { get; set; }
        public Point Offset { get; set; }
        public Thickness Padding { get; set; }
        public bool AlwaysUseMaxBounds { get; set; }
        public TextureRegion2D? WindowBackground { get; set; }
        public SpriteAnimation? OptionBackground { get; set; }
        public Thickness OptionBackgroundPadding { get; set; }
        public SpriteAnimation? SelectIcon { get; set; }
        public SelectIconLocation SelectIconLocation { get; set; }
        public Point SelectIconOffset { get; set; }
        public bool AddSelectedOptionAsLine { get; set; }
        public bool RemoveIgnoredOptionsOnSelect { get; set; }
        public Func<int?>? QuickSelect { get; set; }
        public Func<int>? MoveSelection { get; set; }
        public Func<bool>? Selected { get; set; }
        public Func<bool>? Canceled { get; set; }
        public int OptionMargin { get; set; }
        public bool AllowScrollWrap { get; set; }
        public float AutoScrollInitialWait { get; set; }
        public float AutoScrollSecondaryWait { get; set; }
        public bool ScrollKeepSelectionCentered { get; set; }
        public bool AllowMouseSelect { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class ProfileDisplayOptions : ICloneable
    {
        public Dictionary<string, CharacterProfile>? Characters { get; set; }
        public PortraitBounds? DefaultPortraitBounds { get; set; }
        public CharacterLocation? DefaultCharacterLocation { get; set; }

        public object Clone() => MemberwiseClone();
    }

    public class DialogueBoxOptions : ICloneable
    {
        public Func<bool> ContinuePressed { get; set; }
        public Func<bool>? FastForwardPressed { get; set; }
        public Action? Dismiss { get; set; }
        public Func<int>? Scroll { get; set; }
        public int TextSpeed { get; set; }
        public TextureRegion2D? Background { get; set; }
        public Rectangle Bounds { get; set; }
        public Thickness Padding { get; set; }
        public IFont Font { get; set; }
        public Color TextColor { get; set; }
        public ISentenceSplitter SentenceSplitter { get; set; }
        public bool DisplayDialogueBoxWhileOptionsAreShowing { get; set; } = true;
        public bool IsOptionWindow { get; set; }
        public OptionBoxOptions OptionBoxOptions { get; set; } = new OptionBoxOptions();
        public ProfileDisplayOptions ProfileOptions { get; set; } = new ProfileDisplayOptions();

        public object Clone()
        {
            var other = (DialogueBoxOptions)MemberwiseClone();
            other.OptionBoxOptions = (OptionBoxOptions)OptionBoxOptions.Clone();
            other.ProfileOptions = (ProfileDisplayOptions)ProfileOptions.Clone();

            return other;
        }
    }
}
