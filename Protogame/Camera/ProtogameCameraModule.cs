using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The camera Protogame dependency injection module, which provides utility classes for
    /// setting up game cameras.
    /// </summary>
    /// <module>Camera</module>
    public class ProtogameCameraModule : IProtoinjectModule
    {
        /// <summary>
        /// You should call <see cref="ModuleLoadExtensions.Load{ProtogameCameraModule}"/> 
        /// instead of calling this method directly.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<IFirstPersonCamera>().To<FirstPersonCamera>().InSingletonScope();
        }
    }
}
