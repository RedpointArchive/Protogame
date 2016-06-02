namespace Protogame
{
    public interface ILight
    {
        void Render(IGameContext gameContext, IRenderContext renderContext, ILightContext lightContext);
    }
}
