using MonoGame.Extended.ViewportAdapters;
using Precisamento.MonoGame.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precisamento.MonoGame.Scenes
{
    public static class SceneManager
    {
        private static readonly List<Scene> _scenes = new List<Scene>();

        public static Scene CurrentScene => _scenes[_scenes.Count - 1];
        public static ViewportAdapter ViewportAdapter { get; set; }

        public static void PushScene(Scene scene)
        {
            AddScene(scene);
        }

        public static void ChangeScene(Scene scene)
        {
            while (_scenes.Count > 0)
                RemoveScene().Dispose();
            AddScene(scene);
        }

        public static void PopScene()
        {
            if (_scenes.Count != 0)
                RemoveScene().Dispose();
        }

        public static Scene PopSceneWithoutDisposing()
        {
            return RemoveScene();
        }

        public static bool ContainsScene(Scene scene)
        {
            return _scenes.Contains(scene);
        }

        public static void Update(float delta)
        {
            // If the scenes change during the update step,
            // update again. Otherwise, there is a noticable camera
            // jump from the first frame to the second.
            Scene scene;
            do
            {
                scene = _scenes[_scenes.Count - 1];
                scene.Update(delta);
            }
            while (scene != _scenes[_scenes.Count - 1]);
        }

        public static void Draw(SpriteBatchState state)
        {
            if (_scenes.Count > 0)
                _scenes[_scenes.Count - 1].Draw(state);
        }

        private static void AddScene(Scene scene)
        {
            _scenes.Add(scene);
        }

        private static Scene RemoveScene()
        {
            var scene = _scenes[_scenes.Count - 1];
            _scenes.RemoveAt(_scenes.Count - 1);
            return scene;
        }
    }
}
