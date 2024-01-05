using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.Resources;
using Precisamento.MonoGame.Scenes;
using Precisamento.MonoGame.UI2;
using Precisamento.MonoGame.UI2.Styling.Brushes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Scenes.GuiTest
{
    public static class GuiScene
    {
        public static Scene Load(Game game)
        {
            var resources = game.Services.GetService<IResourceLoader>();
            var input = game.Services.GetService<InputManager>();

            var font = new SpriteFontWrapper(resources.Load<SpriteFont>("Content/Fonts/UIFont"));
            var camera = new OrthographicCamera(SceneManager.ViewportAdapter);

            var label = new Label();
            label.Text = "Hello  GUI";
            label.Font = font;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.Padding = new Thickness(5);
            label.Background = new SolidBrush(Color.DarkRed);
            label.TextColor = Color.White;

            var button = new Button();
            button.Content = label;
            button.VerticalAlignment = VerticalAlignment.Stretch;
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            button.Background = new SolidBrush(Color.White);
            button.Border = new SolidBrush(Color.Green);

            button.Margin = new Thickness(100);
            button.BorderThickness = new Thickness(10);

            var panel = new Panel();
            panel.Background = new SolidBrush(Color.Black);
            panel.ChildrenList.Add(button);

            var gui = new GuiSystem(game, input, camera);
            gui.Root = panel;

            var world = new World();

            var scene = new Scene(
                world,
                new ISystem<float>[]
                {
                    new ActionSystem<float>(delta =>
                    {
                        gui.Update(delta);
                    })
                },
                new ISystem<SpriteBatchState>[]
                {
                    new ActionSystem<SpriteBatchState>(state =>
                    {
                        gui.Draw(state);
                    })
                },
                new ISystem<SpriteBatchState>[] 
                {
                });

            return scene;
        }
    }
}
