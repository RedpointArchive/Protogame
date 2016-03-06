using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// The HasMatrix interface.
    /// </summary>
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