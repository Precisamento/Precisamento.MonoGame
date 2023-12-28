using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
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
        private DialogueCharacterState _characterState;
        private List<List<CharacterDisplay>> _characterDisplay = new();
        private List<CharacterDisplay> _characters = new();
        private List<CharacterMoving> _movingCharacters = new();
        private Rectangle _dialogueBounds;
        private int _maxCharacters;
        private int _maxCharactersInLocation;
        private CharacterAddBehavior _characterAddBehavior;
        private CharacterSpeakerBehavior _characterSpeakerBehavior;
        private Color _nonSpeakerColor = new Color(150, 150, 150, 230);
        private DarkenNonSpeaker _darkenBehavior;
        private bool _moveSpeakerToFront;
        private bool _addToFront;
        private Point _speakerLineOffset;
        private List<CharacterLocation> _offsets;
        private CharacterLocation _defaultLocation;
        private bool _flipFacesOnRight;
        private bool _flipBackgroundsOnRight;
        private PortraitBounds _defaultPortraitBounds;

        private Tweener _tweener = new();
        private float _moveTime;

        public CharacterHandler(CharacterDisplayOptions options, DialogueCharacterState characterState)
        {
            _characterFactory = options.CharacterFactory!;
            _defaultPortraitBounds = options.DefaultPortraitBounds!;
            _defaultLocation = options.DefaultLocation!;
            _maxCharacters = options.MaxCharacters;
            _maxCharactersInLocation = options.MaxCharactersInAGivenLocation;
            _characterAddBehavior = options.AddBehavior;
            _characterSpeakerBehavior = options.SpeakerBehavior;
            _nonSpeakerColor = options.NonSpeakerColor;
            _darkenBehavior = options.DarkenNonSpeakerBehavior;
            _moveSpeakerToFront = options.MoveSpeakerToFront;
            _addToFront = options.LeftIsFront;
            _speakerLineOffset = options.MultipleSpeakerOffset;
            _offsets = options.DrawOffsets;
            _flipFacesOnRight = options.FlipFacesOnRight;
            _flipBackgroundsOnRight = options.FlipBackgroundsOnRight;
            _moveTime = options.MoveTime;
            _characterState = characterState;
        }

        public void Initialize(Rectangle dialogueBounds)
        {
            _dialogueBounds = dialogueBounds;
        }

        public void Reset()
        {
            _characterDisplay.Clear();
            _characters.Clear();
        }

        public void Update(float delta)
        {
            var updated = false;
            foreach (var character in _characterState.Adding)
            {
                AddCharacter(character);
                updated = true;
            }

            _characterState.Adding.Clear();

            foreach (var character in _characterState.Removing)
            {
                RemoveCharacter(character.Name);
                updated = true;
            }

            _characterState.Removing.Clear();

            updated |= _movingCharacters.Count > 0;
            _tweener.Update(delta);

            UpdateSpeakers();

            if (updated)
            {
                RemoveExcessCharacters();
                SetAllCharacterDrawBounds();
            }

            foreach(var character in _characters)
            {
                character.BackgroundAnimationPlayer.Update(delta);
                character.FaceAnimationPlayer.Update(delta);
            }
        }

        public void FastForward()
        {
            while(_movingCharacters.Count > 0)
            {
                _movingCharacters[^1].Tween.CancelAndComplete();
            }
        }

        public void Draw(SpriteBatchState state)
        {
            var defaultParams = new SpriteDrawParams() { Color = Color.White };
            var darkenParams = new SpriteDrawParams() { Color = _nonSpeakerColor };

            foreach(var character in _characters)
            {
                var bgParams = (!character.Speaking && _darkenBehavior.HasFlag(DarkenNonSpeaker.Background))
                    ? darkenParams
                    : defaultParams;

                var faceParams = (!character.Speaking && _darkenBehavior.HasFlag(DarkenNonSpeaker.Face))
                    ? darkenParams
                    : defaultParams;

                faceParams.Effects = character.Character.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                if (_flipFacesOnRight || _flipBackgroundsOnRight)
                {
                    var x = character.BackgroundBounds.IsEmpty ? character.FaceBounds.Center.X : character.BackgroundBounds.Center.X;
                    if (x != 0)
                    {
                        if (x >= _dialogueBounds.Center.X)
                        {
                            if (_flipFacesOnRight)
                            {
                                faceParams.Effects = faceParams.Effects == SpriteEffects.FlipHorizontally ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                            }

                            if (_flipBackgroundsOnRight)
                            {
                                bgParams.Effects = bgParams.Effects == SpriteEffects.FlipHorizontally ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                            }
                        }
                    }
                }

                character.BackgroundAnimationPlayer.Draw(state, character.BackgroundBounds, ref bgParams);
                character.FaceAnimationPlayer.Draw(state, character.FaceBounds, ref faceParams);
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
                        AddMoveCharacter(display, oldIndex);
                        break;
                    case CharacterAddBehavior.Ignore:
                        AddIgnoreCharacter(display, oldIndex);
                        break;
                    case CharacterAddBehavior.IgnoreUnlessExplicitlySet:
                        if (explicitLocation)
                            AddOverwriteCharacter(display, oldIndex);
                        else
                            AddIgnoreCharacter(display, oldIndex);
                        break;
                    case CharacterAddBehavior.IgnoreOrMoveIfExplicitySet:
                        if (explicitLocation)
                            AddMoveCharacter(display, oldIndex);
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

        private void AddMoveCharacter(CharacterDisplay character, int characterListIndex)
        {
            var oldCharacter = _characters[characterListIndex];
            AddOverwriteCharacter(character, characterListIndex);
            var oldLocation = oldCharacter.Character.Location ?? _defaultLocation;
            var newLocation = character.Character.Location ?? _defaultLocation;
            if (oldLocation == newLocation)
                return;

            GetDrawBounds(character, 0, out var newFaceBounds, out var newBackgroundBounds);

            var oldBounds = oldCharacter.BackgroundBounds.IsEmpty ? oldCharacter.FaceBounds : oldCharacter.BackgroundBounds;
            var currentlyMoving = _movingCharacters.Find(m => m.Character.Character.Profile.Name == character.Character.Profile.Name);
            currentlyMoving?.Tween.Cancel();

            var bounds = newBackgroundBounds.IsEmpty ? newFaceBounds : newBackgroundBounds;

            var moving = new CharacterMoving()
            {
                Position = currentlyMoving?.Position ?? oldBounds.Location.ToVector2(),
                Character = character,
                FinalFaceBounds = newFaceBounds,
                FinalBackgroundBounds = newBackgroundBounds
            };

            moving.Tween = _tweener
                .TweenTo(moving, moving => moving.Position, bounds.Location.ToVector2(), _moveTime)
                .Easing(EasingFunctions.QuinticInOut)
                .OnEnd(t => {
                    _movingCharacters.Remove(moving);
                    var list = GetCharactersAtLocation(newLocation!);
                    SetCharacterDrawBounds(character, list.IndexOf(character));
                });

            _movingCharacters.Add(moving);
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

        private void SetAllCharacterDrawBounds()
        {
            foreach (var list in _characterDisplay)
            {
                var i = 0;
                foreach (var character in list)
                {
                    SetCharacterDrawBounds(character, i++);
                }
            }
        }

        private void SetCharacterDrawBounds(CharacterDisplay display, int index)
        {
            var asMoving = _movingCharacters.FirstOrDefault(moving => moving.Character == display);
            if (asMoving != null)
            {
                var movePosition = asMoving.Position.ToPoint();
                if (!asMoving.FinalFaceBounds.IsEmpty && !asMoving.FinalBackgroundBounds.IsEmpty)
                {
                    var offset = asMoving.FinalFaceBounds.Location - asMoving.FinalBackgroundBounds.Location;

                    var faceBounds = asMoving.FinalFaceBounds;
                    faceBounds.Location = movePosition + offset;
                    display.FaceBounds = faceBounds;

                    var backgroundBounds = asMoving.FinalBackgroundBounds;
                    backgroundBounds.Location = movePosition;
                    display.BackgroundBounds = backgroundBounds;
                }
                else if (!asMoving.FinalFaceBounds.IsEmpty)
                {
                    var faceBounds = asMoving.FinalFaceBounds;
                    faceBounds.Location = movePosition;
                    display.FaceBounds = faceBounds;
                }
                else if (!asMoving.FinalBackgroundBounds.IsEmpty)
                {
                    var backgroundBounds = asMoving.FinalBackgroundBounds;
                    backgroundBounds.Location = movePosition;
                    display.BackgroundBounds = backgroundBounds;
                }
            }
            else
            {
                GetDrawBounds(display, index, out var faceBounds, out var backgroundBounds);
                display.FaceBounds = faceBounds;
                display.BackgroundBounds = backgroundBounds;
            }
        }

        private PortraitBounds GetDisplayPortraitBounds(CharacterDisplay display)
        {
            return display.Character.Profile.PortraitBounds ?? _defaultPortraitBounds;
        }

        private void GetDrawBounds(CharacterDisplay display, int index, out Rectangle faceBounds, out Rectangle backgroundBounds)
        {
            var faceFrame = display.FaceAnimationPlayer.CurrentFrame;
            var backgroundFrame = display.BackgroundAnimationPlayer.CurrentFrame;
            var location = display.Character.Location ?? _defaultLocation;
            var offset = _offsets.FirstOrDefault(l => l.RenderLocation == location!.RenderLocation)?.ExactLocation ?? Point.Zero;

            offset.X += _speakerLineOffset.X * index;
            offset.Y += _speakerLineOffset.Y * index;

            if (backgroundFrame != null)
            {
                var bounds = GetDisplayPortraitBounds(display);
                Size backgroundSize = MeasureDisplay(display);

                var backgroundPosition = WindowPositioning.PositionAround(_dialogueBounds, backgroundSize, location!.RenderLocation, offset);

                backgroundBounds = new Rectangle(
                    backgroundPosition,
                    backgroundSize);

                if (faceFrame != null)
                {
                    faceBounds = new Rectangle(
                        backgroundPosition.X + bounds.Padding.Left,
                        backgroundPosition.Y + bounds.Padding.Top,
                        backgroundSize.Width - bounds.Padding.Width,
                        backgroundSize.Height - bounds.Padding.Height);
                }
                else
                {
                    faceBounds = Rectangle.Empty;
                }
            }
            else if (faceFrame != null)
            {
                var position = WindowPositioning.PositionAround(_dialogueBounds, (Size)faceFrame.Size, location!.RenderLocation, offset);

                faceBounds = new Rectangle(
                    position,
                    (Point)faceFrame.Size);

                backgroundBounds = Rectangle.Empty;
            }
            else
            {
                faceBounds = Rectangle.Empty;
                backgroundBounds = Rectangle.Empty;
            }
        }

        private Size MeasureDisplay(CharacterDisplay display)
        {
            var faceFrame = display.FaceAnimationPlayer.CurrentFrame;
            var backgroundFrame = display.BackgroundAnimationPlayer.CurrentFrame;

            if (backgroundFrame != null)
            {
                var bounds = GetDisplayPortraitBounds(display);
                Size backgroundSize = new();
                switch (bounds.BoundsType)
                {
                    case PortraitBoundsType.ExactSize:
                        backgroundSize = bounds.Size;
                        break;
                    case PortraitBoundsType.SpriteSize:
                        backgroundSize = (Size?)backgroundFrame?.Size ?? new Size();
                        backgroundSize += bounds.Padding.Size;
                        break;
                    case PortraitBoundsType.SurroundImage:
                        backgroundSize = (Size?)faceFrame?.Size ?? new Size();
                        backgroundSize += bounds.Padding.Size;
                        break;
                }

                return backgroundSize;
            }
            else if (faceFrame != null)
            {
                return (Size)faceFrame.Size;
            }

            return Size.Empty;
        }

        private void RemoveExcessCharacters()
        {
            while (_characters.Count > _maxCharacters)
            {
                var removing = _characters[0];
                var list = GetCharactersAtLocation(removing.Character.Location ?? _defaultLocation!);
                list.Remove(removing);
                _characters.RemoveAt(0);
            }

            foreach(var list in _characterDisplay)
            {
                while (list.Count > _maxCharactersInLocation)
                {
                    // Get the character who spoke the longest time ago.
                    var removing = list.MinBy(cd => _characters.IndexOf(cd));
                    
                    list.Remove(removing!);
                    _characters.Remove(removing!);
                }
            }
        }

        private List<CharacterDisplay> GetCharactersAtLocation(CharacterLocation location)
        {
            switch(location.RenderLocation)
            {
                case DialogueOptionRenderLocation.List:
                case DialogueOptionRenderLocation.Inline:
                case DialogueOptionRenderLocation.AboveLeft:
                case DialogueOptionRenderLocation.AboveCenter:
                case DialogueOptionRenderLocation.AboveRight:
                case DialogueOptionRenderLocation.BelowLeft:
                case DialogueOptionRenderLocation.BelowCenter:
                case DialogueOptionRenderLocation.BelowRight:
                case DialogueOptionRenderLocation.LeftTop:
                case DialogueOptionRenderLocation.LeftCenter:
                case DialogueOptionRenderLocation.LeftBottom:
                case DialogueOptionRenderLocation.RightTop:
                case DialogueOptionRenderLocation.RightCenter:
                case DialogueOptionRenderLocation.RightBottom:
                {
                    var list = _characterDisplay.FirstOrDefault(l => l.Count > 0 && l[0].Character.Location?.RenderLocation == location.RenderLocation);

                    if (list is null)
                    {
                        list = new List<CharacterDisplay>();
                        _characterDisplay.Add(list);
                    }

                    return list;
                }
                case DialogueOptionRenderLocation.CustomCenterPosition:
                case DialogueOptionRenderLocation.CustomTopLeftPosition:
                case DialogueOptionRenderLocation.CustomTopRightPosition:
                case DialogueOptionRenderLocation.CustomBottomLeftPosition:
                case DialogueOptionRenderLocation.CustomBottomRightPosition:
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
