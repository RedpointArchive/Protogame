using Ninject;
using Ninject.Modules;
using Ninject.Extensions.Interception.Infrastructure.Language;
using System;

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
#else
            this.Bind<IProfiler>().To<NullProfiler>().InSingletonScope();
#endif
        }
    }
}
