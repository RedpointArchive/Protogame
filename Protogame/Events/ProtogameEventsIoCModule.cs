namespace Protogame
{
    using Ninject.Modules;

    /// <summary>
    /// The protogame events io c module.
    /// </summary>
    public class ProtogameEventsIoCModule : NinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public override void Load()
        {
            this.Bind<IEventEngine<IGameContext>>().To<DefaultEventEngine<IGameContext>>().InSingletonScope();
            this.Bind<IEngineHook>().To<EventEngineHook>();
            this.Bind<IEventBinder<IGameContext>>().To<ConsoleEventBinder>();
            this.Bind<IEventBinder<IGameContext>>().To<UIEventBinder>();
        }
    }
}