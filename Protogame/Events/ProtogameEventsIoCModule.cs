using Ninject.Modules;

namespace Protogame
{
    public class ProtogameEventsIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IEventEngine>().To<DefaultEventEngine>().InSingletonScope();
            this.Bind<IEngineHook>().To<EventEngineHook>();
        }
    }
}
