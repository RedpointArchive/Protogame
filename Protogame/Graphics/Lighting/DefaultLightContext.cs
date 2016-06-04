using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class DefaultLightContext : ILightContext
    {
        public DefaultLightContext(
            Texture2D deferredColorMap,
            Texture2D deferredNormalMap,
            Texture2D deferredDepthMap,
            RenderTarget2D lightRenderTarget,
            BlendState lightBlendState,
            RasterizerState rasterizerStateCullNone,
            RasterizerState rasterizerStateCullClockwiseFace,
            RasterizerState rasterizerStateCullCounterClockwiseFace,
            Vector2 halfPixel)
        {
            DeferredColorMap = deferredColorMap;
            DeferredNormalMap = deferredNormalMap;
            DeferredDepthMap = deferredDepthMap;
            LightRenderTarget = lightRenderTarget;
            LightBlendState = lightBlendState;
            RasterizerStateCullNone = rasterizerStateCullNone;
            RasterizerStateCullClockwiseFace = rasterizerStateCullClockwiseFace;
            RasterizerStateCullCounterClockwiseFace = rasterizerStateCullCounterClockwiseFace;
            HalfPixel = halfPixel;
        }

        public Texture2D DeferredColorMap { get; }
        public Texture2D DeferredNormalMap { get; }
        public Texture2D DeferredDepthMap { get; }
        public RenderTarget2D LightRenderTarget { get; }
        public BlendState LightBlendState { get; }
        public RasterizerState RasterizerStateCullNone { get; }
        public RasterizerState RasterizerStateCullClockwiseFace { get; }
        public RasterizerState RasterizerStateCullCounterClockwiseFace { get; }
        public Vector2 HalfPixel { get; }
    }
}