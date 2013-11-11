using Ninject.Modules;

namespace Protogame
{
    public class ProtogameEventsIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IEventEngine<IGameContext>>().To<DefaultEventEngine<IGameContext>>().InSingletonScope();
            this.Bind<IEngineHook>().To<EventEngineHook>();
            this.Bind<IEventBinder<IGameContext>>().To<ConsoleEventBinder>();
        }
    }
}
