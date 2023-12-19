using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Dialogue;
using Precisamento.MonoGame.Dialogue.Options;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.Resources;
using Precisamento.MonoGame.Scenes;
using Precisamento.MonoGame.Systems.Debugging;
using Precisamento.MonoGame.Systems.Dialogue;
using Precisamento.MonoGame.Systems.Graphics;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground
{
    public static class SceneLoader
    {
        public static Scene LoadMainScene(Game game)
        {
            var resources = game.Services.GetService<IResourceLoader>();

            var world = new World();
            var input = game.Services.GetService<InputManager>();
            var actions = game.Services.GetService<IActionManager>();
            var dialogueRunner = game.Services.GetService<DialogueRunner>();

            CreateDialogueBox(game, world, resources, actions, input, dialogueRunner);

            var scene = new Scene(
                world,
                new ISystem<float>[]
                {
                    InputManager.CreateInputSystem(game),
                    new ActionSystem<float>(delta => dialogueRunner.Update()),
                    new DialogueUpdateSystem(world)
                },
                new ISystem<SpriteBatchState>[]
                {
                    new AnimationDrawSystem(world),
                    new DialogueDrawSystem(world),
                    new ColliderDebugSystem(Color.Red, world)
                },
                new ISystem<SpriteBatchState>[]
                {

                });

            return scene;
        }

        private static Entity CreateDialogueBox(Game game, World world, IResourceLoader resources, IActionManager actions, InputManager input, DialogueRunner runner)
        {
            var font = new SpriteFontWrapper(resources.Load<SpriteFont>("Content/Fonts/DebugFont"));
            // var borders = Sprite.FromJson("Content/Sprites/MainMenuButtons.sprite", resources);
            var borders = resources.Load<Sprite>("Content/Sprites/MainMenuButtons");

            var entity = world.CreateEntity();

            var dialogue = new DialogueBoxBuilder(game)
                .SetBackground(borders.Animations["Normal"].Frames[0])
                .SetBounds(new Rectangle(10, 10, 300, 100))
                .SetTextColor(Color.White)
                .SetPadding(new Thickness(10))
                .SetFont(font)
                .SetTextSpeed(2000)
                .SetContinuePressed(actions, (int)Actions.Accept)
                .SetFastForwardPressed(actions, (int)Actions.FastForward)
                .SetScroll(() =>
                {
                    if (input.MouseCheck(MouseButtons.WheelDown))
                        return -1;

                    if (input.MouseCheck(MouseButtons.WheelUp))
                        return 1;

                    return 0;
                })
                .SetDismiss(entity)
                .SetOptionLocation(DialogueOptionRenderLocation.Inline)
                .SetOptionBoxBackground(borders.Animations["Normal"].Frames[0])
                .SetOptionBoxPadding(new Thickness(25, 10, 12, 10))
                .SetOptionBoxOffset(new Point(0, 5))
                .SetOptionMargin(5)
                .SetOptionBackground(borders.Animations["SelectBackground"])
                .SetOptionBackgroundPadding(new Thickness(5, 1))
                .SetOptionSelectIcon(borders.Animations["Cursor"])
                .SetOptionSelectIconLocation(SelectIconLocation.Left)
                .SetOptionSelectIconOffset(new Point(-7, 0))
                .SetOptionMoveSelection(actions, (int)Actions.Up, (int)Actions.Down)
                .Build();

            dialogue.AttachToRunner(runner);

            entity.Set(dialogue);
            return entity;
        }
    }
}