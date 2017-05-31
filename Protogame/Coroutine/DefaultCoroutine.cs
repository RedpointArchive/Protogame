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

        private async Task WrapCoroutine(Func<Task> coroutine)
        {
            await Task.Yield();
            await coroutine();
        }

        private async Task<T> WrapCoroutine<T>(Func<Task<T>> coroutine)
        {
            await Task.Yield();
            return await coroutine();
        }

        public Task Run(Func<Task> coroutine)
        {
            var oldContext = SynchronizationContext.Current;
            try
            {
                var syncContext = (SynchronizationContext) _coroutineScheduler;
                SynchronizationContext.SetSynchronizationContext(syncContext);
                // The await inside WrapCoroutine will cause it to be placed on the coroutine scheduler.
                var task = WrapCoroutine(coroutine);
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
                // The await inside WrapCoroutine will cause it to be placed on the coroutine scheduler.
                var task = WrapCoroutine(coroutine);
                return task;
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }

        public async Task WaitForNextUpdate(IGameContext gameContext)
        {
            var f = gameContext.FrameCount;
            while (f == gameContext.FrameCount)
            {
                await Task.Yield();
            }
        }
    }
}
