using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface ILightContext
    {
        RenderTarget2D LightRenderTarget { get; }

        Texture2D DeferredColorMap { get; }

        Texture2D DeferredNormalMap { get; }

        Texture2D DeferredDepthMap { get; }

        Vector2 HalfPixel { get; }

        BlendState LightBlendState { get; }
    }
}
