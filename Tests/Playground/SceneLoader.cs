using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Dialogue;
using Precisamento.MonoGame.Dialogue.Options;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.MathHelpers;
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
        public static Scene LoadTestScene(Game game)
        {
            var world = new World();

            var rect = new Rectangle(0, 0, 64, 32);
            var position = new Vector2(32, 32);
            var rotation = MathHelper.ToRadians(45);
            var origin = new Vector2(0, 16);
            var scale = new Vector2(2, 2);

            var baseTransform =
                Matrix.CreateTranslation(new Vector3(-origin, 0f)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateScale(new Vector3(scale, 1f)) *
                Matrix.CreateTranslation(new Vector3(origin, 0f));

            var translation = Matrix.CreateTranslation(new Vector3(position, 0f));

            var positionFirst = translation * baseTransform;

            var positionSecond = baseTransform * translation;

            var location = rect.Location.ToVector2();

            var pointsFirst = new List<Vector2>()
            {
                Vector2.Transform(location, positionFirst),
                Vector2.Transform(location + new Vector2(rect.Width, 0), positionFirst),
                Vector2.Transform(location + rect.Size.ToVector2(), positionFirst),
                Vector2.Transform(location + new Vector2(0, rect.Height), positionFirst),
                Vector2.Transform(location, positionFirst)
            };

            var pointsSecond = new List<Vector2>()
            {
                Vector2.Transform(location, positionSecond),
                Vector2.Transform(location + new Vector2(rect.Width, 0), positionSecond),
                Vector2.Transform(location + rect.Size.ToVector2(), positionSecond),
                Vector2.Transform(location + new Vector2(0, rect.Height), positionSecond),
                Vector2.Transform(location, positionSecond)
            };

            var scene = new Scene(
                world,
                new ISystem<float>[]
                {

                },
                new ISystem<SpriteBatchState>[]
                {
                    new ActionSystem<SpriteBatchState>((state) =>
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            state.SpriteBatch.DrawLine(pointsFirst[i], pointsFirst[i + 1], Color.Red);

                            state.SpriteBatch.DrawLine(pointsSecond[i], pointsSecond[i + 1], Color.Blue);
                        }


                        state.SpriteBatch.DrawCircle(new Vector2(32, 48), 5, 16, Color.Purple);
                    })
                },
                new ISystem<SpriteBatchState>[]
                {

                });

            return scene;
        }

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