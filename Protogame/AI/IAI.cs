namespace Protogame
{
    using Microsoft.Xna.Framework;

    public interface IAI
    {
        Vector2 Update(IGameContext gameContext, IUpdateContext updateContext, Agent agent);

        void RenderDebug(IGameContext gameContext, IRenderContext renderContext, Agent agent, I2DRenderUtilities twoDRenderUtilities);
    }
}