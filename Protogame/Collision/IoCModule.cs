using Ninject.Modules;

namespace Protogame.Collision
{
    public class IoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ICollision>().To<DefaultCollision>();
        }
    }
}
