namespace Protogame
{
    /// <summary>
    /// The mx utility.
    /// </summary>
    public static class MxUtility
    {
        /// <summary>
        /// The u int bitsize.
        /// </summary>
        public const int UIntBitsize = 32;

        /// <summary>
        /// Gets the sequence number difference between the "new" and "current" messages.
        /// If the "new" sequence ID represents a later message, then the result is positive;
        /// if the "new" sequence ID represents an older message, then the result is negative.
        /// </summary>
        /// <param name="new">
        /// </param>
        /// <param name="current">
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public static long GetSequenceNumberDifference(uint @new, uint current)
        {
            var max = uint.MaxValue;

            if (@new > current)
            {
                var isMoreRecent = @new - current <= max / 2;

                return isMoreRecent ? @new - current : -(current - @new);
            }

            if (current > @new)
            {
                var isMoreRecent = @current - @new > max / 2;

                var newAdded = @new + (long)max + 1;
                return isMoreRecent ? (int)(newAdded - current) : -(int)(current - newAdded);
            }

            return 0;
        }
    }
}