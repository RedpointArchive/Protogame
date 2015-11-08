namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The protogame collision io c module.
    /// </summary>
    public class ProtogameCollisionIoCModule : IProtoinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<ICollision>().To<DefaultCollision>();
        }
    }
}