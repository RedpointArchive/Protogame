namespace Protogame
{
    using System;
    using System.Threading;
    using Protoinject;

    /// <summary>
    /// The default implementation for tick regulation on a game server.
    /// </summary>
    public class DefaultTickRegulator : ITickRegulator
    {
        private readonly int m_TicksPerSecond;

        private DateTime? m_StartTime;
        
        public DefaultTickRegulator()
        {
            // The default tick amount aligns with MonoGame's 60 FPS target.
            // If you decrease or increase the tick rate of the server, you will need
            // to compensate when applying velocities to objects, since the server
            // will be applying velocities at a slower rate than clients (and thus
            // you will need to multiply velocities so the server moves further in
            // each tick).
            this.m_TicksPerSecond = 60;
        }

        protected DefaultTickRegulator(int ticksPerSecond)
        {
            this.m_TicksPerSecond = ticksPerSecond;
        }

        public void WaitUntilReady()
        {
            if (this.m_StartTime == null)
            {
                // First tick, mark the start time for the next update
                // and return immediately.
                this.m_StartTime = DateTime.Now;
                return;
            }

            var amount = (1000f / this.m_TicksPerSecond) - (DateTime.Now - this.m_StartTime.Value).TotalMilliseconds;
            if (amount > 0)
            {
                Thread.Sleep((int)amount);
                this.m_StartTime = DateTime.Now;
            }
            else
            {
                Console.WriteLine(
                    "WARNING: Tick took " + (int)(DateTime.Now - this.m_StartTime.Value).TotalMilliseconds
                    + "ms, which is longer than " + (1000f / this.m_TicksPerSecond) + "ms.");

                // Adjust the next start time so that we wait less.  This allows us to "catch up" on ticks, ensuring
                // that the same number of ticks occur for a given time period, regardless of how long an update
                // takes (assuming that some of those updates are short and allow it to catch up).
                this.m_StartTime = DateTime.Now.AddMilliseconds(amount);
            }
        }
    }
}

