using Ninject.Modules;

namespace Protogame
{
    public class ProtogamePlatformingIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IPlatforming>().To<DefaultPlatforming>();
        }
    }
}
