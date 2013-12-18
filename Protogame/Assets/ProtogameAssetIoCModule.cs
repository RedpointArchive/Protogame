using Ninject;
using Ninject.Modules;

namespace Protogame
{
    public class ProtogameAssetIoCModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IAssetLoader>().To<FontAssetLoader>();
            this.Bind<IAssetLoader>().To<LanguageAssetLoader>();
            this.Bind<IAssetLoader>().To<TextureAssetLoader>();
            this.Bind<IAssetLoader>().To<LevelAssetLoader>();
            this.Bind<IAssetLoader>().To<AudioAssetLoader>();
            this.Bind<IAssetLoader>().To<TilesetAssetLoader>();
            this.Bind<IAssetLoader>().To<EffectAssetLoader>();
            this.Bind<IAssetSaver>().To<FontAssetSaver>();
            this.Bind<IAssetSaver>().To<LanguageAssetSaver>();
            this.Bind<IAssetSaver>().To<TextureAssetSaver>();
            this.Bind<IAssetSaver>().To<LevelAssetSaver>();
            this.Bind<IAssetSaver>().To<AudioAssetSaver>();
            this.Bind<IAssetSaver>().To<TilesetAssetSaver>();
            this.Bind<IAssetSaver>().To<EffectAssetSaver>();
            this.Bind<IRawAssetLoader>().To<RawAssetLoader>();
            this.Bind<IRawAssetSaver>().To<RawAssetSaver>();
            this.Bind<ITransparentAssetCompiler>().To<DefaultTransparentAssetCompiler>();

#if PLATFORM_WINDOWS
            this.Bind<IAssetCompiler<TextureAsset>>().To<TextureAssetCompiler>();
            this.Bind<IAssetCompiler<FontAsset>>().To<FontAssetCompiler>();
#endif
        }
    }
}

