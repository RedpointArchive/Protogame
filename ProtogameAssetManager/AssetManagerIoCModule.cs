using Protoinject;
using Protogame;

namespace ProtogameAssetManager
{
    internal class AssetManagerIoCModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<ISkin>().To<BasicSkin>();
            kernel.Bind<IBasicSkin>().To<AssetManagerBasicSkin>();
        }
    }
}

