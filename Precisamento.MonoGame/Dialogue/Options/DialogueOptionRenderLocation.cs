using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Dialogue.Options
{
    public enum DialogueOptionRenderLocation
    {
        /// <summary>
        /// Shows dialogue options inside of the dialogue box
        /// </summary>
        Inline,

        /// <summary>
        /// Shows dialogue options in a floating window above and 
        /// on the left side of the dialogue box.
        /// </summary>
        AboveLeft,

        /// <summary>
        /// Shows dialogue options in a floating window above and 
        /// in the center of the dialogue box.
        /// </summary>
        AboveCenter,

        /// <summary>
        /// Shows dialogue options in a floating window above and 
        /// on the right side of the dialogue box.
        /// </summary>
        AboveRight,

        /// <summary>
        /// Shows dialogue options in a floating window below and 
        /// on the left side of the dialogue box.
        /// </summary>
        BelowLeft,

        /// <summary>
        /// Shows dialogue options in a floating window below and 
        /// in the center of the dialogue box.
        /// </summary>
        BelowCenter,

        /// <summary>
        /// Shows dialogue options in a floating window below and 
        /// on the right side of the dialogue box.
        /// </summary>
        BelowRight,

        LeftTop,
        LeftCenter,
        LeftBottom,
        RightTop,
        RightCenter,
        RightBottom,

        /// <summary>
        /// Shows dialogue options in a floating window at a custom
        /// position defined by <see cref="DialogueBoxOptions.OptionBoxOffset"/>
        /// as the center position.
        /// </summary>
        CustomCenterPosition,

        /// <summary>
        /// Shows dialogue options in a floating window at a custom
        /// position defined by <see cref="DialogueBoxOptions.OptionBoxOffset"/>
        /// as the top left position.
        /// </summary>
        CustomTopLeftPosition,

        /// <summary>
        /// Shows dialogue options in a floating window at a custom
        /// position defined by <see cref="DialogueBoxOptions.OptionBoxOffset"/>
        /// as the top right position.
        /// </summary>
        CustomTopRightPosition,

        /// <summary>
        /// Shows dialogue options in a floating window at a custom
        /// position defined by <see cref="DialogueBoxOptions.OptionBoxOffset"/>
        /// as the bottom left position.
        /// </summary>
        CustomBottomLeftPosition,

        /// <summary>
        /// Shows dialogue options in a floating window at a custom
        /// position defined by <see cref="DialogueBoxOptions.OptionBoxOffset"/>
        /// as the bottom right position.
        /// </summary>
        CustomBottomRightPosition,

        /// <summary>
        /// Shows dialogue options as a list in the center of the screen
        /// in a style most commmonly aassociated with Visual Novels
        /// </summary>
        List,

        /// <summary>
        /// Uses a custom render method passed through <see cref="DialogueBoxOptions"/>
        /// to render the dialogue options.
        /// </summary>
        CustomRenderMethod
    }
}
