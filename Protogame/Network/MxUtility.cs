namespace Protogame
{
    public static class MxUtility
    {
        public const int UIntBitsize = 32;

        public static long GetSequenceNumberDifference(uint @new, uint current)
        {
            var max = uint.MaxValue;

            if (@new > current)
            {
                var isMoreRecent = @new - current <= max / 2;

                return isMoreRecent ? @new - current : current - @new;
            }

            if (current > @new)
            {
                var isMoreRecent = @current - @new > max / 2;

                var newAdded = @new + (long)max + 1;
                return isMoreRecent ? (int)(newAdded - current) : (int)(current - newAdded);
            }

            return 0;
        }
    }
}