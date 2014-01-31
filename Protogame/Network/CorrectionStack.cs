namespace Protogame
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a correction stack by which to keep track of the amount of
    /// error correction currently provided to a local value.
    /// </summary>
    public class CorrectionStack
    {
        /// <summary>
        /// The currently applied corrections.
        /// </summary>
        private List<Correction> m_Corrections = new List<Correction>();

        /// <summary>
        /// Indicates that the specified amount of correction has been applied
        /// at the specified detection tick and expires at the specified expiration tick.
        /// </summary>
        /// <param name="detectionTick">The tick at which the error was detected and correction applied.</param>
        /// <param name="expirationTick">The tick at which the error correction expires (e.g. the real value should have caught up).</param>
        /// <param name="correction">The amount of correction applied.</param>
        public void ApplyCorrection(int detectionTick, int expirationTick, float correction)
        {
            this.m_Corrections.Add(new Correction
                {
                    DetectionTick = detectionTick,
                    ExpirationTick = expirationTick,
                    Amount = correction
                });
        }

        /// <summary>
        /// Returns the current amount of error correction that is being applied
        /// to the value.
        /// </summary>
        /// <returns>The amount of error correction.</returns>
        /// <param name="tick">The current tick.</param>
        public float GetCorrection(int tick)
        {
            return this.m_Corrections
                    .Where(x => x.DetectionTick <= tick)
                    .Where(x => x.ExpirationTick > tick)
                    .Select(x => x.Amount)
                    .Sum();
        }

        public void Purge(int tick)
        {
            this.m_Corrections.RemoveAll(x => x.ExpirationTick < tick);
        }

        private struct Correction
        {
            public int DetectionTick { get; set; }

            public int ExpirationTick { get; set; }

            public float Amount { get; set; }
        }
    }
}

