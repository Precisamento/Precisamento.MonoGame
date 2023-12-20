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
            _options.OptionSelected = continuedPressed;
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
            _options.OptionBoxPadding = padding;
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
            _options.OptionRenderLocation = location;
            return this;
        }

        public DialogueBoxBuilder SetOptionBoxBackground(TextureRegion2D background)
        {
            _options.OptionBoxWindowBackground = background;
            return this;
        }

        public DialogueBoxBuilder SetOptionBoxPadding(Thickness padding)
        {
            _options.OptionBoxPadding = padding;
            return this;
        }

        public DialogueBoxBuilder SetOptionBoxOffset(Point offset)
        {
            _options.OptionBoxOffset = offset;
            return this;
        }

        public DialogueBoxBuilder SetOptionMargin(int margin)
        {
            _options.OptionMargin = margin;
            return this;
        }

        public DialogueBoxBuilder SetOptionBackground(SpriteAnimation optionBackground)
        {
            _options.OptionBackground = optionBackground;
            return this;
        }

        public DialogueBoxBuilder SetOptionBackgroundPadding(Thickness padding)
        {
            _options.OptionBackgroundPadding = padding;
            return this;
        }

        public DialogueBoxBuilder SetOptionSelectIcon(SpriteAnimation optionSelectIcon)
        {
            _options.OptionSelectIcon = optionSelectIcon;
            return this;
        }

        public DialogueBoxBuilder SetOptionSelectIconLocation(SelectIconLocation selectIconLocation)
        {
            _options.OptionSelectIconLocation = selectIconLocation;
            return this;
        }

        public DialogueBoxBuilder SetOptionSelectIconOffset(Point offset)
        {
            _options.OptionSelectIconOffset = offset;
            return this;
        }

        public DialogueBoxBuilder SetOptionMoveSelection(Func<int> optionMoveSelection)
        {
            _options.OptionMoveSelection = optionMoveSelection;
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
            _options.Characters = characters;
            return this;
        }

        public DialogueBox Build()
        {
            _options.SentenceSplitter ??= new SeparatorSplitter();
            return new DialogueBox(_game, _options);
        }
    }
}