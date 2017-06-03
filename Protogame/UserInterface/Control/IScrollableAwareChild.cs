using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface IScrollableAwareChild
    {
        void Render(IRenderContext context, ISkinLayout skinLayout, ISkinDelegator skinDelegator, Rectangle layout, Rectangle renderedLayout);
    }
}
