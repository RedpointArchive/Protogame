using System;
using System.Collections.Generic;
using System.Threading;

namespace Protogame
{
    public class DefaultCoroutineScheduler : SynchronizationContext, ICoroutineScheduler
    {
        private List<KeyValuePair<SendOrPostCallback, object>> _continuations;

        public DefaultCoroutineScheduler()
        {
            _continuations = new List<KeyValuePair<SendOrPostCallback, object>>();
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d));
            }

            _continuations.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException();
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            Update();
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            Update();
        }

        private void Update()
        {
            var toProcess = _continuations.ToArray();
            _continuations.Clear();
            foreach (var continuation in toProcess)
            {
                var currentContext = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(this);

                    continuation.Key(continuation.Value);
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(currentContext);
                }
            }
        }
    }
}
