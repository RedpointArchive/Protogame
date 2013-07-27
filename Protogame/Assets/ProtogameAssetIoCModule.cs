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
            this.Bind<IAssetSaver>().To<FontAssetSaver>();
            this.Bind<IAssetSaver>().To<LanguageAssetSaver>();
            this.Bind<IAssetSaver>().To<TextureAssetSaver>();
            this.Bind<IAssetSaver>().To<LevelAssetSaver>();
            this.Bind<IAssetSaver>().To<AudioAssetSaver>();
            this.Bind<IRawAssetLoader>().To<RawAssetLoader>();
            this.Bind<IRawAssetSaver>().To<RawAssetSaver>();
            
            this.Kernel.Load<ContentCompilationIoCModule>();
        }
    }
}

