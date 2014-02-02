namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The utility extensions.
    /// </summary>
    public static class UtilityExtensions
    {
        /// <summary>
        /// The shuffle.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <remarks>
        /// Http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp.
        /// </remarks>
        public static void Shuffle<T>(this IList<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do provider.GetBytes(box);
                while (box[0] >= n * (byte.MaxValue / n));
                int k = box[0] % n;
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Converts the color to it's premultiplied format.
        /// </summary>
        /// <param name="color">
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public static Color ToPremultiplied(this Color color)
        {
            return new Color(
                color.R / 255f * color.A / 255f, 
                color.G / 255f * color.A / 255f, 
                color.B / 255f * color.A / 255f, 
                color.A / 255f);
        }
    }
}