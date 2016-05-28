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
            if (Effect == null)
            {
                Effect = _assetManager.Get<EffectAsset>("effect.Skinned");
            }

            if (Model != null)
            {
                var matrix = Matrix.Identity;
                var matrixComponent = _node.Parent?.UntypedValue as IHasMatrix;
                if (matrixComponent != null)
                {
                    matrix *= matrixComponent.GetFinalMatrix();
                }

                if (_lastCachedModel != Model)
                {
                    if (Model.Material.TextureDiffuse != null)
                    {
                        _lastCachedTexture = _textureFromHintPath.GetTextureFromHintPath(Model.Material.TextureDiffuse);
                    }
                    _lastCachedModel = Model;
                }

                if (_lastCachedTexture != null)
                {
                    renderContext.EnableTextures();
                    renderContext.SetActiveTexture(_lastCachedTexture.Texture);
                }
                else
                {
                    renderContext.EnableVertexColors();
                }

                renderContext.PushEffect(Effect.Effect);
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
