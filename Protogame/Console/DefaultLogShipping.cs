using System.Collections.Generic;

namespace Protogame
{
    public class DefaultLogShipping : ILogShipping
    {
        private List<PendingLogForShip> _pendingLogs;
        private object _pendingLogsLock;

        public DefaultLogShipping()
        {
            _pendingLogs = new List<PendingLogForShip>();
            _pendingLogsLock = new object();
        }

        public void AddLog(PendingLogForShip log)
        {
            lock (_pendingLogsLock)
            {
                _pendingLogs.Add(log);
            }
        }

        public PendingLogForShip[] GetAndFlushLogs()
        {
            lock (_pendingLogsLock)
            {
                var logs = _pendingLogs.ToArray();
                _pendingLogs.Clear();
                return logs;
            }
        }
    }
}