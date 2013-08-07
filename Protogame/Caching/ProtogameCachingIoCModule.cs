using Ninject.Modules;

namespace Protogame
{
    public class ProtogameCachingIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IRenderCache>().To<DefaultRenderCache>().InSingletonScope();
        }
    }
}
