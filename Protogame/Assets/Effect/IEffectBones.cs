namespace Protogame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The EffectBones interface.
    /// </summary>
    internal interface IEffectBones
    {
        /// <summary>
        /// Gets the bones array.
        /// </summary>
        /// <value>
        /// The bones.
        /// </value>
        Matrix[] Bones { get; }
    }
}