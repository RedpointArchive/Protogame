using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Render2DTextureComponent : IRenderableComponent, IEnabledComponent
    {
        private readonly I2DRenderUtilities _renderUtilities;

        public TextureAsset Texture { get; set; }

        public Render2DTextureComponent(I2DRenderUtilities renderUtilities)
        {
            _renderUtilities = renderUtilities;

            Enabled = true;
        }

        public bool Enabled { get; set; }

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (!Enabled)
            {
                return;
            }

            if (!renderContext.IsCurrentRenderPass<I3DRenderPass>() && Texture != null)
            {
                _renderUtilities.RenderTexture(
                    renderContext,
                    new Vector2(entity.Transform.LocalPosition.X, entity.Transform.LocalPosition.Y),
                    Texture);
            }
        }
    }
}
