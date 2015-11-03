namespace Protogame
{
    using Ninject;
    using Ninject.Modules;

    /// <summary>
    /// The protogame ui io c module.
    /// </summary>
    public class ProtogameUIIoCModule : NinjectModule
    {
        /// <summary>
        /// The load.
        /// </summary>
        public override void Load()
        {
        }

        /// <remarks>
        /// Ninject doesn't offer us any way of specifying a default binding
        /// without causing multiple bindings to occur when a user uses
        /// the normal Bind method.  We use this static method as a way of
        /// getting a default skin for any internal services.
        /// </remarks>
        public static ISkin GetDefaultSkin(IKernel kernel)
        {
            return new BasicSkin(
                new DefaultBasicSkin(),
                kernel.Get<I2DRenderUtilities>(),
                kernel.Get<IAssetManagerProvider>());
        }
    }
}