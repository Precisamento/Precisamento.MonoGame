using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Graphics
{
    public class SpriteReader : ResourceTypeReader<Sprite>
    {
        public override string FileExtension => ".bin";

        public override Sprite Read(ResourceReader reader)
        {
            string? textureName = null;

            var sprite = new Sprite();
            sprite.Name = reader.ReadString();
            var sameTexture = reader.ReadBoolean();
            if(sameTexture)
                textureName = reader.ReadString();

            var animationCount = reader.ReadInt32();
            for(var i = 0; i < animationCount; i++)
            {
                var animation = new SpriteAnimation();
                animation.Name = reader.ReadString();
                animation.UpdateMode = (SpriteUpdateMode)reader.ReadInt32();
                animation.Origin = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                animation.FramesPerSecond = reader.ReadSingle();
                animation.StartFrameIndex = reader.ReadInt32();

                var frameCount = reader.ReadInt32();
                animation.Frames = new List<TextureRegion2D>(frameCount);

                for(var j = 0; j < frameCount; j++)
                {
                    if(!sameTexture)
                        textureName = reader.ReadString();

                    var flags = reader.ReadByte();
                    var thicknessIsEqual = (flags & 1) != 0;
                    var hasThickness = (flags & 2) != 0;
                    var usesRegions = (flags & 4) != 0;
                    var thickness = new Thickness();

                    if(hasThickness)
                    {
                        if(thicknessIsEqual)
                        {
                            thickness = new Thickness(reader.ReadInt32());
                        }
                        else
                        {
                            thickness = new Thickness(
                                reader.ReadInt32(), 
                                reader.ReadInt32(), 
                                reader.ReadInt32(),
                                reader.ReadInt32());
                        }
                    }

                    TextureRegion2D frame;

                    if(usesRegions)
                    {
                        var atlas = reader.ResourceLoader.Content.Load<TextureAtlas>(textureName);
                        var regionName = reader.ReadString();
                        frame = atlas.GetRegion(regionName);
                    }
                    else
                    {
                        var texture = reader.ResourceLoader.Load<Texture2D>(textureName!);
                        frame = new TextureRegion2D(
                            texture,
                            reader.ReadInt32(),
                            reader.ReadInt32(),
                            reader.ReadInt32(),
                            reader.ReadInt32());
                    }

                    if (hasThickness && frame is not NinePatchRegion2D)
                        frame = new NinePatchRegion2D(frame, thickness);

                    animation.Frames.Add(frame);
                }

                sprite.AnimationList.Add(animation);
                sprite.Animations.Add(animation.Name, animation);
            }

            return sprite;
        }
    }
}
