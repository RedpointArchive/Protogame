using Ninject.Modules;

namespace Protogame
{
    public class CollisionIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ICollision>().To<DefaultCollision>();
        }
    }
}
