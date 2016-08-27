// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Protoinject;

namespace Protogame
{
    /// <summary>
    /// This modules provides services and implementations related to debugging and
    /// diagnostics in a game.  You can load it by calling <see cref="Protoinject.IKernel.Load{T}"/>
    /// during the execution of <see cref="IGameConfiguration.ConfigureKernel"/>.
    /// </summary>
    /// <module>Debug</module>
    public class ProtogameDebugModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Rebind<IDebugRenderer>().To<DefaultDebugRenderer>().InSingletonScope();
        }
    }
}
