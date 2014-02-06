namespace Protogame
{
    using System;
    using System.Threading;
    using Ninject;

    public class DefaultTickRegulator : ITickRegulator
    {
        private readonly int m_TicksPerSecond;

        private DateTime? m_StartTime;

        [Inject]
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
            }
            else
            {
                Console.WriteLine(
                    "WARNING: Tick took " + (int)(DateTime.Now - this.m_StartTime.Value).TotalMilliseconds
                    + "ms, which is longer than " + (1000f / this.m_TicksPerSecond) + "ms.");
            }

            this.m_StartTime = DateTime.Now;
        }
    }
}

