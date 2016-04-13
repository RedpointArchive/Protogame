namespace Protogame
{
    using System.Collections.Generic;

    /// <summary>
    /// A collection of animations, accessible by the animation's name.
    /// </summary>
    public interface IAnimationCollection : IEnumerable<IAnimation>
    {
        /// <summary>
        /// Retrieve an animation by the animation's name.
        /// </summary>
        /// <param name="name">
        /// The name of the animation.
        /// </param>
        /// <returns>
        /// The <see cref="IAnimation"/>, or null if there's no animation with this name.
        /// </returns>
        IAnimation this[string name] { get; }
    }
}