#pragma warning disable CS1591

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// The default implementation of an <see cref="I2DBatchedRenderPass"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.I2DBatchedRenderPass</interface_ref>
    public class Default2DBatchedRenderPass : I2DBatchedRenderPass
    {
        public bool IsPostProcessingPass => false;
        public bool SkipWorldRenderBelow => false;
        public bool SkipWorldRenderAbove => false;
        public bool SkipEntityRender => false;
        public bool SkipEngineHookRender => false;
        public string EffectTechniqueName => RenderPipelineTechniqueName.Batched2D;

        public Viewport? Viewport { get; set; }

        public Matrix? TransformMatrix { get; private set; }

        public SpriteSortMode TextureSortMode
        {
            get;
            set;
        }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            renderContext.SpriteBatch.Begin(TextureSortMode, transformMatrix: TransformMatrix);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
            renderContext.SpriteBatch.End();
        }

        public void RestartWithTransformMatrix(IRenderContext renderContext, Matrix matrix)
        {
            TransformMatrix = matrix;

            renderContext.SpriteBatch.End();
            renderContext.SpriteBatch.Begin(TextureSortMode, transformMatrix: TransformMatrix);
        }

        public string Name { get; set; }
    }
}

