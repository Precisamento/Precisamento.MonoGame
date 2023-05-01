using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Precisamento.MonoGame.Resources.Sprites
{
    [ResourceImporter(typeof(SpriteWriter))]
    public class SpriteImporter : ResourceImporter<SpriteDescription>
    {   
        public override string FileExtension => ".sprite";

        protected override SpriteDescription ImportImpl(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var json = JsonDocument.Parse(stream);
                return ReadSpriteDescription(json.RootElement);
            }
        }

        /// <summary>
        /// Creates a Sprite from a JsonElement.
        /// </summary>
        /// <param name="root">The root element of the sprite.</param>
        /// <param name="resources">A resource loader that can be used to load the texture values.</param>
        public SpriteDescription ReadSpriteDescription(JsonElement root)
        {
            var sprite = new SpriteDescription();
            if (TryGetProperty(root, out var name, "Name", "name"))
                sprite.Name = name.GetString() ?? "";

            string? defaultTexture = null;
            int defaultWidth = -1;
            int defaultHeight = -1;
            Thickness defaultThickness = new(0);
            float defaultFps = 0;
            var defaultUpdateMode = SpriteUpdateMode.None;
            var defaultStartingFrame = 0;
            var defaultOrigin = Vector2.Zero;

            if (TryGetProperty(root, out var textureElem, "Texture", "texture"))
            {
                defaultTexture = textureElem.GetString();
            }

            if (TryGetProperty(root, out var widthElem, "Width", "width"))
            {
                defaultWidth = widthElem.GetInt32();
            }

            if (TryGetProperty(root, out var heightElem, "Height", "height"))
            {
                defaultHeight = heightElem.GetInt32();
            }

            if (TryGetProperty(root, out var thicknessElem, "Thickness", "thickness"))
            {
                defaultThickness = ReadThickness(thicknessElem, defaultThickness);
            }

            if (TryGetProperty(root, out var fpsElem, "FPS", "Fps", "fps"))
            {
                defaultFps = fpsElem.GetSingle();
            }

            if (TryGetProperty(root, out var startingFrameElem, "StartingFrame", "startingFrame", "starting_frame"))
            {
                defaultStartingFrame = startingFrameElem.GetInt32();
            }

            if (TryGetProperty(root, out var updateModeElem, "UpdateMode", "updateMode", "update_mode"))
            {
                defaultUpdateMode = ConvertUpdateModeStringToEnum(updateModeElem.GetString());
            }

            if (TryGetProperty(root, out var originElem, "Origin", "origin"))
            {
                defaultOrigin.X = GetProperty(originElem, "X", "x").GetSingle();
                defaultOrigin.Y = GetProperty(originElem, "Y", "y").GetSingle();
            }

            if (TryGetProperty(root, out var animations, "Animations", "animations"))
            {
                for (var i = 0; i < animations.GetArrayLength(); i++)
                {
                    var animation = animations[i];
                    var result = new SpriteAnimationDescription();
                    result.Frames = new List<SpriteFrameDescription>();

                    if (TryGetProperty(animation, out var nameElem, "Name", "name"))
                    {
                        result.Name = nameElem.GetString();
                        if (string.IsNullOrWhiteSpace(result.Name))
                            throw new JsonException("Invalid value for Name. Cannot be an empty string.");
                    }
                    else
                    {
                        result.Name = i.ToString();
                    }

                    sprite.Animations.Add(result);

                    if (TryGetProperty(animation, out updateModeElem, "UpdateMode", "updateMode", "update_mode"))
                    {
                        result.UpdateMode = ConvertUpdateModeStringToEnum(updateModeElem.GetString());
                    }
                    else
                    {
                        result.UpdateMode = defaultUpdateMode;
                    }

                    if (TryGetProperty(animation, out originElem, "Origin", "origin"))
                    {
                        var origin = new Vector2();
                        origin.X = GetProperty(originElem, "X", "x").GetSingle();
                        origin.Y = GetProperty(originElem, "Y", "y").GetSingle();
                        result.Origin = origin;
                    }
                    else
                    {
                        result.Origin = defaultOrigin;
                    }

                    if (TryGetProperty(animation, out fpsElem, "FPS", "fps"))
                    {
                        result.FramesPerSecond = fpsElem.GetSingle();
                        if (result.FramesPerSecond < 0)
                            throw new JsonException("Invalid value for FPS. Cannot be < 0.");
                    }
                    else
                    {
                        result.FramesPerSecond = defaultFps;
                    }

                    if (TryGetProperty(animation, out startingFrameElem, "StartingFrame", "startingFrame", "starting_frame"))
                    {
                        result.StartingFrame = startingFrameElem.GetInt32();
                        if (result.StartingFrame < 0)
                            throw new JsonException("Invalid value for StartingFrame. Cannot be < 0.");
                    }
                    else
                    {
                        result.StartingFrame = defaultStartingFrame;
                    }

                    if (TryGetProperty(animation, out var frames, "Frames", "frames"))
                    {
                        var width = defaultWidth;
                        var height = defaultHeight;
                        var thickness = defaultThickness;
                        var texture = defaultTexture;

                        // Validation checks are done by the Frame/Region creation.

                        if (TryGetProperty(animation, out widthElem, "Width", "width"))
                            width = widthElem.GetInt32();

                        if (TryGetProperty(animation, out heightElem, "Height", "height"))
                            height = heightElem.GetInt32();

                        if (TryGetProperty(animation, out thicknessElem, "Thickness", "thickness"))
                            thickness = ReadThickness(thicknessElem, defaultThickness);

                        if (TryGetProperty(animation, out textureElem, "Texture", "texture"))
                            texture = textureElem.GetString();

                        for (var j = 0; j < frames.GetArrayLength(); j++)
                        {
                            var frameElem = frames[j];
                            var frame = ReadFrameOrRegion(frameElem, width, height, thickness, texture);
                            result.Frames.Add(frame);
                        }
                    }
                    else
                    {
                        var frame = ReadFrameOrRegion(animation, defaultWidth, defaultHeight, defaultThickness, defaultTexture);
                        result.Frames.Add(frame);
                    }
                }
            }
            else if (TryGetProperty(root, out var frames, "Frames", "frames"))
            {
                var animation = new SpriteAnimationDescription();
                animation.Frames = new List<SpriteFrameDescription>();
                animation.FramesPerSecond = defaultFps;
                animation.Name = "0";
                animation.Origin = defaultOrigin;
                animation.UpdateMode = defaultUpdateMode;
                animation.StartingFrame = defaultStartingFrame;

                for (var j = 0; j < frames.GetArrayLength(); j++)
                {
                    var frameElem = frames[j];
                    var frame = ReadFrameOrRegion(frameElem, defaultWidth, defaultHeight, defaultThickness, defaultTexture);
                    animation.Frames.Add(frame);
                }
            }
            else
            {
                var frame = ReadFrameOrRegion(root, defaultWidth, defaultHeight, defaultThickness, defaultTexture);

                var animation = new SpriteAnimationDescription();
                animation.Frames = new List<SpriteFrameDescription>() { frame };
                animation.FramesPerSecond = defaultFps;
                animation.Name = "0";
                animation.Origin = defaultOrigin;
                animation.UpdateMode = defaultUpdateMode;
                animation.StartingFrame = defaultStartingFrame;
            }

            return sprite;
        }

        private static SpriteFrameDescription ReadFrameOrRegion(JsonElement element, int defaultWidth, int defaultHeight, Thickness defaultThickness, string? defaultTexture)
        {
            if (TryGetProperty(element, out _, "Region", "region"))
            {
                return ReadSpriteRegion(element, defaultThickness, defaultTexture);
            }
            else
            {
                return ReadSpriteFrame(element, defaultWidth, defaultHeight, defaultThickness, defaultTexture);
            }
        }

        private static SpriteFrameDescription ReadSpriteRegion(JsonElement element, Thickness defaultThickness, string? defaultTexture)
        {
            var thickness = defaultThickness;
            string? texture = defaultTexture;
            string? region = GetProperty(element, "Region", "region").GetString();

            if (TryGetProperty(element, out var thicknessElem, "Thickness", "thickness"))
                thickness = ReadThickness(thicknessElem, defaultThickness);

            if (TryGetProperty(element, out var textureElem, "Texture", "texture"))
                texture = textureElem.GetString();

            if (string.IsNullOrWhiteSpace(texture))
                throw new JsonException("Invalid value for Texture. Cannot be an empty string.");

            if (string.IsNullOrWhiteSpace(region))
                throw new JsonException("Invalid value for Region. Cannot be an empty string.");

            var frame = new SpriteFrameDescription()
            {
                TextureName = texture,
                RegionName = region,
                Thickness = thickness,
                UsesRegionNames = true
            };

            return frame;
        }

        private static SpriteFrameDescription ReadSpriteFrame(JsonElement element, int defaultWidth, int defaultHeight, Thickness defaultThickness, string? defaultTexture)
        {
            var x = GetProperty(element, "X", "x").GetInt32();
            var y = GetProperty(element, "Y", "y").GetInt32();
            var width = defaultWidth;
            var height = defaultHeight;
            var thickness = defaultThickness;
            var texture = defaultTexture;

            if (TryGetProperty(element, out var widthElem, "Width", "width"))
                width = widthElem.GetInt32();

            if (TryGetProperty(element, out var heightElem, "Height", "height"))
                height = heightElem.GetInt32();

            if (TryGetProperty(element, out var thicknessElem, "Thickness", "thickness"))
                thickness = ReadThickness(thicknessElem, defaultThickness);

            if (TryGetProperty(element, out var textureElem, "Texture", "texture"))
                texture = textureElem.GetString();

            if (x < 0)
                throw new JsonException("Invalid value for X. Cannot be < 0.");

            if (y < 0)
                throw new JsonException("Invalid value for Y. Cannot be < 0.");

            if (width < 0)
                throw new JsonException("Invalid value for Width. Cannot be < 0.");

            if (height < 0)
                throw new JsonException("Invalid value for Height. Cannot be < 0.");

            if (string.IsNullOrWhiteSpace(texture))
                throw new JsonException("Invalid value for Texture. Cannot be an empty string.");

            var frame = new SpriteFrameDescription()
            {
                TextureName = texture,
                Thickness = thickness,
                Bounds = new Rectangle(x, y, width, height),
                UsesRegionNames = false
            };

            return frame;
        }

        private static bool ThicknessIsNotZero(Thickness thickness)
        {
            return thickness.Top != 0
                || thickness.Bottom != 0
                || thickness.Left != 0
                || thickness.Right != 0;
        }

        private static Thickness ReadThickness(JsonElement element, Thickness defaultThickness)
        {
            var result = defaultThickness;

            if (element.TryGetInt32(out var padding))
            {
                result = new Thickness(padding);
            }
            else
            {
                if (TryGetProperty(element, out var top, "Top", "top"))
                {
                    result.Top = top.GetInt32();
                }

                if (TryGetProperty(element, out var left, "Left", "left"))
                {
                    result.Left = left.GetInt32();
                }

                if (TryGetProperty(element, out var bottom, "Bottom", "bottom"))
                {
                    result.Bottom = bottom.GetInt32();
                }

                if (TryGetProperty(element, out var right, "Right", "right"))
                {
                    result.Right = right.GetInt32();
                }
            }

            if (result.Top < 0)
                throw new JsonException("Invalid value for Thickness.Top. Cannot be < 0");

            if (result.Left < 0)
                throw new JsonException("Invalid value for Thickness.Left. Cannot be < 0");

            if (result.Bottom < 0)
                throw new JsonException("Invalid value for Thickness.Bottom. Cannot be < 0");

            if (result.Right < 0)
                throw new JsonException("Invalid value for Thickness.Right. Cannot be < 0");

            return result;
        }

        private static JsonElement GetProperty(JsonElement element, string name)
        {
            return element.GetProperty(name);
        }

        private static JsonElement GetProperty(JsonElement element, string name1, string name2)
        {
            if (TryGetProperty(element, out var property, name1))
                return property;

            return element.GetProperty(name2);
        }

        private static JsonElement GetProperty(JsonElement element, string name1, string name2, string name3)
        {
            if (TryGetProperty(element, out var property, name1)
                || TryGetProperty(element, out property, name2))
            {
                return property;
            }

            return element.GetProperty(name3);
        }

        private static JsonElement GetProperty(JsonElement element, params string[] propertyNames)
        {
            for (var i = 0; i < propertyNames.Length - 1; i++)
            {
                if (TryGetProperty(element, out var property, propertyNames[i]))
                    return property;
            }

            return element.GetProperty(propertyNames[propertyNames.Length - 1]);
        }

        private static bool TryGetProperty(JsonElement element, out JsonElement property, string name)
        {
            if (element.TryGetProperty(name, out property))
                return true;

            property = default;
            return false;
        }

        private static bool TryGetProperty(JsonElement element, out JsonElement property, string name1, string name2)
        {
            if (element.TryGetProperty(name1, out property) || element.TryGetProperty(name2, out property))
                return true;

            property = default;
            return false;
        }

        private static bool TryGetProperty(JsonElement element, out JsonElement property, string name1, string name2, string name3)
        {
            if (element.TryGetProperty(name1, out property)
                || element.TryGetProperty(name2, out property)
                || element.TryGetProperty(name3, out property))
            {
                return true;
            }

            property = default;
            return false;
        }

        private static bool TryGetProperty(JsonElement element, out JsonElement property, params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                if (element.TryGetProperty(name, out property))
                    return true;
            }

            property = default;
            return false;
        }

        private static SpriteUpdateMode ConvertUpdateModeStringToEnum(string? updateMode)
        {
            if (updateMode is null)
                throw new JsonException("Invalid value for UpdateMode");

            return (SpriteUpdateMode)Enum.Parse(typeof(SpriteUpdateMode), updateMode, true);
        }
    }
}
