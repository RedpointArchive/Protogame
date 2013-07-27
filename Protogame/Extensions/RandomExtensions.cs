using System;

namespace Protogame
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns a new normally distributed number.
        /// </summary>
        /// <remarks>
        /// From http://stackoverflow.com/questions/218060/random-gaussian-variables.
        /// </remarks>
        /// <param name="rand">The random number generator.</param>
        /// <param name="min">The negative sigma 3 lower bound.</param>
        /// <param name="max">The positive sigma 3 upper bound.</param>
        /// <returns>A random normally distributed number.</returns>
        public static double NextGuassian(this Random rand, double min, double max)
        {
            double u1 = rand.NextDouble();
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2);
            return (min + (max - min) / 2) + ((max - min) * 1.5f * randStdNormal);
        }

        public static double NextGuassianClamped(this Random rand, double min, double max)
        {
            return Math.Max(min, Math.Min(max, rand.NextGuassian(min, max)));
        }
    }
}

