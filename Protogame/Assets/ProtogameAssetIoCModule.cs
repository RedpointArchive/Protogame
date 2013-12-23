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
            this.Bind<IAssetLoader>().To<AIAssetLoader>();
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

#if DEBUG
            this.Bind<ILoadStrategy>().To<RawTextureLoadStrategy>();
            this.Bind<ILoadStrategy>().To<RawEffectLoadStrategy>();
#endif
            this.Bind<ILoadStrategy>().To<LocalSourceLoadStrategy>();
            this.Bind<ILoadStrategy>().To<EmbeddedSourceLoadStrategy>();
            this.Bind<ILoadStrategy>().To<LocalCompiledLoadStrategy>();
            this.Bind<ILoadStrategy>().To<EmbeddedCompiledLoadStrategy>();
            this.Bind<ILoadStrategy>().To<AssemblyLoadStrategy>();

#if PLATFORM_WINDOWS
            this.Bind<IAssetCompiler<TextureAsset>>().To<TextureAssetCompiler>();
            this.Bind<IAssetCompiler<FontAsset>>().To<FontAssetCompiler>();
            this.Bind<IAssetCompiler<EffectAsset>>().To<EffectAssetCompiler>();
#elif PLATFORM_LINUX
            this.Bind<IAssetCompiler<TextureAsset>>().To<TextureAssetCompiler>();
            this.Bind<IAssetCompiler<FontAsset>>().To<FontAssetCompiler>();
#endif
        }
    }
}

