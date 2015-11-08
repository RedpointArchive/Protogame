#if FALSE

using Protoinject.Extensions.Interception;

namespace Protogame
{
    /// <summary>
    /// The default profiling interceptor.
    /// </summary>
    internal class DefaultProfilingInterceptor : IInterceptor
    {
        /// <summary>
        /// The m_ profiler.
        /// </summary>
        private readonly DefaultProfiler m_Profiler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultProfilingInterceptor"/> class.
        /// </summary>
        /// <param name="profiler">
        /// The profiler.
        /// </param>
        public DefaultProfilingInterceptor(DefaultProfiler profiler)
        {
            this.m_Profiler = profiler;
        }

        /// <summary>
        /// The intercept.
        /// </summary>
        /// <param name="invocation">
        /// The invocation.
        /// </param>
        public void Intercept(IInvocation invocation)
        {
            var typeName = invocation.Request.Target != null
                               ? invocation.Request.Target.GetType().FullName
                               : invocation.Request.Method.DeclaringType.FullName;
            using (this.m_Profiler.Measure(typeName + "." + invocation.Request.Method.Name))
            {
                invocation.Proceed();
            }
        }
    }
}

#endif