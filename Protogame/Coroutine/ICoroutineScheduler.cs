namespace Protogame
{
    /// <summary>
    /// The internal scheduler for coroutines.  This interface is used by <see cref="CoroutineEngineHook"/> to
    /// progress coroutines as the game executes.
    /// </summary>
    /// <module>Coroutine</module>
    public interface ICoroutineScheduler
    {
        void Update(IGameContext gameContext, IUpdateContext updateContext);

        void Update(IServerContext serverContext, IUpdateContext updateContext);
    }
}