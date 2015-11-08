namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The protogame events io c module.
    /// </summary>
    public class ProtogameEventsIoCModule : IProtoinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<IEventEngine<IGameContext>>().To<DefaultEventEngine<IGameContext>>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<EventEngineHook>();
            kernel.Bind<IEventBinder<IGameContext>>().To<ConsoleEventBinder>();
            kernel.Bind<IEventBinder<IGameContext>>().To<UIEventBinder>();
        }
    }
}