using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Styling.Brushes
{
    public class TextureBrush : IBrush
    {
        public TextureRegion2D Texture { get; set; }

        public TextureBrush(TextureRegion2D texture)
        {
            Texture = texture;
        }

        public TextureBrush(Texture2D texture)
        {
            Texture = new TextureRegion2D(texture);
        }

        public void Update(float delta)
        {
        }

        public void Draw(SpriteBatchState state, Rectangle dest, Color color)
        {
            state.SpriteBatch.Draw(Texture, dest, color);
        }
    }
}
