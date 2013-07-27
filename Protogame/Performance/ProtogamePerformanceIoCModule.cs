using Ninject;
using Ninject.Modules;

namespace Protogame
{
    public class ProtogamePerformanceIoCModule : NinjectModule
    {
        public override void Load()
        {
#if DEBUG
            this.Bind<IProfiler>().To<DefaultProfiler>().InSingletonScope();
            if (this.Kernel.Get<IRenderUtilities>() is DefaultRenderUtilities)
                this.Rebind<IRenderUtilities>().To<ProfiledRenderUtilities>();
#elif RELEASE
            this.Bind<IProfiler>().To<NullProfiler>().InSingletonScope();
#else
            throw new System.InvalidOperationException("Not in Debug or Release mode.");
#endif
        }
    }
}
