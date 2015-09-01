namespace Protogame
{
    public interface IUpdatableComponent
    {
        void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext);
    }
}