using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Graphics
{
    public class SpriteReader : ResourceTypeReader<Sprite>
    {
        public override string FileExtension => ".sprite";

        public override Sprite Read(ResourceReader reader)
        {
            var sprite = new Sprite();
            sprite.Name = reader.ReadString();

            var animationCount = reader.ReadInt32();
            for(var i = 0; i < animationCount; i++)
            {
                var animation = new SpriteAnimation();
                animation.Name = reader.ReadString();
                animation.UpdateMode = (SpriteUpdateMode)reader.ReadInt32();
                animation.Origin = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                animation.FramesPerSecond = reader.ReadSingle();
                var usesRegionNames = reader.ReadBoolean();
                animation.StartFrameIndex = reader.ReadInt32();

                var frameCount = reader.ReadInt32();
                for(var j = 0; j < frameCount; j++)
                {
                    var textureName = reader.ReadString();
                    TextureRegion2D frame;

                    if(usesRegionNames)
                    {
                        var atlas = reader.ResourceLoader.Content.Load<TextureAtlas>(textureName);
                        var regionName = reader.ReadString();
                        frame = atlas.GetRegion(regionName);
                    }
                    else
                    {
                        var texture = reader.ResourceLoader.Load<Texture2D>(textureName);
                        frame = new TextureRegion2D(
                            texture,
                            reader.ReadInt32(),
                            reader.ReadInt32(),
                            reader.ReadInt32(),
                            reader.ReadInt32());
                    }

                    var thickness = reader.ReadInt32();
                    if (thickness != 0 && !(frame is NinePatchRegion2D))
                        frame = new NinePatchRegion2D(frame, thickness);

                    animation.Frames.Add(frame);
                }
            }

            return sprite;
        }
    }
}
