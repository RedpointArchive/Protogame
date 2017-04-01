using System;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class Render2DTextureComponent : IRenderableComponent, IEnabledComponent
    {
        private readonly I2DRenderUtilities _renderUtilities;

        public IAssetReference<TextureAsset> Texture { get; set; }

        public Vector2? RotationAnchor { get; set; }

        public Color? Color { get; set; }

        public bool FlipHorizontally { get; set; }

        public bool FlipVertically { get; set; }

        public Rectangle? SourceArea { get; set; }

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
                Vector2? size = null;
                if (Texture.IsReady)
                {
                    size = new Vector2(
                        Texture.Asset.Texture.Width * entity.Transform.LocalScale.X,
                        Texture.Asset.Texture.Width * entity.Transform.LocalScale.Y);
                }
                
                var defaultVec = Vector3.Right;
                var transformedVec = Vector3.Transform(defaultVec, entity.Transform.LocalRotation);
                var twodVec = new Vector2(transformedVec.X, transformedVec.Y);
                var rotation = (float)Math.Atan2(twodVec.Y, twodVec.X);

                _renderUtilities.RenderTexture(
                    renderContext,
                    new Vector2(entity.Transform.LocalPosition.X, entity.Transform.LocalPosition.Y),
                    Texture,
                    size,
                    Color,
                    rotation,
                    RotationAnchor,
                    FlipHorizontally,
                    FlipVertically,
                    SourceArea);
            }
        }
    }
}
