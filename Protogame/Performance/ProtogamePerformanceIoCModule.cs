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
            // Presence of the interception library interferes with the Mono Debugger because
            // it can't seem to handle the intercepted call stack.  Therefore, under Mono, we
            // disable the profiler if the debugger is attached.
            if (!System.Diagnostics.Debugger.IsAttached || System.Type.GetType("Mono.Runtime") == null)
            {
                this.Bind<IProfiler>().To<DefaultProfiler>().InSingletonScope();
                var x = new DefaultProfilingInterceptor(this.Kernel.Get<IProfiler>() as DefaultProfiler);
                this.Kernel.Intercept(p => p.Request.Service.IsInterface).With(x);
            }
            else
                this.Bind<IProfiler>().To<NullProfiler>().InSingletonScope();
#else
            this.Bind<IProfiler>().To<NullProfiler>().InSingletonScope();
#endif
        }
    }
}
