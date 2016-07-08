using System;
using System.Threading;
using System.Threading.Tasks;

namespace Protogame
{
    public class DefaultCoroutine : ICoroutine
    {
        private readonly ICoroutineScheduler _coroutineScheduler;

        public DefaultCoroutine(ICoroutineScheduler coroutineScheduler)
        {
            _coroutineScheduler = coroutineScheduler;
        }

        public Task Run(Func<Task> coroutine)
        {
            var oldContext = SynchronizationContext.Current;
            try
            {
                var syncContext = (SynchronizationContext) _coroutineScheduler;
                SynchronizationContext.SetSynchronizationContext(syncContext);
                var task = coroutine();
                syncContext.Post(async _=> { await task; }, null);
                return task;
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }

        public Task<T> Run<T>(Func<Task<T>> coroutine)
        {
            var oldContext = SynchronizationContext.Current;
            try
            {
                var syncContext = (SynchronizationContext)_coroutineScheduler;
                SynchronizationContext.SetSynchronizationContext(syncContext);
                var task = coroutine();
                syncContext.Post(async _ => { await task; }, null);
                return task;
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }
    }
}
