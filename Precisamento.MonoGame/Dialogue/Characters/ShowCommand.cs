using Antlr4.Runtime.Misc;
using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Dialogue.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public class ShowCommand
    {
        private static Action<string, CharacterParams>[] _positionalSetters = new Action<string, CharacterParams>[]
        {
            SetSprite,
            SetLocation,
            SetFlipped,
            SetBackground
        };

        public static CharacterParams ParseShowCommand(string[] args, ICharacterProcessorFactory characterFactory)
        {
            if (args.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(args), "Must pass at least one argument to << show >>");

            var result = new CharacterParams()
            {
                Profile = characterFactory.GetCharacter(args[1])
            };

            var namedArgs = false;
            for (var i = 2; i < args.Length; i++)
            {
                if (args[i].Contains('='))
                    namedArgs = true;

                if (namedArgs)
                    ProcessShowNamedArgument(args[i], result);
                else
                    _positionalSetters[i - 2](args[i], result);
            }

            return result;
        }

        private static void ProcessShowNamedArgument(string argument, CharacterParams character)
        {
            var parts = argument.Split('=');
            switch (parts[0])
            {
                case "sprite":
                case "face":
                    SetSprite(parts[1], character);
                    break;
                case "bg":
                case "background":
                    SetBackground(parts[1], character);
                    break;
                case "flip":
                case "flipped":
                    SetFlipped(parts[1], character);
                    break;
                case "location":
                    SetLocation(parts[1], character);
                    break;
            }
        }

        private static void SetSprite(string value, CharacterParams character)
        {
            character.Sprite = value == "default" ? character.Profile.DefaultCharacterSprite : value;
        }

        private static void SetBackground(string value, CharacterParams character)
        {
            character.Background = value == "default" ? character.Profile.DefaultBackground : value;
        }

        private static void SetFlipped(string value, CharacterParams character)
        {
            character.Flipped = bool.Parse(value);
        }

        private static void SetLocation(string value, CharacterParams character)
        {
            character.Location = ParseLocation(value);
        }

        public static CharacterLocation ParseLocation(string value)
        {
            switch(value.ToLower())
            {
                case "left":
                    return new CharacterLocation(DialogueOptionRenderLocation.AboveLeft);
                case "right":
                    return new CharacterLocation(DialogueOptionRenderLocation.AboveRight);
                case "center":
                    return new CharacterLocation(DialogueOptionRenderLocation.AboveCenter);
            }

            if (Enum.TryParse<DialogueOptionRenderLocation>(value, true, out var result))
            {
                return new CharacterLocation(result);
            }

            var parts = value.Split(',');
            var point = new Point(int.Parse(parts[0]), int.Parse(parts[1]));
            return new CharacterLocation(DialogueOptionRenderLocation.CustomTopLeftPosition, point);
        }
    }
}
