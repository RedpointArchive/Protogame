using Ninject.Extensions.Interception;

namespace Protogame
{
    internal class DefaultProfilingInterceptor : IInterceptor
    {
        private DefaultProfiler m_Profiler;
    
        public DefaultProfilingInterceptor(DefaultProfiler profiler)
        {
            this.m_Profiler = profiler;
        }

        public void Intercept(IInvocation invocation)
        {
            var typeName = invocation.Request.Target != null ?
                invocation.Request.Target.GetType().FullName :
                invocation.Request.Method.DeclaringType.FullName;
            using (this.m_Profiler.Measure(typeName + "." + invocation.Request.Method.Name))
            {
                invocation.Proceed();
            }
        }
    }
}

