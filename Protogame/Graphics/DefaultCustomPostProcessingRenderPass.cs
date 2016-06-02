using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    /// <summary>
    /// A default implementation of <see cref="ICustomPostProcessingRenderPass"/>.
    /// </summary>
    /// <module>Graphics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ICustomPostProcessingRenderPass</interface_ref>
    public class DefaultCustomPostProcessingRenderPass : ICustomPostProcessingRenderPass
    {
        private readonly Effect _effect;

        private readonly IGraphicsBlit _graphicsBlit;

        public DefaultCustomPostProcessingRenderPass(IGraphicsBlit graphicsBlit, IAssetManagerProvider assetManagerProvider, string effectAssetName)
        {
            _effect = assetManagerProvider.GetAssetManager().Get<EffectAsset>(effectAssetName).Effect;
            _graphicsBlit = graphicsBlit;
        }

        public DefaultCustomPostProcessingRenderPass(IGraphicsBlit graphicsBlit, EffectAsset effectAsset)
        {
            _effect = effectAsset.Effect;
            _graphicsBlit = graphicsBlit;
        }

        public DefaultCustomPostProcessingRenderPass(IGraphicsBlit graphicsBlit, Effect effect)
        {
            _effect = effect;
            _graphicsBlit = graphicsBlit;
        }

        /// <summary>
        /// Gets a value indicating that this is a post-processing render pass.
        /// </summary>
        /// <value>Always true.</value>
        public bool IsPostProcessingPass
        {
            get { return true; }
        }

        public string EffectTechniqueName { get { return RenderPipelineTechniqueName.PostProcess; } }

        /// <summary>
        /// Gets or sets the number of blur iterations to apply.
        /// </summary>
        public int Iterations { get; set; }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            _graphicsBlit.Blit(renderContext, postProcessingSource, null, _effect);
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }

        public void SetValue(string name, bool value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, int value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Matrix value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Matrix[] value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Quaternion value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, float value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, float[] value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Texture value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector2 value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector2[] value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector3 value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector3[] value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector4 value)
        {
            _effect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector4[] value)
        {
            _effect.Parameters[name].SetValue(value);
        }
    }
}
