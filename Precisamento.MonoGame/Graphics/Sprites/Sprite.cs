using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Precisamento.MonoGame.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Precisamento.MonoGame.Graphics
{
    public class Sprite
    {
        public string Name { get; set; }
        public List<SpriteAnimation> AnimationList { get; } = new List<SpriteAnimation>();
        public Dictionary<string, SpriteAnimation> Animations { get; set; } = new Dictionary<string, SpriteAnimation>();

        /// <summary>
        /// Creates a Sprite from a JSON file.
        /// </summary>
        /// <param name="jsonFile">The path to the JSON file containing the sprite info.</param>
        /// <param name="resources">A resource loader that can be used to load the texture values.</param>
        public static Sprite FromJson(string jsonFile, IResourceLoader resources)
        {
            using(var reader = new FileStream(jsonFile, FileMode.Open, FileAccess.Read))
            {
                var doc = JsonDocument.Parse(reader);
                return FromJson(doc.RootElement, resources);
            }
        }

        /// <summary>
        /// Creates a Sprite from a JsonElement.
        /// </summary>
        /// <param name="root">The root element of the sprite.</param>
        /// <param name="resources">A resource loader that can be used to load the texture values.</param>
        public static Sprite FromJson(JsonElement root, IResourceLoader resources)
        {
            var sprite = new Sprite();
            sprite.Name = root.GetProperty("Name").GetString();

            string defaultTexture = null;
            int defaultWidth = -1;
            int defaultHeight = -1;
            int defaultThickness = 0;
            float defaultFps = 0;
            SpriteUpdateMode defaultUpdateMode = SpriteUpdateMode.None;
            int defaultStartingFrame = 0;
            bool defaultUsesRegions = false;
            Vector2 defaultOrigin = Vector2.Zero;

            if (root.TryGetProperty("Texture", out var textureElem))
            {
                defaultTexture = textureElem.GetString();
                if (string.IsNullOrWhiteSpace(defaultTexture))
                    throw new JsonException("Invalid value for Texture. Cannot be an empty string.");
            }

            if (root.TryGetProperty("Width", out var widthElem))
            {
                defaultWidth = widthElem.GetInt32();
                if (defaultWidth <= 0)
                    throw new JsonException("Invalid value for Width. Cannot be <= 0.");
            }

            if (root.TryGetProperty("Height", out var heightElem))
            {
                defaultHeight = heightElem.GetInt32();
                if (defaultHeight <= 0)
                    throw new JsonException("Invalid value for Height. Cannot be <= 0.");
            }

            if (root.TryGetProperty("Thickness", out var thicknessElem))
            {
                defaultThickness = thicknessElem.GetInt32();
                if (defaultThickness < 0)
                    throw new JsonException("Invalid value for Thickness. Cannot be < 0.");
            }

            if (root.TryGetProperty("FPS", out var fpsElem))
            {
                defaultFps = fpsElem.GetSingle();
                if (defaultFps < 0)
                    throw new JsonException("Invalid value for FPS. Cannot be < 0.");
            }

            if (root.TryGetProperty("StartingFrame", out var startingFrameElem))
            {
                defaultStartingFrame = startingFrameElem.GetInt32();
                if (defaultStartingFrame < 0)
                    throw new JsonException("Invalid value for StartingFrame. Cannot be < 0.");
            }

            if (root.TryGetProperty("UpdateMode", out var updateModeElem))
            {
                defaultUpdateMode = ConvertUpdateModeStringToEnum(updateModeElem.GetString());
            }

            if (root.TryGetProperty("Origin", out var originElem))
            {
                defaultOrigin.X = originElem.GetProperty("X").GetSingle();
                defaultOrigin.Y = originElem.GetProperty("Y").GetSingle();
            }

            if (root.TryGetProperty("UsesRegions", out var usesRegionsElem))
                defaultUsesRegions = usesRegionsElem.GetBoolean();

            if (root.TryGetProperty("Animations", out var animations))
            {
                for (var i = 0; i < animations.GetArrayLength(); i++)
                {
                    var animation = animations[i];
                    var result = new SpriteAnimation();
                    result.Frames = new List<TextureRegion2D>();

                    if (animation.TryGetProperty("Name", out var nameElem))
                    {
                        result.Name = nameElem.GetString();
                        if (string.IsNullOrWhiteSpace(result.Name))
                            throw new JsonException("Invalid value for Name. Cannot be an empty string.");
                    }
                    else
                    {
                        result.Name = i.ToString();
                    }

                    sprite.AnimationList.Add(result);
                    sprite.Animations.Add(result.Name, result);

                    if (animation.TryGetProperty("UpdateMode", out updateModeElem))
                    {
                        result.UpdateMode = ConvertUpdateModeStringToEnum(updateModeElem.GetString());
                    }
                    else
                    {
                        result.UpdateMode = defaultUpdateMode;
                    }

                    if (animation.TryGetProperty("Origin", out originElem))
                    {
                        var origin = new Vector2();
                        origin.X = originElem.GetProperty("X").GetSingle();
                        origin.Y = originElem.GetProperty("Y").GetSingle();
                        result.Origin = origin;
                    }
                    else
                    {
                        result.Origin = defaultOrigin;
                    }

                    if (animation.TryGetProperty("FPS", out fpsElem))
                    {
                        result.FramesPerSecond = fpsElem.GetSingle();
                        if (result.FramesPerSecond < 0)
                            throw new JsonException("Invalid value for FPS. Cannot be < 0.");
                    }
                    else
                    {
                        result.FramesPerSecond = defaultFps;
                    }

                    if (animation.TryGetProperty("StartingFrame", out startingFrameElem))
                    {
                        result.StartFrameIndex = startingFrameElem.GetInt32();
                        if (result.StartFrameIndex < 0)
                            throw new JsonException("Invalid value for StartingFrame. Cannot be < 0.");
                    }
                    else
                    {
                        result.StartFrameIndex = defaultStartingFrame;
                    }

                    bool usesRegions = defaultUsesRegions;

                    if (animation.TryGetProperty("UsesRegions", out usesRegionsElem))
                        usesRegions = usesRegionsElem.GetBoolean();

                    if (animation.TryGetProperty("Frames", out var frames))
                    {
                        var width = defaultWidth;
                        var height = defaultHeight;
                        var thickness = defaultThickness;
                        var texture = defaultTexture;

                        if (animation.TryGetProperty("Width", out widthElem))
                            width = widthElem.GetInt32();

                        if (animation.TryGetProperty("Height", out heightElem))
                            height = heightElem.GetInt32();

                        if (animation.TryGetProperty("Thickness", out thicknessElem))
                            thickness = thicknessElem.GetInt32();

                        if (animation.TryGetProperty("Texture", out textureElem))
                            texture = textureElem.GetString();

                        for (var j = 0; j < frames.GetArrayLength(); j++)
                        {
                            var frameElem = frames[j];
                            var frame = usesRegions ?
                                ReadSpriteRegion(frameElem, resources, thickness, texture) :
                                ReadSpriteFrame(frameElem, resources, width, height, thickness, texture);
                            result.Frames.Add(frame);
                        }
                    }
                    else
                    {
                        var frame = usesRegions ?
                            ReadSpriteRegion(animation, resources, defaultThickness, defaultTexture) :
                            ReadSpriteFrame(animation, resources, defaultWidth, defaultHeight, defaultThickness, defaultTexture);
                        result.Frames.Add(frame);
                    }
                }
            }
            else
            {
                var frame = defaultUsesRegions ?
                    ReadSpriteRegion(root, resources, defaultThickness, defaultTexture) :
                    ReadSpriteFrame(root, resources, defaultWidth, defaultHeight, defaultThickness, defaultTexture);

                var animation = new SpriteAnimation();
                animation.Frames.Add(frame);
                animation.FramesPerSecond = defaultFps;
                animation.Name = "Default";
                animation.Origin = defaultOrigin;
                animation.UpdateMode = defaultUpdateMode;
                animation.StartFrameIndex = defaultStartingFrame;
            }

            return sprite;
        }

        private static TextureRegion2D ReadSpriteRegion(JsonElement element, IResourceLoader resources, int defaultThickness, string defaultTexture)
        {
            int thickness = defaultThickness;
            string texture = defaultTexture;
            string region = element.GetProperty("Region").GetString();

            if (element.TryGetProperty("Thickness", out var thicknessElem))
                thickness = thicknessElem.GetInt32();

            if (element.TryGetProperty("Texture", out var textureElem))
                texture = textureElem.GetString();

            if (string.IsNullOrWhiteSpace(texture))
                throw new JsonException("Invalid value for Texture. Cannot be an empty string.");

            if (string.IsNullOrWhiteSpace(region))
                throw new JsonException("Invalid value for Region. Cannot be an empty string.");

            var atlas = resources.Load<TextureAtlas>(texture);
            var frame = atlas.GetRegion(region);

            if (thickness != 0 && !(frame is NinePatchRegion2D))
                frame = new NinePatchRegion2D(frame, thickness);

            return frame;
        }

        private static TextureRegion2D ReadSpriteFrame(JsonElement element, IResourceLoader resources, int defaultWidth, int defaultHeight, int defaultThickness, string defaultTexture)
        {
            var x = element.GetProperty("X").GetInt32();
            var y = element.GetProperty("Y").GetInt32();
            int width = defaultWidth;
            int height = defaultHeight;
            int thickness = defaultThickness;
            string texture = defaultTexture;

            if (element.TryGetProperty("Width", out var widthElem))
                width = widthElem.GetInt32();

            if (element.TryGetProperty("Height", out var heightElem))
                height = heightElem.GetInt32();

            if (element.TryGetProperty("Thickness", out var thicknessElem))
                thickness = thicknessElem.GetInt32();

            if (element.TryGetProperty("Texture", out var textureElem))
                texture = textureElem.GetString();

            if (x < 0)
                throw new JsonException("Invalid value for X. Cannot be < 0.");

            if (y < 0)
                throw new JsonException("Invalid value for Y. Cannot be < 0.");

            if (width < 0)
                throw new JsonException("Invalid value for Width. Cannot be < 0.");

            if (height < 0)
                throw new JsonException("Invalid value for Height. Cannot be < 0.");

            if (thickness < 0)
                throw new JsonException("Invalid value for Thickness. Cannot be < 0.");

            if (string.IsNullOrWhiteSpace(texture))
                throw new JsonException("Invalid value for Texture. Cannot be an empty string.");

            var atlas = resources.Load<Texture2D>(texture);
            var frame = new TextureRegion2D(atlas, x, y, width, height);

            if (thickness != 0)
                frame = new NinePatchRegion2D(frame, thickness);

            return frame;
        }

        private static SpriteUpdateMode ConvertUpdateModeStringToEnum(string updateMode)
        {
            return (SpriteUpdateMode)Enum.Parse(typeof(SpriteUpdateMode), updateMode, true);
        }
    }
}