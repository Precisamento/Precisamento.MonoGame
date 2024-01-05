using Microsoft.Extensions.Primitives;
using Microsoft.Xna.Framework;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Graphics.Fonts
{
    public class NoContextStringSplitter : StringSplitter
    {
        public override IEnumerable<string> Split(string text, IFont font, Point availableSize)
        {
            var start = 0;
            var builder = new StringBuilder();
            var height = 0;
            var approximateCharactersInLine = MathExt.FastFloorToInt(availableSize.X / font.MeasureString("M").X);
            while (start < text.Length)
            {
                builder.Clear();
                var end = GetNextLineEnd(text, start);
                builder.Append(text, start, end);

                var size = font.MeasureString(builder);

                if (height + size.Y > availableSize.Y)
                {
                    break;
                }

                foreach(var line in SplitSubstring(builder, font, availableSize, approximateCharactersInLine))
                {
                    if (height + font.LineSpacing > availableSize.Y)
                        yield break;

                    height += font.LineSpacing;

                    yield return line;
                }

                start += end + 1;
            }
        }

        private IEnumerable<string> SplitSubstring(StringBuilder text, IFont font, Point availableSize, int approximateCharactersInLine)
        {
            var size = font.MeasureString(text).ToPoint();
            if (size.X < availableSize.X)
            {
                yield return text.ToString();
                yield break;
            }

            var secondBuilder = new StringBuilder();
            var start = 0;
            while (start < text.Length)
            {
                secondBuilder.Clear();

                var count = approximateCharactersInLine;
                secondBuilder.Append(text, start, count);
                size = font.MeasureString(secondBuilder).ToPoint();

                var direction = availableSize.X.CompareTo(size.X);
                while (direction != 0 && count < text.Length && count >= 0)
                {
                    if (direction == -1)
                    {
                        secondBuilder.Remove(secondBuilder.Length - 2, 1);
                        count--;
                    }
                    else
                    {
                        secondBuilder.Append(text[start + count]);
                        count++;
                    }

                    size = font.MeasureString(secondBuilder).ToPoint();
                    var newDirection = availableSize.X.CompareTo(size.X);

                    if (newDirection != direction)
                    {
                        break;
                    }
                }

                start = count + 1;
                yield return secondBuilder.ToString();
            }
        }
    }
}
