using Ninject.Modules;

namespace Protogame
{
    public class ProtogameCollisionIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ICollision>().To<DefaultCollision>();
        }
    }
}
