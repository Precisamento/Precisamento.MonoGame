using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarn.Markup;

namespace Precisamento.MonoGame.Dialogue
{
    public class DialogueFontProcessor : DialogueProcessor
    {
        private IFont _font;

        public DialogueFontProcessor()
        { }

        public DialogueFontProcessor(IFont font)
        {
            _font = font;
        }

        public override void Init(Game game, MarkupAttribute attribute)
        {
            base.Init(game, attribute);
            var loader = game.Services.GetService<IResourceLoader>();
            var fontFile = attribute.Properties[attribute.Name].StringValue;
            try
            {
                var spriteFont = loader.Load<SpriteFont>(fontFile);
                _font = new SpriteFontWrapper(spriteFont);
            }
            catch
            {
                try
                {
                    var bmpFont = loader.Load<BitmapFont>(fontFile);
                    _font = new BitmapFontWrapper(bmpFont);
                }
                catch(Exception e)
                {
                    throw new ArgumentException(
                        $"Unable to load font file: {fontFile}",
                        nameof(attribute),
                        e);
                }
            }
        }

        public override void Pop(DialogueState state)
        {
            state.Fonts.Pop();
        }

        public override void Push(DialogueState state)
        {
            state.Fonts.Push(_font);
        }
    }
}