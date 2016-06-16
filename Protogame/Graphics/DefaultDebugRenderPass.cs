using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultDebugRenderPass : IDebugRenderPass
    {
        private RasterizerState _oldState;

        private RasterizerState _debugState;

        private EffectAsset _basicEffect;

        public DefaultDebugRenderPass(IAssetManagerProvider assetManagerProvider)
        {
            _basicEffect = assetManagerProvider.GetAssetManager().Get<EffectAsset>("effect.Color");
            Lines = new List<VertexPositionNormalColor>();
            Triangles = new List<VertexPositionNormalColor>();
        }

        /// <summary>
        /// Gets a value indicating that this is not a post-processing render pass.
        /// </summary>
        /// <value>Always false.</value>
        public bool IsPostProcessingPass
        {
            get { return false; }
        }

        public string EffectTechniqueName { get { return RenderPipelineTechniqueName.Forward; } }

        public List<VertexPositionNormalColor> Lines { get; }

        public List<VertexPositionNormalColor> Triangles { get; }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            _oldState = renderContext.GraphicsDevice.RasterizerState;

            if (_debugState == null)
            {
                _debugState = new RasterizerState()
                {
                    CullMode = CullMode.None,
                    FillMode = FillMode.WireFrame,
                    DepthBias = _oldState.DepthBias,
                    DepthClipEnable = true,
                    MultiSampleAntiAlias = false,
                    Name = "PhysicsDebugRasterizerState",
                    ScissorTestEnable = false,
                    SlopeScaleDepthBias = _oldState.SlopeScaleDepthBias
                };
            }

            renderContext.GraphicsDevice.RasterizerState = _debugState;
            renderContext.PushEffect(_basicEffect.Effect);

            Lines.Clear();
            Triangles.Clear();
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
            var world = renderContext.World;
            renderContext.World = Matrix.Identity;

            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                if (Lines.Count > 0)
                {
                    // Batch render all lines.
                    var lines = Lines.ToArray();
                    for (var i = 0; i < lines.Length; i += 1000)
                    {
                        renderContext.GraphicsDevice.DrawUserPrimitives(
                            PrimitiveType.LineList,
                            lines,
                            i,
                            lines.Length - i > 1000 ? 500 : (lines.Length - i) / 2);
                    }
                }

                if (Triangles.Count > 0)
                {
                    // Batch render all triangles.
                    var triangles = Triangles.ToArray();
                    for (var i = 0; i < triangles.Length; i += 1500)
                    {
                        renderContext.GraphicsDevice.DrawUserPrimitives(
                            PrimitiveType.TriangleList,
                            triangles,
                            i,
                            triangles.Length - i > 1500 ? 500 : (triangles.Length - i) / 3);
                    }
                }
            }

            renderContext.World = world;

            renderContext.PopEffect();
            renderContext.GraphicsDevice.RasterizerState = _oldState;
        }
    }
}