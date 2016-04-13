using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The collision module for Protogame.  This module provides services for advanced collision detection.
    /// </summary>
    /// <module>Collision</module>
    public class ProtogameCollisionModule : IProtoinjectModule
    {
        /// <summary>
        /// Loads the collision module into the kernel.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<ICollision>().To<DefaultCollision>();
        }
    }
}
