using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Precisamento.MonoGame.Dialogue;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.Logging;
using Precisamento.MonoGame.Resources;
using Precisamento.MonoGame.Scenes;
using Precisamento.MonoGame.Systems.Debugging;
using Precisamento.MonoGame.YarnSpinner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Yarn;
using Yarn.Compiler;
using Yarn.Markup;

namespace Playground
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatchState _spriteBatchState;
        private IResourceLoader _loader;
        private IGameLogger _logger = new ConsoleGameLogger();
        //private string _text = "title: test\r\n---\r\n[color=red]Hello [color=blue]wor[/color]ld[/color]!\r\nThis is another line\r\nFinal test line.\r\nAnd they don't stop coming.\r\nOne more for good measure\r\nMuahaha. Out of bounds now\r\n===title: next\r\n---\r\nAnother simple string\r\nMoo\r\n===";
        private string _text = "title: test\r\n---\r\n[just hor=center /]Hello world. [color=blue]This is [font=\"Content/Fonts/Test\"]going to[/font] be a really [/color]long [just hor=left /]line of text that must keep going in order to wrap onto the next line. So let's see how long it goes. This seems like it's good enough. Need it to wrap through to one more line.\r\nThis is another line\r\n===";


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 450;
            _graphics.ApplyChanges();
            IsMouseVisible = true;

            _loader = new ResourceLoader(Services);
            Services.AddService(_loader);
            Services.AddService(_logger);

            SetupInput();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var font = new SpriteFontWrapper(_loader.Load<SpriteFont>("Content/Fonts/DebugFont"));

            var debugDisplay = new DebugDisplay(font);
            _spriteBatchState = new SpriteBatchState(this);
            var dialogueRunner = CreateDialogueRunner();

            Services.AddService(debugDisplay);
            Services.AddService(dialogueRunner);
            Services.AddService(_spriteBatchState);

            SceneManager.ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 400, 225);

            var scene = SceneLoader.LoadMainScene(this);

            SceneManager.ChangeScene(scene);

            dialogueRunner.Start("test");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            SceneManager.Update(gameTime.GetElapsedSeconds());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SceneManager.Draw(_spriteBatchState);

            base.Draw(gameTime);
        }

        private CompilationResult CompileYarnProgram(string programText)
        {
            var file = new CompilationJob.File() { FileName = "Test.txt", Source = programText };
            var job = new CompilationJob() { Files = new[] { file } };
            return Compiler.Compile(job); 
        }

        private DialogueRunner CreateDialogueRunner()
        {
            var result = CompileYarnProgram(_text);
            var locale = new YarnLocale("en-US", result.StringTable.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.text));
            var localization = new YarnLocalization();
            localization.BaseLocale = locale;
            localization.Locales[locale.Locale] = locale;
            foreach (var pair in result.StringTable)
                localization.Metadata.Add(pair.Key, pair.Value.metadata);

            var dialogueRunner = new DialogueRunner(localization);
            dialogueRunner.Dialogue.AddProgram(result.Program);
            dialogueRunner.LineNeedsPresented += (s, line) => Debug.WriteLine($"[Line] {line.Text.Text}");
            dialogueRunner.NodeStarted += (s, node) => Debug.WriteLine($"[NodeStart] {node}");
            dialogueRunner.NodeEnded += (s, node) => Debug.WriteLine($"[NodeEnd] {node}");
            dialogueRunner.DialogueCompleted += (s, e) => Debug.WriteLine("[End]");
            dialogueRunner.DialogueStarted += (s, e) => Debug.WriteLine("[Start]");

            dialogueRunner.CommandHandler.RegisterCommand(HelloCommand);

            return dialogueRunner;
        }

        private void SetupInput()
        {
            var input = new InputManager();
            var actionManager = new SinglePlayerActionManager((int)Actions.Count, input);

            var up = new SingleActionMap();
            up.Add(Keys.Up);

            var left = new SingleActionMap();
            left.Add(Keys.Left);

            var down = new SingleActionMap();
            down.Add(Keys.Down);

            var right = new SingleActionMap();
            right.Add(Keys.Right);

            var accept = new SingleActionMap();
            accept.Add(Keys.Z);

            var fastForward = new SingleActionMap().Add(Keys.X);

            actionManager.Actions[(int)Actions.Up] = up;
            actionManager.Actions[(int)Actions.Left] = left;
            actionManager.Actions[(int)Actions.Down] = down;
            actionManager.Actions[(int)Actions.Right] = right;
            actionManager.Actions[(int)Actions.Accept] = accept;
            actionManager.Actions[(int)Actions.FastForward] = fastForward;

            Services.AddService<InputManager>(input);
            Services.AddService<IActionManager>(actionManager);
        }

        private void DrawLetter(string text, int index, List<MarkupAttribute> attributes)
        {
            for (var i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].Position == index)
                {
                    if (attributes[i].Name == "color")
                    {
                        var value = attributes[i].Properties["color"].StringValue;
                        Debug.Write($"[color={value}]");
                    }
                }
            }

            Debug.Write(text[index]);

            for (var i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].Position + attributes[i].Length - 1 == index)
                {
                    Debug.Write($"[/{attributes[i].Name}]");
                }
            }
        }

        [YarnCommand("hello")]
        private void HelloCommand()
        {
            Debug.WriteLine("Hello from the console");
        }
    }
}
