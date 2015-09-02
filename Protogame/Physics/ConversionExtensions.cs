namespace Protogame
{
    using Jitter.LinearMath;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Provides extension methods for converting between Jitter and XNA / MonoGame
    /// data structures.
    /// </summary>
    /// <remarks>
    /// This is from the Jitter demo code.
    /// </remarks>
    /// <module>Physics</module>
    public static class ConversionExtensions
    {
        /// <summary>
        /// Convert the XNA vector to a Jitter vector.
        /// </summary>
        /// <returns>The Jitter representation of the XNA vector.</returns>
        /// <param name="vector">The XNA vector.</param>
        public static JVector ToJitterVector(this Vector3 vector)
        {
            return new JVector(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Convert the Jitter matrix to an XNA matrix.
        /// </summary>
        /// <returns>The XNA representation of the Jitter matrix.</returns>
        /// <param name="matrix">The Jitter matrix.</param>
        public static Matrix ToXNAMatrix(this JMatrix matrix)
        {
            return new Matrix(matrix.M11,
                matrix.M12,
                matrix.M13,
                0.0f,
                matrix.M21,
                matrix.M22,
                matrix.M23,
                0.0f,
                matrix.M31,
                matrix.M32,
                matrix.M33,
                0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        /// Convert the XNA matrix to a Jitter matrix.
        /// </summary>
        /// <returns>The Jitter representation of the XNA matrix.</returns>
        /// <param name="matrix">The XNA matrix.</param>
        public static JMatrix ToJitterMatrix(this Matrix matrix)
        {
            JMatrix result;
            result.M11 = matrix.M11;
            result.M12 = matrix.M12;
            result.M13 = matrix.M13;
            result.M21 = matrix.M21;
            result.M22 = matrix.M22;
            result.M23 = matrix.M23;
            result.M31 = matrix.M31;
            result.M32 = matrix.M32;
            result.M33 = matrix.M33;
            return result;
        }

        /// <summary>
        /// Converts the Jitter vector to an XNA vector.
        /// </summary>
        /// <returns>The XNA representation of the Jitter vector.</returns>
        /// <param name="vector">The Jitter vector.</param>
        public static Vector3 ToXNAVector(this JVector vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}

