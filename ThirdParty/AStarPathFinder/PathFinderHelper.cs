using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeenGames.Utils.AStarPathFinder
{
    /// <summary>
    /// Not actually used by the real PathFinder code, but created by
    /// Deen Games so we can easily reference path-finder tile values.
    /// </summary>
    public static class PathFinderHelper
    {
        public const int BLOCKED_TILE = 0;
        public const int EMPTY_TILE = 1;

        public static int RoundToNearestPowerOfTwo(int n)
        {
            if (n <= 0)
            {
                throw new ArgumentOutOfRangeException("Calling this function with a non-positive n doesn't make sense (you sent " + n + ")");
            } else if (n == 1 || n == 2) {
                return n;
            }
            else
            {
                return (int)Math.Pow(2, Math.Ceiling(Math.Log(n) / Math.Log(2)));
            }
        }
    }
}
