using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Components;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.MathHelpers;
using Precisamento.MonoGame.Resources;
using Precisamento.MonoGame.Scenes;
using Precisamento.MonoGame.Systems.Collisions;
using Precisamento.MonoGame.Systems.Debugging;
using Precisamento.MonoGame.Systems.Graphics;
using Precisamento.MonoGame.Systems.UI;
using Precisamento.MonoGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Scenes.CollisionTest
{
    public static class CollisionScene
    {
        public static Scene Load(Game game)
        {
            var resources = game.Services.GetService<IResourceLoader>();
            var input = game.Services.GetService<InputManager>();
            var actions = game.Services.GetService<IActionManager>();
            var debugDisplay = game.Services.GetService<DebugDisplay>();

            var debugBlue = new DebugColor { Color = Color.Blue };
            var debugRed = new DebugColor { Color = Color.Red };

            var collisionWorld = new SpatialHash(124);
            var world = new World();
            world.AddCollisions(collisionWorld);

            var player = world.CreateEntity();

            SetSquareCollider(player);
            player.Set<PlayerMoveComponent>();
            player.Set<VelocityComponent>(new VelocityComponent() { Delta = new Transform2(scale: Vector2.Zero) });
            player.Set(debugBlue);

            var wall = world.CreateEntity();
            var wallCollider = new BoxCollider(32, 256);
            wallCollider.Position = new Vector2(32, 128);
            wall.Set<Collider>(wallCollider);
            //wall.Set(debugRed);

            var wall2 = world.CreateEntity();
            var wall2Collider = new BoxCollider(256, 32);
            wall2Collider.Position = new Vector2(64, 352);
            wall2.Set<Collider>(wall2Collider);
            

            AddShapeSelectButtons(world, resources, player);

            var scene = new Scene(
                world,
                new ISystem<float>[]
                {
                    DebugCommandSystem.Create(game),
                    new ButtonListUpdateSystem(world, game),
                    new MoveSystem(game, world),
                    new MovementSystem(collisionWorld, world),
                    new CollisionResolutionSystem(collisionWorld, world),
                },
                new ISystem<SpriteBatchState>[]
                {
                    new AnimationDrawSystem(world),
                    new ButtonDrawSystem(world),
                    new ColliderDebugSystem(Color.Red, world),
                },
                new ISystem<SpriteBatchState>[]
                {
                    debugDisplay.CreateDebugDisplaySystem()
                });

            return scene;
        }

        private static void SetSquareCollider(Entity entity)
        {
            var box = new BoxCollider(48, 48);
            box.OriginalCenter = new Vector2(24);
            box.Position = GetPlayerPosition(entity);
            box.Rotation = GetPlayerRotation(entity);

            entity.Set<Collider>(box);
        }

        private static void SetCircleCollider(Entity entity)
        {
            var circle = new CircleCollider(24);
            circle.Position = GetPlayerPosition(entity);

            entity.Set<Collider>(circle);
        }

        private static void SetTriangleCollider(Entity entity)
        {
            // Equilateral Triangle pointing up with side lengths of 48 (radius of 24).
            var points = new Vector2[3];
            points[0] = MathExt.LengthDir(24, MathHelper.ToRadians(90));
            points[1] = MathExt.LengthDir(24, MathHelper.ToRadians(210));
            points[2] = MathExt.LengthDir(24, MathHelper.ToRadians(330));
            var triangle = new PolygonCollider(points, PolygonCollider.FindPolygonCenter(points));
            triangle.Position = GetPlayerPosition(entity);
            triangle.Rotation = GetPlayerRotation(entity);

            entity.Set<Collider>(triangle);
        }

        private static Vector2 GetPlayerPosition(Entity entity)
        {
            if (entity.Has<Collider>())
            {
                return entity.Get<Collider>().Position;
            }

            return new Vector2(32);
        }

        private static float GetPlayerRotation(Entity entity)
        {
            return entity.Has<Collider>() ? entity.Get<Collider>().Rotation : 0;
        }

        private static void AddShapeSelectButtons(World world, IResourceLoader resources, Entity playerEntity)
        {
            var font = new SpriteFontWrapper(resources.Load<SpriteFont>("Content/Fonts/UIFont"));
            var ui = resources.Load<Sprite>("Content/Sprites/UI");

            var square = new ButtonComponent(ui, "Normal", "Hover", "Press", null);
            var circle = new ButtonComponent(ui, "Normal", "Hover", "Press", null);
            var triangle = new ButtonComponent(ui, "Normal", "Hover", "Press", null);

            square.Clicked += () => SetSquareCollider(playerEntity);
            circle.Clicked += () => SetCircleCollider(playerEntity);
            triangle.Clicked += () => SetTriangleCollider(playerEntity);

            var buttons = new List<(ButtonComponent, string)> 
            { 
                (square, "Square"), 
                (circle, "Circle"), 
                (triangle, "Triangle")
            };

            var entities = new List<Entity>();

            for(var i = 0; i < buttons.Count; i++)
            {
                var (button, type) = buttons[i];
                var collider = new BoxCollider(200, 50)
                {
                    Position = new Vector2(500, 50 + i * 75)
                };

                var ninePatch = new NinePatchComponent(collider.BoundingBox.ToRectangle());
                var text = new TextComponent(font, type);
                var spritePlayer = new SpriteAnimationPlayer();
                spritePlayer.Animation = ui.Animations[button.CurrentState];

                var entity = world.CreateEntity();
                entity.Set(button);
                entity.Set<Collider>(collider);
                entity.Set(ninePatch);
                entity.Set(text);
                entity.Set(spritePlayer);

                entities.Add(entity);
            }

            var buttonList = new ButtonListComponent(entities);
            var listEntity = world.CreateEntity();
            listEntity.Set(buttonList);
        }
    }
}
