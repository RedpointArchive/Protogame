using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class BasicScrollableContainerSkinRenderer : ISkinRenderer<ScrollableContainer>
    {
        private readonly IBasicSkinHelper _basicSkinHelper;
        private readonly I2DRenderUtilities _renderUtilities;
        private readonly ISkinLayout _skinLayout;

        public BasicScrollableContainerSkinRenderer(IBasicSkinHelper basicSkinHelper, I2DRenderUtilities renderUtilities, ISkinLayout skinLayout)
        {
            _basicSkinHelper = basicSkinHelper;
            _renderUtilities = renderUtilities;
            _skinLayout = skinLayout;
        }

        public void Render(IRenderContext renderContext, Rectangle layout, ScrollableContainer scrollableContainer)
        {
            _basicSkinHelper.DrawSunken(renderContext, layout);

            var layoutWidth = layout.Width - _skinLayout.HorizontalScrollBarHeight;
            var layoutHeight = layout.Height - _skinLayout.VerticalScrollBarWidth;

            var childContent = scrollableContainer.ChildContent;

            _renderUtilities.RenderTexture(
                renderContext,
                new Vector2(layout.X, layout.Y),
                childContent,
                new Vector2(layoutWidth, layoutHeight),
                sourceArea: new Rectangle(
                    (int)(scrollableContainer.ScrollX * (System.Math.Max(childContent.Width, layoutWidth) - layoutWidth)),
                    (int)(scrollableContainer.ScrollY * (System.Math.Max(childContent.Height, layoutHeight) - layoutHeight)),
                    layoutWidth,
                    layoutHeight));

            var raisedPadding = 3;

            _basicSkinHelper.DrawSunken(renderContext, new Rectangle(
                layout.X,
                layout.Y + layout.Height - _skinLayout.HorizontalScrollBarHeight,
                layout.Width - _skinLayout.VerticalScrollBarWidth,
                _skinLayout.HorizontalScrollBarHeight));
            _basicSkinHelper.DrawSunken(renderContext, new Rectangle(
                layout.X + layout.Width - _skinLayout.VerticalScrollBarWidth,
                layout.Y,
                _skinLayout.VerticalScrollBarWidth,
                layout.Height - _skinLayout.HorizontalScrollBarHeight));

            _basicSkinHelper.DrawRaised(renderContext, new Rectangle(
                (int)(layout.X + scrollableContainer.ScrollX * (layoutWidth - layoutWidth / (float)childContent.Width * layoutWidth)) + raisedPadding,
                layout.Y + layout.Height - _skinLayout.HorizontalScrollBarHeight + raisedPadding,
                (int)(layoutWidth / (float)childContent.Width * layoutWidth) - raisedPadding * 2,
                _skinLayout.HorizontalScrollBarHeight - raisedPadding * 2));

            _basicSkinHelper.DrawRaised(renderContext, new Rectangle(
                layout.X + layout.Width - _skinLayout.VerticalScrollBarWidth + raisedPadding,
                (int)(layout.Y + scrollableContainer.ScrollY * (layoutHeight - layoutHeight / (float)childContent.Height * layoutHeight)) + raisedPadding,
                _skinLayout.VerticalScrollBarWidth - raisedPadding * 2,
                (int)(layoutHeight / (float)childContent.Height * layoutHeight) - raisedPadding * 2));
        }

        public Vector2 MeasureText(IRenderContext renderContext, string text, ScrollableContainer container)
        {
            throw new NotSupportedException();
        }
    }
}
