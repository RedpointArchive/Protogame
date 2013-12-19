using Ninject.Modules;
using Protogame;

namespace ProtogameAssetManager
{
    internal class AssetManagerIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ISkin>().To<BasicSkin>();
            this.Bind<IBasicSkin>().To<AssetManagerBasicSkin>();
        }
    }
}

