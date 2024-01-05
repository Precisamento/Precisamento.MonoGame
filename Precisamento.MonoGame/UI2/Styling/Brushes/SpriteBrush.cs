using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Styling.Brushes
{
    public class SpriteBrush
    {
        private SpriteDrawParams _drawParams = new SpriteDrawParams();

        public SpriteAnimationPlayer AnimationPlayer { get; set; }

        public SpriteBrush(Sprite sprite)
        {
            AnimationPlayer = new SpriteAnimationPlayer();
            AnimationPlayer.Animation = sprite.AnimationList[0];
        }

        public SpriteBrush(SpriteAnimationPlayer player)
        {
            AnimationPlayer = player;
        }

        public void Update(float delta)
        {
            AnimationPlayer.Update(delta);
        }

        public void Draw(SpriteBatchState state, Rectangle rect, Color color)
        {
            _drawParams.Color = color;
            AnimationPlayer.Draw(state, rect, ref _drawParams);
        }
    }
}
