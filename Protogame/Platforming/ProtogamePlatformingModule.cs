namespace Protogame
{
    using Protoinject;

    /// <summary>
    /// The platforming module, which provides utility classes for platforming and other 2D games.
    /// </summary>
    public class ProtogamePlatformingModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<IPlatforming>().To<DefaultPlatforming>();
        }
    }
}