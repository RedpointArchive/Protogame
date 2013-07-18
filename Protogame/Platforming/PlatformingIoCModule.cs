using Ninject.Modules;

namespace Protogame
{
    public class PlatformingIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IPlatforming>().To<DefaultPlatforming>();
        }
    }
}
