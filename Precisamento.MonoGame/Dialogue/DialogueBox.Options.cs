using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Dialogue.Options;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue
{
    public partial class DialogueBox
    {
        private void UpdateOption(float delta)
        {
            var movement = _optionSelectMove?.Invoke() ?? 0;
            var direction = Math.Sign(movement);

            if (movement != 0)
            {
                var moved = false;
                if (direction == _optionAutoScrollDirection)
                {
                    _optionAutoScrollTimer -= delta;
                    if (_optionAutoScrollTimer < 0)
                    {
                        moved = UpdateSelectedIndex(movement);
                        _optionAutoScrollTimer = moved ? _optionAutoScrollSecondaryWait : 100000;
                    }
                }
                else
                {
                    moved = UpdateSelectedIndex(movement);
                    _optionAutoScrollTimer = moved ? _optionAutoScrollInitialWait : 100000;
                }

                // Jump the view to the option if it's not visible.

                JumpToOption();

                if (moved)
                    Redraw();
            }

            _optionAutoScrollDirection = direction;

            var quickSelect = _optionQuickSelect?.Invoke() ?? -1;

            if (_optionSelected?.Invoke() ?? false)
            {
                OptionSelected(_options[_optionSelectedIndex].DialogueOptionId);
                return;
            }
            else if (_optionCanceled?.Invoke() ?? false)
            {
                var cancelOption = _options.FirstOrDefault(o => o.Line.Metadata.Contains("cancel"));
                if (cancelOption != null)
                {

                    OptionSelected(cancelOption.DialogueOptionId);
                    return;
                }
            }
            else if (quickSelect != -1)
            {
                OptionSelected(_options[quickSelect].DialogueOptionId);
                return;
            }

            if (_hasCustomProcessors || CheckScroll())
            {
                Redraw();
            }
        }

        private void OptionSelected(int optionId)
        {
            if (_isOptionWindow)
                _frames.Clear();
            _onOptionSelected?.Invoke(optionId);
        }

        private bool UpdateSelectedIndex(int movement)
        {
            var initialIndex = _optionSelectedIndex;

            var direction = Math.Sign(movement);
            var increment = direction * -1;
            for (; Math.Abs(movement) > 0; movement += increment)
            {
                _optionSelectedIndex += direction;
                if (_optionSelectedIndex < 0)
                {
                    if (_optionAllowScrollWrap)
                    {
                        _optionSelectedIndex = _options.Count - 1;
                    }
                    else
                    {
                        _optionSelectedIndex = initialIndex;
                        return false;
                    }
                }
                else if (_optionSelectedIndex >= _options.Count)
                {
                    if (_optionAllowScrollWrap)
                    {
                        _optionSelectedIndex = 0;
                    }
                    else
                    {
                        _optionSelectedIndex = initialIndex;
                        return false;
                    }
                }

                if (!_options[_optionSelectedIndex].IsAvailable)
                {
                    movement -= increment;
                }
            }

            return _optionSelectedIndex != initialIndex;
        }

        private void JumpToOption()
        {
            var frame = _options[_optionSelectedIndex].Line;
            var frameIndex = _frames.IndexOf(frame);

            if (frame.LineStart <= _startingLineIndex)
            {
                _startingLineIndex = frame.LineStart;
                return;
            }

            var startingFrameIndex = FindStartingFrame();
            var endingFrameIndex = startingFrameIndex;
            var totalHeight = 0;

            for (var i = startingFrameIndex + 1; i < _frames.Count; i++)
            {
                totalHeight += _frames[i].TotalSize.Height;
                if (totalHeight <= _state.DrawArea.Height)
                {
                    endingFrameIndex++;
                }
                else
                {
                    break;
                }
            }

            var endingFrame = _frames[endingFrameIndex];

            if (frame.LineStart + frame.Lines.Count > endingFrame.LineStart + endingFrame.Lines.Count)
            {
                _startingLineIndex = frame.LineStart + frame.Lines.Count - 1;
                totalHeight = 0;
                for (var i = frameIndex; i >= 0; i--)
                {
                    for (var line = _frames[i].Lines.Count - 1; line >= 0; line--)
                    {
                        totalHeight += _frames[i].Lines[line].Height;
                        if (totalHeight <= _state.DrawArea.Height)
                        {
                            _startingLineIndex = _frames[i].LineStart + line;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }

        private void SetOptionsForOptionsWindow(DialogueOption[] options, Rectangle parentBounds, Action<int> onOptionSelected)
        {
            _state.DrawArea = new Rectangle(
                _optionWindowPadding.Left,
                _optionWindowPadding.Top,
                _optionWindowMaxBounds.Width > 0
                    ? _optionWindowMaxBounds.Width - _optionWindowPadding.Width
                    : int.MaxValue / 2,
                _optionWindowMaxBounds.Height > 0
                    ? _optionWindowMaxBounds.Height - _optionWindowPadding.Height
                    : int.MaxValue / 2);

            PresentInlineOptions(options);

            _bounds.Size = MeasureOptions();
            CheckGenerateWindowTexture(_bounds.Size);
            RepositionWindow(parentBounds);

            Redraw();

            _onOptionSelected = onOptionSelected;
        }

        private Size MeasureOptions()
        {
            Size result = _optionWindowMinBounds - _optionWindowPadding.Size;
            result.Height = Math.Max(result.Height, _optionWindowPadding.Height);

            if (_optionWindowAlwaysUseMaxBounds)
            {
                if (_optionWindowMaxBounds.Width > 0 && _optionWindowMaxBounds.Height > 0)
                    return _optionWindowMaxBounds;

                if (_optionWindowMaxBounds.Width > 0)
                    result.Width = _optionWindowMaxBounds.Width;

                if (_optionWindowMaxBounds.Height > 0)
                    result.Height = _optionWindowMaxBounds.Height;
            }

            foreach (var option in _options.Select(o => o.Line))
            {
                result.Width = Math.Max(option.TotalSize.Width + _optionWindowPadding.Width, result.Width);
                if (_optionWindowMaxBounds.Width > 0)
                    result.Width = Math.Min(result.Width, _optionWindowMaxBounds.Width);

                //result.Height += option.TotalSize.Height + _margin;
                result.Height += option.TotalSize.Height;
                if (_optionWindowMaxBounds.Height > 0)
                    result.Height = Math.Min(result.Height, _optionWindowMaxBounds.Height);

                if (result == _optionWindowMaxBounds)
                    break;
            }

            return result;
        }

        private void RepositionWindow(Rectangle dialogueBounds)
        {
            Point position = dialogueBounds.Location;

            switch (_optionWindowLocation)
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
                    throw new InvalidOperationException($"Invalid render position: {_optionWindowLocation}");
            }

            _bounds.Location = position;
        }

        private void CheckGenerateWindowTexture(Size size)
        {
            var windowWidth = _texture?.Width ?? 0;
            var windowHeight = _texture?.Height ?? 0;
            if (size.Width > windowWidth || size.Height > windowHeight)
            {
                windowWidth = Math.Max(size.Width, windowWidth);
                windowHeight = Math.Max(size.Height, windowHeight);

                _texture?.Dispose();

                _texture = new RenderTarget2D(_game.GraphicsDevice, windowWidth, windowHeight);
            }
        }

        private Rectangle CalculateSelectedBackgroundBounds(Rectangle optionBounds)
        {
            optionBounds.X -= _optionSelectedBackgroundPadding.Left;
            optionBounds.Y -= _optionSelectedBackgroundPadding.Top;
            optionBounds.Width += _optionSelectedBackgroundPadding.Width;
            optionBounds.Height += _optionSelectedBackgroundPadding.Height;

            return optionBounds;
        }

        private Rectangle CalculateSelectIconBounds(Rectangle optionBounds)
        {
            var icon = _optionSelectedIconPlayer.CurrentFrame;
            if (icon is null)
                return Rectangle.Empty;

            var optionSize = _options[_optionSelectedIndex].Line.TotalSize;

            var x = optionBounds.X + _optionSelectedIconOffset.X;
            var y = optionBounds.Y + _optionSelectedIconOffset.Y;
            var width = icon.Width;
            var height = icon.Height;

            switch (_optionSelectedIconLocation)
            {
                case SelectIconLocation.Left:
                    x -= icon.Width;
                    y += (optionSize.Height - icon.Height) / 2;
                    break;
                case SelectIconLocation.Right:
                    x += optionSize.Width;
                    y += (optionSize.Height - icon.Height) / 2;
                    break;
                case SelectIconLocation.Top:
                    x += (optionSize.Width - icon.Width) / 2;
                    y -= icon.Height;
                    break;
                case SelectIconLocation.Bottom:
                    x += (optionSize.Width - icon.Width) / 2;
                    y += optionSize.Height;
                    break;
                case SelectIconLocation.LeftStretch:
                case SelectIconLocation.RightStretch:
                case SelectIconLocation.TopStretch:
                case SelectIconLocation.BottomStretch:
                    // Todo: Implement option select icon stretch.
                    throw new NotImplementedException("Option select icon stretch not yet implemented");
            }

            return new Rectangle(x, y, width, height);
        }
    }
}
