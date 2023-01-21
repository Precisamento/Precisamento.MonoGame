using DefaultEcs;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
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
            return this;
        }

        public DialogueBoxBuilder SetContinuePressed(IActionManager actionManager, int action)
        {
            _options.ContinuePressed = () => actionManager.ActionCheckPressed(action);
            return this;
        }

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

        public DialogueBox Build()
        {
            _options.SentenceSplitter ??= new SeparatorSplitter();
            return new DialogueBox(_game, _options);
        }
    }
}