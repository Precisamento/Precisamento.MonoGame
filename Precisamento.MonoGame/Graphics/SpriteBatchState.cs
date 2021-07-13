using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Precisamento.MonoGame.Graphics
{
    public class SpriteBatchState
    {
        private BlendState _blend;
        private SpriteSortMode _sortMode;
        private SamplerState _samplerState = SamplerState.PointClamp;
        private DepthStencilState _depthStencilState;
        private RasterizerState _rasterizerState;
        private Effect _effect;
        private Matrix? _transformMatrix;
        private Matrix? _savedTransform;
        private Viewport? _savedViewport;

        public bool Drawing { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        public BlendState Blend
        {
            get => _blend;
            set
            {
                _blend = value;
                ApplySettings();
            }
        }

        public SpriteSortMode SortMode
        {
            get => _sortMode;
            set
            {
                _sortMode = value;
                ApplySettings();
            }
        }

        public SamplerState SamplerState
        {
            get => _samplerState;
            set
            {
                _samplerState = value;
                ApplySettings();
            }
        }

        public DepthStencilState DepthStencilState
        {
            get => _depthStencilState;
            set
            {
                _depthStencilState = value;
                ApplySettings();
            }
        }

        public RasterizerState RasterizerState
        {
            get => _rasterizerState;
            set
            {
                _rasterizerState = value;
                ApplySettings();
            }
        }

        public Effect Effect
        {
            get => _effect;
            set
            {
                _effect = value;
                ApplySettings();
            }
        }

        public Matrix? TransformMatrix
        {
            get => _transformMatrix;
            set
            {
                _transformMatrix = value;
                ApplySettings();
            }
        }

        public SpriteBatchState(Game game)
        {
            GraphicsDevice = game.GraphicsDevice;
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public void Begin()
        {
            SpriteBatch.Begin(SortMode, Blend, SamplerState, DepthStencilState, RasterizerState, Effect, TransformMatrix);
            Drawing = true;
        }

        public void End()
        {
            Drawing = false;
            SpriteBatch.End();
        }

        public void SetRenderTarget(RenderTarget2D renderTarget)
        {
            var state = Drawing;

            if (Drawing)
                End();

            if (renderTarget == null && _savedTransform != null)
            {
                _transformMatrix = _savedTransform;
                GraphicsDevice.Viewport = _savedViewport.Value;
                _savedViewport = null;
                _savedTransform = null;
            }
            else
            {
                _savedViewport = GraphicsDevice.Viewport;
                _savedTransform = _transformMatrix;
                _transformMatrix = null;
            }

            SpriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);

            if (state)
                Begin();
        }

        private void ApplySettings()
        {
            if (Drawing)
            {
                End();
                Begin();
            }
        }
    }
}
