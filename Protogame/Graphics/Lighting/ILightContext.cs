using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public interface ILightContext
    {
        RenderTarget2D DiffuseLightRenderTarget { get; }

        RenderTarget2D SpecularLightRenderTarget { get; }

        Texture2D DeferredColorMap { get; }

        Texture2D DeferredNormalMap { get; }

        Texture2D DeferredDepthMap { get; }

        Texture2D DeferredSpecularMap { get; }

        Vector2 HalfPixel { get; }

        BlendState LightBlendState { get; }

        RasterizerState RasterizerStateCullNone { get; }

        RasterizerState RasterizerStateCullClockwiseFace { get; }

        RasterizerState RasterizerStateCullCounterClockwiseFace { get; }
    }
}
