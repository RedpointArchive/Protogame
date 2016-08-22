using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface ISkinRenderer<in TContainer> where TContainer : IContainer
    {
        void Render(
            IRenderContext renderContext,
            Rectangle layout,
            TContainer container);

        Vector2 MeasureText(IRenderContext renderContext, string text, TContainer container);
    }
}
