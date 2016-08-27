// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Protoinject;

namespace Protogame
{
    /// <summary>
    /// This modules provides services and implementations related to physics in a game.
    /// You can load it by calling <see cref="Protoinject.IKernel.Load{T}"/>
    /// during the execution of <see cref="IGameConfiguration.ConfigureKernel"/>.
    /// </summary>
    /// <module>Physics</module>
    public class ProtogamePhysicsModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<IPhysicsEngine>().To<DefaultPhysicsEngine>().InSingletonScope();
            kernel.Bind<IEngineHook>().To<PhysicsEngineHook>();
            kernel.Bind<IPhysicsFactory>().ToFactory();

            kernel.Bind<IEventEngine<IPhysicsEventContext>>().To<DefaultEventEngine<IPhysicsEventContext>>().InSingletonScope();
            kernel.Bind<IEventBinder<IPhysicsEventContext>>().To<GeneralPhysicsEventBinder>().InSingletonScope();
        }
    }
}
