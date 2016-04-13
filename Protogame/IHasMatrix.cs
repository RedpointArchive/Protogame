using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// An interface which indicates that an object provides a local matrix and a method
    /// for calculating the final matrix taking into account any parent objects (if the 
    /// object resides in a hierarchy).
    /// </summary>
    /// <module>Core API</module>
    public interface IHasMatrix
    {
        /// <summary>
        /// The local matrix of this object.
        /// </summary>
        Matrix LocalMatrix { get; set; }

        /// <summary>
        /// Gets the final calculated matrix, taking into account any parent matrixes that
        /// need to be applied.
        /// </summary>
        /// <returns>
        /// The final calculated matrix, taking into account any parent matrixes that
        /// need to be applied.
        /// </returns>
        Matrix GetFinalMatrix();
    }
}