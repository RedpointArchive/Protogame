namespace Protogame
{
    public interface ILogShipping
    {
        void AddLog(PendingLogForShip log);

        PendingLogForShip[] GetAndFlushLogs();
    }
}