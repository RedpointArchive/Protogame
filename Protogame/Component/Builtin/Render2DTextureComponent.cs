using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Render2DTextureComponent : IRenderableComponent
    {
        private readonly I2DRenderUtilities _renderUtilities;

        public TextureAsset Texture { get; set; }

        public Render2DTextureComponent(I2DRenderUtilities renderUtilities)
        {
            _renderUtilities = renderUtilities;
        }

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext && Texture != null)
            {
                _renderUtilities.RenderTexture(
                    renderContext,
                    new Vector2(entity.LocalMatrix.Translation.X, entity.LocalMatrix.Translation.Y),
                    Texture);
            }
        }
    }
}
