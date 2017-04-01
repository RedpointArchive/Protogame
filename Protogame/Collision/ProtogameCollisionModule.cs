using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The collision module for Protogame.  This module provides services for 2D and advanced collision detection.
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

            kernel.Bind<IPerPixelCollisionEngine>().To<DefaultPerPixelCollisionEngine>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<PerPixelCollisionEngineHook>();
            kernel.Bind<IPerPixelCollisionFactory>().ToFactory();

            kernel.Bind<IEventEngine<IPerPixelCollisionEventContext>>().To<DefaultEventEngine<IPerPixelCollisionEventContext>>().InSingletonScope();
            kernel.Bind<IEventBinder<IPerPixelCollisionEventContext>>().To<GeneralPerPixelCollisionEventBinder>().InSingletonScope();
        }
    }
}
