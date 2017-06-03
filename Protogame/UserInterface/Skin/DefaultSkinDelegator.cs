using System;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class DefaultSkinDelegator : ISkinDelegator
    {
        private readonly IKernel _kernel;

        public DefaultSkinDelegator(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Render<TContainer>(IRenderContext renderContext, Rectangle layout, TContainer container) where TContainer : IContainer
        {
            var implementation = _kernel.Get<ISkinRenderer<TContainer>>();
            implementation.Render(renderContext, layout, container);
        }

        public Vector2 MeasureText<TContainer>(IRenderContext context, string text, TContainer container) where TContainer : IContainer
        {
            var implementation = _kernel.Get<ISkinRenderer<TContainer>>();
            return implementation.MeasureText(context, text, container);
        }

        public void Render<TContainer>(IRenderContext renderContext, Rectangle layout, Rectangle renderedLayout, TContainer container) where TContainer : IContainer
        {
            var implementation = _kernel.Get<ISkinRenderer<TContainer>>();
            var scrollableImplementation = implementation as IScrollableAwareSkinRenderer<TContainer>;
            if (scrollableImplementation != null)
            {
                scrollableImplementation.Render(renderContext, layout, renderedLayout, container);
            }
            else
            {
                implementation.Render(renderContext, layout, container);
            }
        }
    }
}