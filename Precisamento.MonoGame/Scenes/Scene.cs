using DefaultEcs;
using DefaultEcs.Command;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Scenes
{
    public class Scene : IDisposable
    {
        private SequentialSystem<float> _update;
        private SequentialSystem<SpriteBatchState> _draw;
        private SequentialSystem<SpriteBatchState> _gui;
        private EntityCommandRecorder? _recorder;
        private bool _disposed;

        public World World { get; }
        public Camera<Vector2> Camera { get; }
        public EntityCommandRecorder Recorder
        {
            get
            {
                if (_recorder is null)
                    _recorder = new EntityCommandRecorder();
                return _recorder;
            }
        }

        public Scene(World world,
                     ISystem<float>[] updateSystems,
                     ISystem<SpriteBatchState>[] drawSystems,
                     ISystem<SpriteBatchState>[] guiSystems)
        {
            World = world;
            _update = new SequentialSystem<float>(updateSystems);
            _draw = new SequentialSystem<SpriteBatchState>(drawSystems);
            _gui = new SequentialSystem<SpriteBatchState>(guiSystems);
            Camera = new OrthographicCamera(SceneManager.ViewportAdapter);
        }

        public Scene(World world,
                     ISystem<float>[] updateSystems,
                     ISystem<SpriteBatchState>[] drawSystems,
                     ISystem<SpriteBatchState>[] guiSystems,
                     Camera<Vector2> camera)
        {
            World = world;
            _update = new SequentialSystem<float>(updateSystems);
            _draw = new SequentialSystem<SpriteBatchState>(drawSystems);
            _gui = new SequentialSystem<SpriteBatchState>(guiSystems);
            Camera = camera;
        }

        public void Update(float delta)
        {
            _update.Update(delta);

            Recorder.Execute();
        }

        public void Draw(SpriteBatchState state)
        {
            state.TransformMatrix = Camera.GetViewMatrix();

            state.Begin();

            _draw.Update(state);

            state.End();

            state.TransformMatrix = SceneManager.ViewportAdapter.GetScaleMatrix();

            state.Begin();

            _gui.Update(state);

            state.End();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _update.Dispose();
                    _draw.Dispose();
                    _gui.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
