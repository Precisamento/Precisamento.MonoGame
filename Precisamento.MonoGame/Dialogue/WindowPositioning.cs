using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.Dialogue.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue
{
    public class WindowPositioning
    {
        public static Point PositionAround(Rectangle bounds, Size windowSize, DialogueOptionRenderLocation renderLocation, Point offset)
        {
            var position = bounds.Location;

            switch (renderLocation)
            {
                case DialogueOptionRenderLocation.AboveLeft:
                    position.Y -= windowSize.Height;
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.AboveCenter:
                    position.Y -= windowSize.Height;
                    position.X += (bounds.Width - windowSize.Width) / 2;
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.AboveRight:
                    position.Y -= windowSize.Height;
                    position.X += bounds.Width - windowSize.Width;
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.BelowLeft:
                    position.Y += bounds.Height;
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.BelowCenter:
                    position.Y += bounds.Height;
                    position.X += (bounds.Width - windowSize.Width) / 2;
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.BelowRight:
                    position.Y += bounds.Height;
                    position.X += bounds.Width - windowSize.Width;
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.LeftTop:
                    position.X -= windowSize.Width;
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.LeftCenter:
                    position.X -= windowSize.Width;
                    position.Y += (bounds.Height - windowSize.Height) / 2;
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.LeftBottom:
                    position.X -= windowSize.Width;
                    position.Y += (bounds.Height - windowSize.Height);
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.RightTop:
                    position.Y += bounds.Width;
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.RightCenter:
                    position.Y += bounds.Width;
                    position.Y += (bounds.Height - windowSize.Height) / 2;
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.RightBottom:
                    position.Y += bounds.Width;
                    position.Y += (bounds.Height - windowSize.Height);
                    position += offset;
                    break;
                case DialogueOptionRenderLocation.CustomTopLeftPosition:
                    position = offset;
                    break;
                case DialogueOptionRenderLocation.CustomTopRightPosition:
                    position.X = offset.X - windowSize.Width;
                    position.Y = offset.Y;
                    break;
                case DialogueOptionRenderLocation.CustomBottomLeftPosition:
                    position.X = offset.X;
                    position.Y = offset.Y - windowSize.Height;
                    break;
                case DialogueOptionRenderLocation.CustomBottomRightPosition:
                    position.X = offset.X - windowSize.Width;
                    position.Y = offset.Y - windowSize.Height;
                    break;
                case DialogueOptionRenderLocation.CustomCenterPosition:
                    position.X = offset.X - windowSize.Width / 2;
                    position.Y = offset.Y - windowSize.Height / 2;
                    break;
                default:
                    break;
            }

            return position;
        }
    }
}
