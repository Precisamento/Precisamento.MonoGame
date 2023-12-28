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

    public class CharacterDisplayOptions : ICloneable
    {
        public Dictionary<string, CharacterProfile>? Characters { get; set; }
        public ICharacterProcessorFactory? CharacterFactory { get; set; }
        public PortraitBounds? DefaultPortraitBounds { get; set; }
        public int MaxCharacters { get; set; } = 9999;
        public int MaxCharactersInAGivenLocation { get; set; } = 9999;
        public CharacterAddBehavior AddBehavior { get; set; } = CharacterAddBehavior.IgnoreUnlessExplicitlySet;
        public CharacterSpeakerBehavior SpeakerBehavior { get; set; } = CharacterSpeakerBehavior.MostRecentOnly;
        public Color NonSpeakerColor { get; set; } = Color.White;
        public DarkenNonSpeaker DarkenNonSpeakerBehavior { get; set; } =
            DarkenNonSpeaker.Both;
        public bool MoveSpeakerToFront { get; set; }
        public bool LeftIsFront { get; set; }
        public Point MultipleSpeakerOffset { get; set; }
        public List<CharacterLocation> DrawOffsets { get; set; } = new();
        public CharacterLocation? DefaultLocation { get; set; }
        public bool FlipFacesOnRight { get; set; }
        public bool FlipBackgroundsOnRight { get; set; }
        public float MoveTime { get; set; } = 0.5f;

        public object Clone() => MemberwiseClone();

        public void SetDefaults()
        {
            // If no character factory has been set, there is no reason to set further defaults
            // since they won't be used.
            if (CharacterFactory is null)
                return;

            DefaultPortraitBounds ??= new PortraitBounds()
            {
                BoundsType = PortraitBoundsType.SpriteSize
            };

            DefaultLocation ??= new CharacterLocation(DialogueOptionRenderLocation.AboveLeft);
        }
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
        public CharacterDisplayOptions CharacterOptions { get; set; } = new CharacterDisplayOptions();

        public object Clone()
        {
            var other = (DialogueBoxOptions)MemberwiseClone();
            other.OptionBoxOptions = (OptionBoxOptions)OptionBoxOptions.Clone();
            other.CharacterOptions = (CharacterDisplayOptions)CharacterOptions.Clone();

            return other;
        }
    }
}
