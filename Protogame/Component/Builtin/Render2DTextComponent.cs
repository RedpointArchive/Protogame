using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Render2DTextComponent : IRenderableComponent
    {
        private readonly I2DRenderUtilities _renderUtilities;

        public FontAsset Font { get; set; }

        public string Text { get; set; }

        public Render2DTextComponent(I2DRenderUtilities renderUtilities)
        {
            _renderUtilities = renderUtilities;
        }

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
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
