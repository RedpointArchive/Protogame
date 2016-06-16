using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class Render3DModelComponent : IRenderableComponent
    {
        private readonly INode _node;

        private readonly I3DRenderUtilities _renderUtilities;

        private readonly ITextureFromHintPath _textureFromHintPath;

        private readonly IAssetManager _assetManager;

        private ModelAsset _lastCachedModel;

        private TextureAsset _lastCachedTexture;

        private bool _useDefaultEffects;

        private EffectAsset _defaultSkinnedEffectAsset;

        private EffectAsset _defaultSkinnedColorEffectAsset;

        public Render3DModelComponent(
            INode node,
            I3DRenderUtilities renderUtilities,
            IAssetManagerProvider assetManagerProvider,
            ITextureFromHintPath textureFromHintPath)
        {
            _node = node;
            _renderUtilities = renderUtilities;
            _textureFromHintPath = textureFromHintPath;
            _assetManager = assetManagerProvider.GetAssetManager();
        }
        
        public ModelAsset Model { get; set; }

        public EffectAsset Effect { get; set; }

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.IsCurrentRenderPass<I3DRenderPass>())
            {
                if (Effect == null)
                {
                    _useDefaultEffects = true;
                }
                else
                {
                    _useDefaultEffects = false;
                }

                if (_useDefaultEffects && _defaultSkinnedEffectAsset == null)
                {
                    _defaultSkinnedEffectAsset = _assetManager.Get<EffectAsset>("effect.TextureSkinned");
                    _defaultSkinnedColorEffectAsset = _assetManager.Get<EffectAsset>("effect.ColorSkinned");
                }

                if (Model != null)
                {
                    var matrix = Matrix.Identity;
                    var matrixComponent = _node.Parent?.UntypedValue as IHasTransform;
                    if (matrixComponent != null)
                    {
                        matrix *= matrixComponent.FinalTransform.AbsoluteMatrix;
                    }
                    
                    if (_lastCachedModel != Model)
                    {
                        if (Model.Material.TextureDiffuse != null)
                        {
                            _lastCachedTexture =
                                _textureFromHintPath.GetTextureFromHintPath(Model.Material.TextureDiffuse);
                        }
                        _lastCachedModel = Model;
                    }

                    if (_lastCachedTexture != null)
                    {
                        if (_useDefaultEffects)
                        {
                            renderContext.PushEffect(_defaultSkinnedEffectAsset.Effect);
                        }
                        else
                        {
                            renderContext.PushEffect(Effect.Effect);
                        }

                        renderContext.EnableTextures();
                        renderContext.SetActiveTexture(_lastCachedTexture.Texture);    
                    }
                    else
                    {
                        if (_useDefaultEffects)
                        {
                            renderContext.PushEffect(_defaultSkinnedColorEffectAsset.Effect);
                        }
                        else
                        {
                            renderContext.PushEffect(Effect.Effect);
                        }

                        renderContext.EnableVertexColors();
                    }

                    Model.Render(
                        renderContext,
                        matrix);
                    renderContext.PopEffect();
                }
                else
                {
                    _lastCachedModel = null;
                    _lastCachedTexture = null;
                }
            }
        }
    }
}
