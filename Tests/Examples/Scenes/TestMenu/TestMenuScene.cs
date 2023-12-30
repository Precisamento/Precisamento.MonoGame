using DefaultEcs;
using DefaultEcs.System;
using Examples.Scenes.CollisionTest;
using Examples.Scenes.DialogueTest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Resources;
using Precisamento.MonoGame.Scenes;
using Precisamento.MonoGame.Systems.Debugging;
using Precisamento.MonoGame.Systems.Graphics;
using Precisamento.MonoGame.Systems.UI;
using Precisamento.MonoGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Scenes.TestMenu
{
    public static class TestMenuScene
    {
        public static Scene Load(Game game)
        {
            var resources = game.Services.GetService<IResourceLoader>();
            var debugDisplay = game.Services.GetService<DebugDisplay>();

            var font = new SpriteFontWrapper(resources.Load<SpriteFont>("Content/Fonts/UIFont"));
            var ui = resources.Load<Sprite>("Content/Sprites/UI");

            var world = new World();

            var actions = new List<(string, Func<Game, Scene>)>()
            {
                ("Simple Collisions", CollisionScene.Load),
                ("Dialogue", DialogueScene.Load)
            };

            var buttons = new List<Entity>();
            var i = 0;

            foreach(var (title, action) in actions)
            {
                var button = new ButtonComponent(ui, "Normal", "Hover", "Press", null);
                button.Clicked += () => SceneManager.PushScene(action(game));

                var collider = new BoxCollider(200, 50);
                collider.Position = new Vector2(300, 50 + i * 75);

                var ninePatch = new NinePatchComponent(collider.BoundingBox.ToRectangle());

                var text = new TextComponent(font, $"{title}");

                var spritePlayer = new SpriteAnimationPlayer();
                spritePlayer.Animation = ui.Animations[button.CurrentState];

                var entity = world.CreateEntity();
                entity.Set(button);
                entity.Set<Collider>(collider);
                entity.Set(ninePatch);
                entity.Set(text);
                entity.Set(spritePlayer);

                buttons.Add(entity);

                i += 1;
            }

            var listEntity = world.CreateEntity();
            var buttonList = new ButtonListComponent(
                buttons,
                (int)Actions.Up,
                (int)Actions.Down,
                (int)Actions.Click);

            listEntity.Set(buttonList);

            var scene = new Scene(
                world,
                new ISystem<float>[]
                {
                    new ButtonListUpdateSystem(world, game)
                },
                new ISystem<SpriteBatchState>[]
                {
                    new AnimationDrawSystem(world),
                    new ButtonDrawSystem(world),
                },
                new ISystem<SpriteBatchState>[]
                {
                    debugDisplay.CreateDebugDisplaySystem()
                });

            return scene;
        }
    }
}
