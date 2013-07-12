using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public static class Extensions
    {
        /// <remarks>
        /// http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
        /// </remarks>
        internal static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do
                    provider.GetBytes(box); while (box[0] >= n * (Byte.MaxValue / n));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Converts the color to it's premultiplied format.
        /// </summary>
        /// <param name="color"></param>
        public static Color ToPremultiplied(this Color color)
        {
            return new Color(color.R / 255f * color.A / 255f, color.G / 255f * color.A / 255f, color.B / 255f * color.A / 255f, color.A / 255f);
        }
    }
}
