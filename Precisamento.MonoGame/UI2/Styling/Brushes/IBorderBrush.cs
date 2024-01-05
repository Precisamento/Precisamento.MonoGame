using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.UI2.Styling.Brushes
{
    public interface IBorderBrush
    {
        void Draw(SpriteBatchState state, Rectangle borderBounds, Thickness borderSize, Color color);
    }
}
