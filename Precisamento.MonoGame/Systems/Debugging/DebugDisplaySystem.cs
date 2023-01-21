using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Systems.Debugging
{
    public class DebugDisplay
    {
        private List<string> _values = new List<string>();

        public IFont Font { get; }
        public IReadOnlyList<string> Values => _values;
        public Color Color { get; set; } = Color.White;

        public DebugDisplay(IFont font)
        {
            Font = font;
        }

        public void Debug(string value)
        {
            _values.Add(value);
        }

        public void Clear()
        {
            _values.Clear();
        }

        public ActionSystem<SpriteBatchState> CreateDebugDisplaySystem()
        {
            return new ActionSystem<SpriteBatchState>(state =>
            {
                Font.Draw(state.SpriteBatch, string.Join("\n", Values), new Vector2(5), Color);

                Clear();
            });
        }
    }
}
