using DefaultEcs;
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

        public World World { get; }
        public Camera<Vector2> Camera { get; }

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

        public void Dispose()
        {
            _update.Dispose();
            _draw.Dispose();
            _gui.Dispose();
        }
    }
}
