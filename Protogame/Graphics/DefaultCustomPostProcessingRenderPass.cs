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
        private readonly IAssetReference<EffectAsset> _effect;
        private readonly IEffect _effectDirect;

        private readonly IGraphicsBlit _graphicsBlit;

        public DefaultCustomPostProcessingRenderPass(IGraphicsBlit graphicsBlit, IAssetManager assetManager, string effectAssetName)
        {
            _effect = assetManager.Get<EffectAsset>(effectAssetName);
            _graphicsBlit = graphicsBlit;
        }

        public DefaultCustomPostProcessingRenderPass(IGraphicsBlit graphicsBlit, IAssetReference<EffectAsset> effectAsset)
        {
            _effect = effectAsset;
            _graphicsBlit = graphicsBlit;
        }

        public DefaultCustomPostProcessingRenderPass(IGraphicsBlit graphicsBlit, IEffect effect)
        {
            _effectDirect = effect;
            _graphicsBlit = graphicsBlit;
        }

        public bool IsPostProcessingPass => true;
        public bool SkipWorldRenderBelow => true;
        public bool SkipWorldRenderAbove => true;
        public bool SkipEntityRender => true;
        public bool SkipEngineHookRender => true;
        public string EffectTechniqueName => RenderPipelineTechniqueName.PostProcess;

        /// <summary>
        /// Gets or sets the number of blur iterations to apply.
        /// </summary>
        public int Iterations { get; set; }

        public void BeginRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass previousPass, RenderTarget2D postProcessingSource)
        {
            var effect = GetEffect();
            if (effect != null)
            {
                _graphicsBlit.Blit(renderContext, postProcessingSource, null, effect);
            }
        }

        public void EndRenderPass(IGameContext gameContext, IRenderContext renderContext, IRenderPass nextPass)
        {
        }

        public string Name { get; set; }

        private IEffect GetEffect()
        {
            if (_effectDirect != null)
            {
                return _effectDirect;
            }

            if (_effect.IsReady)
            {
                return _effect.Asset.Effect;
            }

            return null;
        }

        public void SetValue(string name, bool value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, int value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Matrix value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Matrix[] value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Quaternion value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, float value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, float[] value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Texture value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector2 value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector2[] value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector3 value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector3[] value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector4 value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }

        public void SetValue(string name, Vector4[] value)
        {
            GetEffect()?.NativeEffect.Parameters[name].SetValue(value);
        }
    }
}
