using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.Dialogue.Options;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.MathHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Characters
{
    public class CharacterHandler
    {
        private ICharacterProcessorFactory _characterFactory;
        private List<List<CharacterDisplay>> _characterDisplay = new();
        private List<CharacterDisplay> _characters = new();
        private DialogueCharacterState _characterState;
        private int _maxCharacters = 9999;
        private int _maxCharactersInLocation = 9999;
        private CharacterAddBehavior _characterAddBehavior = CharacterAddBehavior.Overwrite;
        private CharacterSpeakerBehavior _characterSpeakerBehavior = CharacterSpeakerBehavior.MostRecentOnly;
        private Color _nonSpeakerColor = new Color(150, 150, 150, 230);
        private bool _moveSpeakerToFront = false;
        private bool _addToFront = false;
        private bool _speakerDirection = false;
        private Point _speakerLineOffset = new Point(20, 0);
        private List<CharacterLocation> _offsets = new List<CharacterLocation>();
        private CharacterLocation? _defaultLocation;
        private bool _flipFacesOnRight = true;
        private bool _flipBackgroundsOnRight = false;
        private PortraitBounds _defaultPortraitBounds = new PortraitBounds()
        {
            BoundsType = PortraitBoundsType.SurroundImage,
            Padding = new Thickness(6),
        };

        public void Update(float delta)
        {
            foreach (var character in _characterState.Adding)
            {
                AddCharacter(character);
            }

            _characterState.Adding.Clear();

            foreach (var character in _characterState.Removing)
            {
                RemoveCharacter(character.Profile.Name);
            }

            _characterState.Removing.Clear();

            UpdateSpeakers();

            foreach(var character in _characters)
            {
                character.BackgroundAnimationPlayer.Update(delta);
                character.FaceAnimationPlayer.Update(delta);
            }
        }

        public void Draw(SpriteBatchState state)
        {
            foreach(var character in _characters)
            {
                var drawParams = new SpriteDrawParams()
                {
                    Color = character.Speaking ? Color.White : _nonSpeakerColor
                };

                character.BackgroundAnimationPlayer.Draw(state, character.BackgroundBounds);
                character.FaceAnimationPlayer.Draw(state, character.FaceBounds, ref drawParams);
            }
        }

        private void AddCharacter(CharacterParams character)
        {
            var explicitLocation = character.Location != null;
            character.Location ??= _defaultLocation;
            if (character.Location == null)
                throw new ArgumentException("Character must have a location");

            var oldIndex = _characters.FindLastIndex(c => c.Character.Profile.Name == character.Profile.Name);

            var display = new CharacterDisplay(character, true);
            var list = GetCharactersAtLocation(character.Location);

            if (oldIndex == -1)
            {
                AddNew(display);
            }
            else
            {
                switch(_characterAddBehavior)
                {
                    case CharacterAddBehavior.Overwrite:
                        AddOverwriteCharacter(display, oldIndex);
                        break;
                    case CharacterAddBehavior.Move:
                        throw new NotImplementedException();
                    case CharacterAddBehavior.Ignore:
                        AddIgnoreCharacter(display, oldIndex);
                        break;
                    case CharacterAddBehavior.IgnoreUnlessExplicitlySet:
                        if (explicitLocation)
                            AddOverwriteCharacter(display, oldIndex);
                        else
                            AddIgnoreCharacter(display, oldIndex);
                        break;
                    case CharacterAddBehavior.Multiply:
                        AddNew(display);
                        break;
                }
            }
        }

        private void AddNew(CharacterDisplay character)
        {
            var list = GetCharactersAtLocation(character.Character.Location!);

            if (_addToFront)
            {
                list.Insert(0, character);
            }
            else
            {
                list.Add(character);
            }

            _characters.Add(character);
            SetCharacterAnimations(character);
        }

        private void AddOverwriteCharacter(CharacterDisplay character, int characterListIndex)
        {
            var list = GetCharactersAtLocation(character.Character.Location!);
            var oldCharacter = _characters[characterListIndex];
            var oldList = GetCharactersAtLocation(oldCharacter.Character.Location!);

            if (oldList != list || _moveSpeakerToFront)
            {
                oldList.Remove(oldCharacter);
                if (_addToFront)
                    list.Insert(0, character);
                else
                    list.Add(character);
            }

            _characters.RemoveAt(characterListIndex);
            _characters.Add(character);
            SetCharacterAnimations(character);
        }

        private void AddIgnoreCharacter(CharacterDisplay character, int characterListIndex)
        {
            var oldCharacter = _characters[characterListIndex];
            var list = GetCharactersAtLocation(oldCharacter.Character.Location!);

            if (_moveSpeakerToFront)
            {
                list.Remove(oldCharacter);
                if (_addToFront)
                    list.Insert(0, oldCharacter);
                else
                    list.Add(oldCharacter);
            }

            _characters.RemoveAt(characterListIndex);
            _characters.Add(oldCharacter);
        }

        private void UpdateSpeakers()
        {
            switch (_characterSpeakerBehavior)
            {
                case CharacterSpeakerBehavior.MostRecentOnly:
                    _characters[^1].Speaking = true;
                    for(var i = _characters.Count - 2; i >= 0; i--)
                        _characters[i].Speaking = false;
                    break;
                case CharacterSpeakerBehavior.MostRecentInEachLocation:
                case CharacterSpeakerBehavior.MostRecentInEachLocationUpdateWithMove:
                    foreach (var list in _characterDisplay)
                    {
                        var found = false;
                        var start = _addToFront ? 0 : list.Count - 1;
                        var delta = _addToFront ? 1 : -1;
                        for(var i = start; i >= 0 && i < list.Count; i += delta)
                        {
                            if (list[i].Speaking)
                            {
                                if (found)
                                    list[i].Speaking = false;
                                found = true;
                            }
                        }

                        if (!found && _characterSpeakerBehavior == CharacterSpeakerBehavior.MostRecentInEachLocationUpdateWithMove)
                        {
                            list[start].Speaking = true;
                        }
                    }
                    break;
            }
        }

        private void RemoveCharacter(string name)
        {
            var index = _characters.FindLastIndex(c => c.Character.Profile.Name == name);
            if (index == -1)
                return;

            var character = _characters[index];
            var list = GetCharactersAtLocation(character.Character.Location!);

            list.Remove(character);
            _characters.RemoveAt(index);
        }

        private void SetCharacterAnimations(CharacterDisplay display)
        {
            var faceSprite = display.Character.Profile.CharacterSprite;
            var faceAnimation = display.Character.Sprite ?? display.Character.Profile.DefaultCharacterSprite;
            if (faceSprite != null && faceAnimation != null)
            {
                display.FaceAnimationPlayer.Animation = faceSprite.Animations[faceAnimation];
            }

            var bgSprite = display.Character.Profile.BackgroundSprite;
            var bgAnimation = display.Character.Background ?? display.Character.Profile.DefaultBackground;

            if (bgSprite != null && bgAnimation != null)
            {
                display.BackgroundAnimationPlayer.Animation = bgSprite.Animations[bgAnimation];
            }
        }

        private void SetCharacterDrawBounds(CharacterDisplay display)
        {
            var faceFrame = display.FaceAnimationPlayer.CurrentFrame;
            if (faceFrame != null)
            {
                var bounds = display.Character.Profile.PortraitBounds ?? _defaultPortraitBounds;
                Size2 backgroundSize;
                switch (bounds.BoundsType)
                {
                    case PortraitBoundsType.ExactSize:
                        backgroundSize = bounds.Size;
                        break;
                    case PortraitBoundsType.SurroundImage:
                        backgroundSize.Width = faceFrame.Width + bounds.Padding.Width;
                        backgroundSize.Height = faceFrame.Height + bounds.Padding.Height;
                        break;
                }
            }
        }

        private void RepositionWindow(Rectangle dialogueBounds, CharacterLocation location)
        {
            Point position = dialogueBounds.Location;

            switch (location.RenderLocation)
            {
                case DialogueOptionRenderLocation.AboveLeft:
                    position.Y -= _bounds.Height;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.AboveCenter:
                    position.Y -= _bounds.Height;
                    position.X += (dialogueBounds.Width - _bounds.Width) / 2;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.AboveRight:
                    position.Y -= _bounds.Height;
                    position.X += dialogueBounds.Width - _bounds.Width;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.BelowLeft:
                    position.Y += dialogueBounds.Height;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.BelowCenter:
                    position.Y += dialogueBounds.Height;
                    position.X += (dialogueBounds.Width - _bounds.Width) / 2;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.BelowRight:
                    position.Y += dialogueBounds.Height;
                    position.X += dialogueBounds.Width - _bounds.Width;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.LeftTop:
                    position.X -= _bounds.Width;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.LeftCenter:
                    position.X -= _bounds.Width;
                    position.Y += (dialogueBounds.Height - _bounds.Height) / 2;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.LeftBottom:
                    position.X -= _bounds.Width;
                    position.Y += (dialogueBounds.Height - _bounds.Height);
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.RightTop:
                    position.X += dialogueBounds.Width;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.RightCenter:
                    position.X += dialogueBounds.Width;
                    position.Y += (dialogueBounds.Height - _bounds.Height) / 2;
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.RightBottom:
                    position.X += dialogueBounds.Width;
                    position.Y += (dialogueBounds.Height - _bounds.Height);
                    position += _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.CustomTopLeftPosition:
                    position = _optionWindowOffset;
                    break;
                case DialogueOptionRenderLocation.CustomTopRightPosition:
                    position.X = _optionWindowOffset.X - _bounds.Width;
                    position.Y = _optionWindowOffset.Y;
                    break;
                case DialogueOptionRenderLocation.CustomBottomLeftPosition:
                    position.X = _optionWindowOffset.X;
                    position.Y = _optionWindowOffset.Y - _bounds.Height;
                    break;
                case DialogueOptionRenderLocation.CustomBottomRightPosition:
                    position.X = _optionWindowOffset.X - _bounds.Width;
                    position.Y = _optionWindowOffset.Y - _bounds.Height;
                    break;
                case DialogueOptionRenderLocation.CustomCenterPosition:
                    position.X = _optionWindowOffset.X - _bounds.Width / 2;
                    position.Y = _optionWindowOffset.Y - _bounds.Height / 2;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid render position: {location.RenderLocation}");
            }

            _bounds.Location = position;
        }

        private List<CharacterDisplay> GetCharactersAtLocation(CharacterLocation location)
        {
            switch(location.RenderLocation)
            {
                case Options.DialogueOptionRenderLocation.List:
                case Options.DialogueOptionRenderLocation.Inline:
                case Options.DialogueOptionRenderLocation.AboveLeft:
                case Options.DialogueOptionRenderLocation.AboveCenter:
                case Options.DialogueOptionRenderLocation.AboveRight:
                case Options.DialogueOptionRenderLocation.BelowLeft:
                case Options.DialogueOptionRenderLocation.BelowCenter:
                case Options.DialogueOptionRenderLocation.BelowRight:
                case Options.DialogueOptionRenderLocation.LeftTop:
                case Options.DialogueOptionRenderLocation.LeftCenter:
                case Options.DialogueOptionRenderLocation.LeftBottom:
                case Options.DialogueOptionRenderLocation.RightTop:
                case Options.DialogueOptionRenderLocation.RightCenter:
                case Options.DialogueOptionRenderLocation.RightBottom:
                {
                    var list = _characterDisplay.FirstOrDefault(l => l.Count > 0 && l[0].Character.Location?.RenderLocation == location.RenderLocation);

                    if (list is null)
                    {
                        list = new List<CharacterDisplay>();
                        _characterDisplay.Add(list);
                    }

                    return list;
                }
                case Options.DialogueOptionRenderLocation.CustomCenterPosition:
                case Options.DialogueOptionRenderLocation.CustomTopLeftPosition:
                case Options.DialogueOptionRenderLocation.CustomTopRightPosition:
                case Options.DialogueOptionRenderLocation.CustomBottomLeftPosition:
                case Options.DialogueOptionRenderLocation.CustomBottomRightPosition:
                {
                    var list = _characterDisplay.FirstOrDefault(l => l.Count > 0 && l[0].Character.Location == location);

                    if (list is null)
                    {
                        list = new List<CharacterDisplay>();
                        _characterDisplay.Add(list);
                    }

                    return list;
                }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
