using DefaultEcs;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Dialogue.Characters;
using Precisamento.MonoGame.Dialogue.Options;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueBoxBuilder
    {
        private DialogueBoxOptions _options = new DialogueBoxOptions();
        private Game _game;

        public DialogueBoxBuilder(Game game)
        {
            _game = game;
        }

        public DialogueBoxBuilder SetContinuePressed(Func<bool> continuedPressed)
        {
            _options.ContinuePressed = continuedPressed;
            _options.OptionBoxOptions.Selected = continuedPressed;
            return this;
        }

        public DialogueBoxBuilder SetContinuePressed(IActionManager actionManager, int action)
            => SetContinuePressed(() => actionManager.ActionCheckPressed(action));

        public DialogueBoxBuilder SetContinuePressed(IActionManager actionManager, int action, int player)
        {
            _options.ContinuePressed = () => actionManager.ActionCheckPressed(action, player);
            return this;
        }

        public DialogueBoxBuilder SetFastForwardPressed(Func<bool> continuedPressed)
        {
            _options.FastForwardPressed = continuedPressed;
            return this;
        }

        public DialogueBoxBuilder SetFastForwardPressed(IActionManager actionManager, int action)
        {
            _options.FastForwardPressed = () => actionManager.ActionCheckPressed(action);
            return this;
        }

        public DialogueBoxBuilder SetFastForwardPressed(IActionManager actionManager, int action, int player)
        {
            _options.FastForwardPressed = () => actionManager.ActionCheckPressed(action, player);
            return this;
        }

        public DialogueBoxBuilder SetDismiss(Action dismiss)
        {
            _options.Dismiss = dismiss;
            return this;
        }

        public DialogueBoxBuilder SetDismiss(Entity entity)
        {
            _options.Dismiss = () => entity.Dispose();
            return this;
        }

        public DialogueBoxBuilder SetScroll(Func<int> scroll)
        {
            _options.Scroll = scroll;
            return this;
        }

        public DialogueBoxBuilder SetTextSpeed(int textSpeed)
        {
            _options.TextSpeed = textSpeed;
            return this;
        }

        public DialogueBoxBuilder SetBackground(TextureRegion2D background)
        {
            _options.Background = background;
            return this;
        }

        public DialogueBoxBuilder SetBounds(Rectangle bounds)
        {
            _options.Bounds = bounds;
            return this;
        }

        public DialogueBoxBuilder SetPadding(Thickness padding)
        {
            _options.Padding = padding;
            _options.OptionBoxOptions.Padding = padding;
            return this;
        }

        public DialogueBoxBuilder SetFont(IFont font)
        {
            _options.Font = font;
            return this;
        }

        public DialogueBoxBuilder SetTextColor(Color color)
        {
            _options.TextColor = color;
            return this;
        }

        public DialogueBoxBuilder SetSentenceSplitter(ISentenceSplitter splitter)
        {
            _options.SentenceSplitter = splitter;
            return this;
        }

        public DialogueBoxBuilder SetOptionLocation(DialogueOptionRenderLocation location)
        {
            _options.OptionBoxOptions.RenderLocation = location;
            return this;
        }

        public DialogueBoxBuilder SetOptionBoxBackground(TextureRegion2D background)
        {
            _options.OptionBoxOptions.WindowBackground = background;
            return this;
        }

        public DialogueBoxBuilder SetOptionBoxPadding(Thickness padding)
        {
            _options.OptionBoxOptions.Padding = padding;
            return this;
        }

        public DialogueBoxBuilder SetOptionBoxOffset(Point offset)
        {
            _options.OptionBoxOptions.Offset = offset;
            return this;
        }

        public DialogueBoxBuilder SetOptionMargin(int margin)
        {
            _options.OptionBoxOptions.OptionMargin = margin;
            return this;
        }

        public DialogueBoxBuilder SetOptionBackground(SpriteAnimation optionBackground)
        {
            _options.OptionBoxOptions.OptionBackground = optionBackground;
            return this;
        }

        public DialogueBoxBuilder SetOptionBackgroundPadding(Thickness padding)
        {
            _options.OptionBoxOptions.OptionBackgroundPadding = padding;
            return this;
        }

        public DialogueBoxBuilder SetOptionSelectIcon(SpriteAnimation optionSelectIcon)
        {
            _options.OptionBoxOptions.SelectIcon = optionSelectIcon;
            return this;
        }

        public DialogueBoxBuilder SetOptionSelectIconLocation(SelectIconLocation selectIconLocation)
        {
            _options.OptionBoxOptions.SelectIconLocation = selectIconLocation;
            return this;
        }

        public DialogueBoxBuilder SetOptionSelectIconOffset(Point offset)
        {
            _options.OptionBoxOptions.SelectIconOffset = offset;
            return this;
        }

        public DialogueBoxBuilder SetOptionMoveSelection(Func<int> optionMoveSelection)
        {
            _options.OptionBoxOptions.MoveSelection = optionMoveSelection;
            return this;
        }

        public DialogueBoxBuilder SetOptionMoveSelection(IActionManager actionManager, int actionUp, int actionDown)
            => SetOptionMoveSelection(() =>
            {
                if (actionManager.ActionCheck(actionUp))
                    return -1;

                if (actionManager.ActionCheck(actionDown))
                    return 1;

                return 0;
            });

        public DialogueBoxBuilder SetCharacters(IEnumerable<CharacterProfile> characters)
            => SetCharacters(characters.ToDictionary(cp => cp.Name));

        public DialogueBoxBuilder SetCharacters(Dictionary<string, CharacterProfile> characters)
        {
            _options.CharacterOptions.Characters = characters;
            return this;
        }

        public DialogueBoxBuilder SetCharacterFactory(ICharacterProcessorFactory characterFactory)
        {
            _options.CharacterOptions.CharacterFactory = characterFactory;
            return this;
        }

        public DialogueBoxBuilder SetDefaultCharacterBackgroundBounds(PortraitBounds bounds)
        {
            _options.CharacterOptions.DefaultPortraitBounds = bounds;
            return this;
        }

        public DialogueBoxBuilder SetMaxCharacters(int max)
        {
            _options.CharacterOptions.MaxCharacters = max;
            return this;
        }

        public DialogueBoxBuilder SetMaxCharactersInAGivenLocation(int max)
        {
            _options.CharacterOptions.MaxCharactersInAGivenLocation = max;
            return this;
        }

        public DialogueBoxBuilder SetCharacterAddBehavior(CharacterAddBehavior addBehavior)
        {
            _options.CharacterOptions.AddBehavior = addBehavior;
            return this;
        }

        public DialogueBoxBuilder SetCharacterSpeakerBehavior(CharacterSpeakerBehavior speakerBehavior)
        {
            _options.CharacterOptions.SpeakerBehavior = speakerBehavior;
            return this;
        }

        public DialogueBoxBuilder SetNonSpeakerColor(Color color)
        {
            _options.CharacterOptions.NonSpeakerColor = color;
            return this;
        }

        public DialogueBoxBuilder SetDarkenNonSpeakerBehavior(DarkenNonSpeaker darkenBehavior)
        {
            _options.CharacterOptions.DarkenNonSpeakerBehavior = darkenBehavior;
            return this;
        }

        public DialogueBoxBuilder SetMoveSpeakerToFront(bool value)
        {
            _options.CharacterOptions.MoveSpeakerToFront = value;
            return this;
        }

        public DialogueBoxBuilder SetSpeakerLeftIsFront(bool value)
        {
            _options.CharacterOptions.LeftIsFront = value;
            return this;
        }

        public DialogueBoxBuilder SetMultipleSpeakerOffset(Point offset)
        {
            _options.CharacterOptions.MultipleSpeakerOffset = offset;
            return this;
        }

        public DialogueBoxBuilder AddCharacterDrawOffset(CharacterLocation offset)
        {
            _options.CharacterOptions.DrawOffsets.Add(offset);
            return this;
        }

        public DialogueBoxBuilder AddCharacterDefaultLocation(CharacterLocation defaultLocation)
        {
            _options.CharacterOptions.DefaultLocation = defaultLocation;
            return this;
        }

        public DialogueBoxBuilder SetCharacterFlipFacesOnRight(bool value)
        {
            _options.CharacterOptions.FlipFacesOnRight = value;
            return this;
        }

        public DialogueBoxBuilder SetCharacterFlipBackgroundsOnRight(bool value)
        {
            _options.CharacterOptions.FlipBackgroundsOnRight = value;
            return this;
        }

        public DialogueBoxBuilder SetCharacterMoveTime(float value)
        {
            _options.CharacterOptions.MoveTime = value;
            return this;
        }

        public DialogueBox Build()
        {
            _options.SentenceSplitter ??= new SeparatorSplitter();
            _options.CharacterOptions.SetDefaults();
            return new DialogueBox(_game, _options);
        }
    }
}