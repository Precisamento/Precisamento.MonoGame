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

        public DialogueOptionRenderLocation OptionRenderLocation { get; set; }
        public Size OptionBoxMaxBounds { get; set; }
        public Size OptionBoxMinBounds { get; set; }
        public Point OptionBoxOffset { get; set; }
        public Thickness OptionBoxPadding { get; set; }
        public bool AlwaysUseOptionBoxMaxBounds { get; set; }
        public TextureRegion2D? OptionBoxWindowBackground { get; set; }
        public SpriteAnimation? OptionBackground { get; set; }
        public Thickness OptionBackgroundPadding { get; set; }
        public SpriteAnimation? OptionSelectIcon { get; set; }
        public SelectIconLocation OptionSelectIconLocation { get; set; }
        public Point OptionSelectIconOffset { get; set; }
        public bool DisplayDialogueBoxWhileOptionsAreShowing { get; set; } = true;
        public Func<int?>? OptionQuickSelect { get; set; }
        public Func<int>? OptionMoveSelection { get; set; }
        public Func<bool>? OptionSelected { get; set; }
        public Func<bool>? OptionCanceled { get; set; }
        public int OptionMargin { get; set; }
        public bool OptionAllowScrollWrap { get; set; }
        public float OptionAutoScrollInitialWait { get; set; }
        public float OptionAutoScrollSecondaryWait { get; set; }
        public bool OptionScrollKeepSelectionCentered { get; set; }
        public bool OptionAllowMouseSelect { get; set; }
        public bool IsOptionWindow { get; set; }

        public Dictionary<string, CharacterProfile>? Characters { get; set; }
        public PortraitBounds? DefaultCharacterPortraitBounds { get; set; }
        public CharacterLocation? DefaultCharacterLocation { get; set; }

        public object Clone()
        {
            var other = new DialogueBoxOptions();
            other.ContinuePressed = ContinuePressed;
            other.FastForwardPressed = FastForwardPressed;
            other.Dismiss = Dismiss;
            other.Scroll = Scroll;
            other.TextSpeed = TextSpeed;
            other.Background = Background;
            other.Bounds = Bounds;
            other.Padding = Padding;
            other.Font = Font;
            other.TextColor = TextColor;
            other.SentenceSplitter = SentenceSplitter;
            other.OptionRenderLocation = OptionRenderLocation;
            other.OptionBoxMaxBounds = OptionBoxMaxBounds;
            other.OptionBoxMinBounds = OptionBoxMinBounds;
            other.OptionBoxOffset = OptionBoxOffset;
            other.OptionBoxPadding = OptionBoxPadding;
            other.AlwaysUseOptionBoxMaxBounds = AlwaysUseOptionBoxMaxBounds;
            other.OptionBoxWindowBackground = OptionBoxWindowBackground;
            other.OptionBackground = OptionBackground;
            other.OptionBackgroundPadding = OptionBackgroundPadding;
            other.OptionSelectIcon = OptionSelectIcon;
            other.OptionSelectIconLocation = OptionSelectIconLocation;
            other.OptionSelectIconOffset = OptionSelectIconOffset;
            other.DisplayDialogueBoxWhileOptionsAreShowing = DisplayDialogueBoxWhileOptionsAreShowing;
            other.OptionQuickSelect = OptionQuickSelect;
            other.OptionMoveSelection = OptionMoveSelection;
            other.OptionSelected = OptionSelected;
            other.OptionCanceled = OptionCanceled;
            other.OptionMargin = OptionMargin;
            other.OptionAllowScrollWrap = OptionAllowScrollWrap;
            other.OptionAutoScrollInitialWait = OptionAutoScrollInitialWait;
            other.OptionAutoScrollSecondaryWait = OptionAutoScrollSecondaryWait;
            other.OptionScrollKeepSelectionCentered = OptionScrollKeepSelectionCentered;
            other.OptionAllowMouseSelect = OptionAllowMouseSelect;
            other.IsOptionWindow = IsOptionWindow;
            other.Characters = Characters;

            return other;
        }
    }
}
