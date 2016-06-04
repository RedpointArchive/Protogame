using System.Collections.Generic;

namespace Protogame
{
    /// <summary>
    /// A component which provides lights for the rendering system.
    /// </summary>
    /// <module>Component</module>
    public interface ILightableComponent
    {
        /// <summary>
        /// Called by the entity or parent component to retrieve a list of lights
        /// that should be rendered in the world.
        /// </summary>
        /// <returns>A list of lights to render in the world.</returns>
        IEnumerable<ILight> GetLights();
    }
}