namespace Protogame
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// An interface which represents an object that contains
    /// Canvas instances that should receive events.
    /// </summary>
    public interface IHasCanvases
    {
        /// <summary>
        /// Gets the Canvas objects.
        /// </summary>
        /// <value>
        /// The Canvas objects.
        /// </value>
        IEnumerable<KeyValuePair<Canvas, Rectangle>> Canvases { get; }
    }
}