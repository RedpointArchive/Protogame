using Ninject;
using Ninject.Modules;
using Ninject.Extensions.Interception.Infrastructure.Language;

namespace Protogame
{
    public class ProtogamePerformanceIoCModule : NinjectModule
    {
        public override void Load()
        {
#if DEBUG
            this.Bind<IProfiler>().To<DefaultProfiler>().InSingletonScope();
            var x = new DefaultProfilingInterceptor(this.Kernel.Get<IProfiler>() as DefaultProfiler);
            this.Kernel.Intercept(p => p.Request.Service.IsInterface).With(x);
#elif RELEASE
            this.Bind<IProfiler>().To<NullProfiler>().InSingletonScope();
#else
            throw new System.InvalidOperationException("Not in Debug or Release mode.");
#endif
        }
    }
}
