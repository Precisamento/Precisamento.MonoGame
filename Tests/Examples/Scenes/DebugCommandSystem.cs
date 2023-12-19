using DefaultEcs.System;
using Examples.Scenes.TestMenu;
using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Scenes
{
    public static class DebugCommandSystem
    {
        public static ActionSystem<float> Create(Game game)
        {
            var actions = game.Services.GetService<IActionManager>();
            return new ActionSystem<float>(delta =>
            {
                if (actions.ActionCheckPressed((int)Actions.PageBack))
                {
                    SceneManager.PopScene();
                }
            });
        }
    }
}
