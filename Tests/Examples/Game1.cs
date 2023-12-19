using Examples.Scenes.CollisionTest;
using Examples.Scenes.TestMenu;
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
using Yarn;

namespace Examples
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatchState _drawState;
        private InputManager _input;
        private SinglePlayerActionManager _actions;
        private IResourceLoader _resources;
        private IGameLogger _logger = new ConsoleGameLogger();

        public Game1()
        {
            Window.AllowUserResizing = true;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 450;
            _graphics.ApplyChanges();
            IsMouseVisible = true;

            _resources = new ResourceLoader(Services);

            _input = new InputManager();
            _actions = new SinglePlayerActionManager((int)Actions.Count, _input);

            SetupActions();

            Services.AddService(_resources);
            Services.AddService(_logger);
            Services.AddService(_input);
            Services.AddService<IActionManager>(_actions);
        }

        protected override void LoadContent()
        {
            _drawState = new SpriteBatchState(this);
            var font = new SpriteFontWrapper(_resources.Load<SpriteFont>("Content/Fonts/UIFont"));

            var debugDisplay = new DebugDisplay(font);

            var dialogueData = _resources.Load<DialogueData>("Content/Dialogue/TestDialogue");

            var runner = new DialogueRunner(dialogueData.Localization);
            runner.Dialogue.AddProgram(dialogueData.YarnProgram);

            Services.AddService(_drawState);
            Services.AddService(debugDisplay);
            Services.AddService(runner);

            SceneManager.ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 450);

            var scene = TestMenuScene.Load(this);

            SceneManager.ChangeScene(scene);
        }

        protected override void Update(GameTime gameTime)
        {
            _input.Update();
            _actions.Update();

            SceneManager.Update(gameTime.GetElapsedSeconds());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SceneManager.Draw(_drawState);

            base.Draw(gameTime);
        }

        private void SetupActions()
        {
            var up = new SingleActionMap();
            up.Add(Keys.Up);
            up.Add(Keys.W);
            up.Add(Buttons.DPadUp);
            up.Add(Buttons.LeftThumbstickUp);

            var left = new SingleActionMap();
            left.Add(Keys.Left);
            left.Add(Keys.A);
            up.Add(Buttons.DPadLeft);
            up.Add(Buttons.LeftThumbstickLeft);

            var down = new SingleActionMap();
            down.Add(Keys.Down);
            down.Add(Keys.S);
            up.Add(Buttons.DPadDown);
            up.Add(Buttons.LeftThumbstickDown);

            var right = new SingleActionMap();
            right.Add(Keys.Right);
            right.Add(Keys.D);
            up.Add(Buttons.DPadRight);
            up.Add(Buttons.LeftThumbstickRight);

            var rotateLeft = new SingleActionMap();
            rotateLeft.Add(Keys.Q);
            rotateLeft.Add(Buttons.LeftShoulder);
            rotateLeft.Add(Buttons.LeftTrigger);

            var rotateRight = new SingleActionMap();
            rotateRight.Add(Keys.E);
            rotateRight.Add(Buttons.RightShoulder);
            rotateRight.Add(Buttons.RightTrigger);

            var accept = new SingleActionMap();
            accept.Add(Keys.Z);
            accept.Add(Keys.Space);
            accept.Add(Buttons.A);

            var cancel = new SingleActionMap();
            cancel.Add(Keys.X);
            cancel.Add(Buttons.B);

            var fastForward = new SingleActionMap();
            fastForward.Add(Keys.X);
            fastForward.Add(Buttons.B);

            var pageBack = new SingleActionMap();
            pageBack.Add(Keys.Escape);

            _actions.Actions[(int)Actions.Up] = up;
            _actions.Actions[(int)Actions.Left] = left;
            _actions.Actions[(int)Actions.Down] = down;
            _actions.Actions[(int)Actions.Right] = right;
            _actions.Actions[(int)Actions.RotateLeft] = rotateLeft;
            _actions.Actions[(int)Actions.RotateRight] = rotateRight;
            _actions.Actions[(int)Actions.Click] = accept;
            _actions.Actions[(int)Actions.Cancel] = cancel;
            _actions.Actions[(int)Actions.FastForward] = fastForward;
            _actions.Actions[(int)Actions.PageBack] = pageBack;
        }
    }
}
