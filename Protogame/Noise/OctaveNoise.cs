namespace Protogame
{
    using System;

    /// <summary>
    /// The octave noise.
    /// </summary>
    public class OctaveNoise
    {
        /// <summary>
        /// The m_ perlin.
        /// </summary>
        private readonly PerlinNoise[] m_Perlin;

        /// <summary>
        /// Initializes a new instance of the <see cref="OctaveNoise"/> class.
        /// </summary>
        /// <param name="seed">
        /// The seed.
        /// </param>
        /// <param name="octaves">
        /// The octaves.
        /// </param>
        public OctaveNoise(int seed, int octaves)
        {
            this.m_Perlin = new PerlinNoise[octaves];
            for (int i = 0; i < octaves; i++)
            {
                this.m_Perlin[i] = new PerlinNoise(new Random(seed + i));
            }
        }

        /// <summary>
        /// The noise.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <param name="z">
        /// The z.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double Noise(double x, double y, double z)
        {
            double scale = 1;
            double value = 0;

            for (int i = 0; i < this.m_Perlin.Length; i++)
            {
                double scaledX = x * scale;
                double scaledY = y * scale;
                double scaledZ = z * scale;

                value += this.m_Perlin[i].Noise(scaledX, scaledY, scaledZ) * scale;
                scale /= 2;
            }

            return value;
        }

        /// <summary>
        /// The noise 2 d.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double Noise2D(double x, double y)
        {
            return this.Noise(x, y, 20);
        }
    }
}