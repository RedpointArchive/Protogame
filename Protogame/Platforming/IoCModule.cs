using Ninject.Modules;

namespace Protogame.Platforming
{
    public class IoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IPlatforming>().To<DefaultPlatforming>();
        }
    }
}
