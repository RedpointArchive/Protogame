namespace Protogame
{
    public class NullLogShipping : ILogShipping
    {
        public void AddLog(PendingLogForShip log)
        {
        }

        public PendingLogForShip[] GetAndFlushLogs()
        {
            return new PendingLogForShip[0];
        }
    }
}