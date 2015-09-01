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
            if (!renderContext.Is3DContext && Font != null)
            {
                _renderUtilities.RenderText(
                    renderContext,
                    new Vector2(entity.X, entity.Y),
                    Text,
                    Font);
            }
        }
    }
}
