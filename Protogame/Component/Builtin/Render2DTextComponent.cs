using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Render2DTextComponent : IRenderableComponent, IEnabledComponent
    {
        private readonly I2DRenderUtilities _renderUtilities;

        public IAssetReference<FontAsset> Font { get; set; }

        public string Text { get; set; }

        public Render2DTextComponent(I2DRenderUtilities renderUtilities)
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

            if (!renderContext.IsCurrentRenderPass<I3DRenderPass>() && Font != null)
            {
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(entity.Transform.LocalPosition.X, entity.Transform.LocalPosition.Y),
                    Text,
                    Font);
            }
        }
    }
}
