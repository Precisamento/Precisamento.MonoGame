using MonoGame.Extended;
using Soren.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Precisamento.MonoGame.Resources.Sprites
{
    public class SpriteWriter : ResourceTypeWriter<SpriteDescription>
    {
        protected override void Write(BinaryWriter writer, SpriteDescription value)
        {
            var textures = value.Animations
                .SelectMany(sad => sad.Frames)
                .Select(f => f.TextureName);

            bool sameTexture = textures.Unanimous();

            writer.Write(value.Name);
            writer.Write(sameTexture);
            if(sameTexture)
                writer.Write(textures.First(t => t != null));

            writer.Write(value.Animations.Count);

            for (var i = 0; i < value.Animations.Count; i++)
            {
                var animation = value.Animations[i];
                writer.Write(animation.Name);
                writer.Write((int)animation.UpdateMode);
                writer.Write(animation.Origin.X);
                writer.Write(animation.Origin.Y);
                writer.Write(animation.FramesPerSecond);
                writer.Write(animation.StartingFrame);
                writer.Write(animation.Frames.Count);

                for (var j = 0; j < animation.Frames.Count; j++)
                {
                    var frame = animation.Frames[j];
                    if(!sameTexture)
                        writer.Write(frame.TextureName);

                    var thicknessIsEqual = ThicknessIsEqual(frame.Thickness) ? 1 : 0;
                    var hasThickness = ThicknessIsZero(frame.Thickness) ? 0 : 2;
                    var usesRegionNames = frame.UsesRegionNames ? 4 : 0;
                    writer.Write((byte)(thicknessIsEqual | hasThickness | usesRegionNames));

                    if(hasThickness != 0)
                    {
                        if(thicknessIsEqual != 0)
                        {
                            writer.Write(frame.Thickness.Left);
                        }
                        else
                        {
                            writer.Write(frame.Thickness.Top);
                            writer.Write(frame.Thickness.Left);
                            writer.Write(frame.Thickness.Bottom);
                            writer.Write(frame.Thickness.Right);
                        }
                    }

                    if (frame.UsesRegionNames)
                    {
                        writer.Write(frame.RegionName);
                    }
                    else
                    {
                        writer.Write(frame.Bounds.X);
                        writer.Write(frame.Bounds.Y);
                        writer.Write(frame.Bounds.Width);
                        writer.Write(frame.Bounds.Height);
                    }
                }
            }
        }

        private bool ThicknessIsEqual(Thickness thickness)
        {
            return thickness.Left == thickness.Right 
                && thickness.Top == thickness.Bottom 
                && thickness.Left == thickness.Top;
        }

        private bool ThicknessIsZero(Thickness thickness)
        {
            return ThicknessIsEqual(thickness) && thickness.Left == 0;
        }
    }
}
