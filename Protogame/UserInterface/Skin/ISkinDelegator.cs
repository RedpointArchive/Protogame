using Microsoft.Xna.Framework;

namespace Protogame
{
    public interface ISkinDelegator
    {
        void Render<TContainer>(
            IRenderContext renderContext,
            Rectangle layout,
            TContainer container) where TContainer : IContainer;

        void Render<TContainer>(
            IRenderContext renderContext,
            Rectangle layout,
            Rectangle renderedLayout,
            TContainer container) where TContainer : IContainer;

        Vector2 MeasureText<TContainer>(IRenderContext context, string text, TContainer container)
            where TContainer : IContainer;
    }
}