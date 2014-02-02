namespace Protogame
{
    using Ninject.Modules;

    /// <summary>
    /// The protogame collision io c module.
    /// </summary>
    public class ProtogameCollisionIoCModule : NinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public override void Load()
        {
            this.Bind<ICollision>().To<DefaultCollision>();
        }
    }
}