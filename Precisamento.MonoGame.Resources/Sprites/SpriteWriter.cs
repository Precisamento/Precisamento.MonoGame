using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Precisamento.MonoGame.Resources.Sprites
{
    public class SpriteWriter : ResourceTypeWriter<SpriteDescription>
    {
        protected override void Write(BinaryWriter writer, SpriteDescription value)
        {
            writer.Write(value.Name);
            writer.Write(value.Animations.Count);

            for (var i = 0; i < value.Animations.Count; i++)
            {
                var animation = value.Animations[i];
                writer.Write(animation.Name);
                writer.Write((int)animation.UpdateMode);
                writer.Write(animation.Origin.X);
                writer.Write(animation.Origin.Y);
                writer.Write(animation.FramesPerSecond);
                writer.Write(animation.UsesRegionNames);
                writer.Write(animation.StartingFrame);
                writer.Write(animation.Frames.Count);

                for (var j = 0; j < animation.Frames.Count; j++)
                {
                    var frame = animation.Frames[j];
                    writer.Write(frame.TextureName);
                    if (animation.UsesRegionNames)
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
                    writer.Write(frame.Thickness);
                }
            }
        }
    }
}
