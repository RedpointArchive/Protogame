namespace Protogame
{
    using System;
    using System.Threading;

    /// <summary>
    /// The default implementation for tick regulation on a game server.
    /// </summary>
    public class DefaultTickRegulator : ITickRegulator
    {
        private readonly int _ticksPerSecond;

        private DateTime? _processedTime;

        private DateTime? _lastActualTime;

        public DefaultTickRegulator()
        {
            // The default tick amount aligns with MonoGame's 60 FPS target.
            // If you decrease or increase the tick rate of the server, you will need
            // to compensate when applying velocities to objects, since the server
            // will be applying velocities at a slower rate than clients (and thus
            // you will need to multiply velocities so the server moves further in
            // each tick).
            _ticksPerSecond = 60;
        }

        protected DefaultTickRegulator(int ticksPerSecond)
        {
            _ticksPerSecond = ticksPerSecond;
        }

        public bool EmitSlowTicks { get; set; }

        public void WaitUntilReady()
        {
            var now = DateTime.Now;

            if (_processedTime == null)
            {
                // First tick, mark the start time for the next update
                // and return immediately.
                _processedTime = now;
                return;
            }

            var targetSpan = 1000f/_ticksPerSecond;
            var amount = targetSpan - (now - _processedTime.Value).TotalMilliseconds;
            if (amount > 0)
            {
                Thread.Sleep((int)amount);
                _processedTime = now;
            }
            else
            {
                if (EmitSlowTicks)
                {
                    if (_lastActualTime != null)
                    {
                        var timeSpan = (int) (now - _lastActualTime.Value).TotalMilliseconds;
                        if (timeSpan > targetSpan)
                        {
                            Console.WriteLine(
                                "WARNING: Tick took " + (int) (now - _lastActualTime.Value).TotalMilliseconds
                                + "ms, which is longer than " + (1000f/_ticksPerSecond) + "ms (currently behind by " +
                                -amount + "ms).");
                        }
                    }
                    else
                    {
                        Console.WriteLine(
                            "WARNING: Currently behind in ticks, target per frame is " + (1000f/_ticksPerSecond) +
                            "ms, behind by " + -amount + "ms.");
                    }
                }

                // Adjust the next start time so that we wait less.  This allows us to "catch up" on ticks, ensuring
                // that the same number of ticks occur for a given time period, regardless of how long an update
                // takes (assuming that some of those updates are short and allow it to catch up).
                _processedTime = now.AddMilliseconds(amount);
            }

            _lastActualTime = now;
        }
    }
}

