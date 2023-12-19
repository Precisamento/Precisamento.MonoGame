using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Components;
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
using Precisamento.MonoGame.Systems.UI;
using Precisamento.MonoGame.UI;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Scenes.DialogueTest
{
    public static class DialogueScene
    {
        public static Scene Load(Game game)
        {
            var resources = game.Services.GetService<IResourceLoader>();
            var input = game.Services.GetService<InputManager>();
            var actions = game.Services.GetService<IActionManager>();
            var runner = game.Services.GetService<DialogueRunner>();

            var font = new SpriteFontWrapper(resources.Load<SpriteFont>("Content/Fonts/UIFont"));

            var world = new World();
            var title = world.CreateEntity();
            var titleText = new TextComponent(font, "Dialogue Screen");
            var origin = titleText.Size / 2;
            origin.Round();
            var titleParams = new TextDrawParams() { Origin = origin, Color = Color.Black };
            var titlePosition = new Transform2(new Vector2(400, 20));
            title.Set(titleText);
            title.Set(titleParams);
            title.Set(titlePosition);

            CreateDialogueBox(game, world, resources, actions, input, runner);
            ShowDialogueOptions(game, world, resources, runner);

            var scene = new Scene(
                world,
                new ISystem<float>[]
                {
                    DebugCommandSystem.Create(game),
                    new ButtonListUpdateSystem(world, game),
                    new ActionSystem<float>(delta => runner.Update()),
                    new DialogueUpdateSystem(world),
                },
                new ISystem<SpriteBatchState>[]
                {
                    new AnimationDrawSystem(world),
                    new DialogueDrawSystem(world),
                    new ColliderDebugSystem(Color.Red, world),
                    new WorldTextDrawSystem(world),
                    new ButtonDrawSystem(world)
                },
                new ISystem<SpriteBatchState>[]
                {

                });

            return scene;
        }

        private static void ShowDialogueOptions(Game game, World world, IResourceLoader resources, DialogueRunner runner)
        {
            var font = new SpriteFontWrapper(resources.Load<SpriteFont>("Content/Fonts/UIFont"));
            var ui = resources.Load<Sprite>("Content/Sprites/UI");

            var title = world.CreateEntity();
            var titleText = new TextComponent(font, "Select a dialogue node to run");
            var origin = titleText.Size / 2;
            origin.Round();
            var titleParams = new TextDrawParams() { Origin = origin, Color = Color.Black };
            var titlePosition = new Transform2(new Vector2(400, 40));
            title.Set(titleText);
            title.Set(titleParams);
            title.Set(titlePosition);

            var listEntity = world.CreateEntity();
            var buttons = new List<Entity>();
            var i = 0;

            foreach (var node in runner.Dialogue.NodeNames)
            {
                var button = new ButtonComponent(ui, "Normal", "Hover", "Press");
                button.Clicked += () =>
                {
                    var recorder = SceneManager.CurrentScene.Recorder;
                    recorder.Record(listEntity).Dispose();
                    recorder.Record(title).Dispose();
                    foreach (var buttonEntity in buttons)
                    {
                        recorder.Record(buttonEntity).Dispose();
                    }
                    runner.Start(node);
                };

                var collider = new BoxCollider(200, 40);
                collider.Position = new Vector2(300, 60 + i * 50);

                var ninePatch = new NinePatchComponent(collider.BoundingBox.ToRectangle());

                var text = new TextComponent(font, node);

                var spritePlayer = new SpriteAnimationPlayer();
                spritePlayer.Animation = ui.Animations[button.CurrentState];

                var entity = world.CreateEntity();
                entity.Set(button);
                entity.Set<Collider>(collider);
                entity.Set(ninePatch);
                entity.Set(text);
                entity.Set(spritePlayer);

                buttons.Add(entity);

                i++;
            }

            var buttonList = new ButtonListComponent(
                buttons,
                (int)Actions.Up,
                (int)Actions.Down,
                (int)Actions.Click);

            listEntity.Set(buttonList);
        }

        private static Entity CreateDialogueBox(Game game, World world, IResourceLoader resources, IActionManager actions, InputManager input, DialogueRunner runner)
        {
            var font = new SpriteFontWrapper(resources.Load<SpriteFont>("Content/Fonts/UIFont"));
            var borders = resources.Load<Sprite>("Content/Sprites/UI");

            var entity = world.CreateEntity();

            var dialogue = new DialogueBoxBuilder(game)
                .SetBackground(borders.Animations["Normal"].Frames[0])
                .SetBounds(new Rectangle(10, 10, 300, 100))
                .SetTextColor(Color.White)
                .SetPadding(new Thickness(10))
                .SetFont(font)
                .SetTextSpeed(2000)
                .SetContinuePressed(actions, (int)Actions.Click)
                .SetFastForwardPressed(actions, (int)Actions.FastForward)
                .SetScroll(() =>
                {
                    if (input.MouseCheck(MouseButtons.WheelDown))
                        return -1;

                    if (input.MouseCheck(MouseButtons.WheelUp))
                        return 1;

                    return 0;
                })
                .SetDismiss(() =>
                {
                    ShowDialogueOptions(game, world, resources, runner);
                })
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
