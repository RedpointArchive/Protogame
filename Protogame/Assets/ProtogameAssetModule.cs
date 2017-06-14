using System;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The asset management module, which provides functionality for loading, saving,
    /// compiling and using game assets.  Loading this module is recommended, but you
    /// can omit it if you want to use your own asset management system.
    /// </summary>
    /// <module>Assets</module>
    public class ProtogameAssetModule : IProtoinjectModule
    {
        public void Load(IKernel kernel)
        {
            kernel.Bind<IAssetManager>().To<DefaultAssetManager>().InSingletonScope();

            kernel.Bind<IAssetLoader<AudioAsset>>().To<AudioAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader<EffectAsset>>().To<EffectAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader<UberEffectAsset>>().To<UberEffectAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader<FontAsset>>().To<FontAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader<LevelAsset>>().To<LevelAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader<ModelAsset>>().To<ModelAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader<TextureAsset>>().To<TextureAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader<UserInterfaceAsset>>().To<UserInterfaceAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader<TilesetAsset>>().To<TilesetAssetLoader>().InSingletonScope();

            kernel.Bind<IModelSerializer>().To<ModelSerializerGeneric>().InSingletonScope();
            kernel.Bind<ITextureFromHintPath>().To<TextureFromHintPath>().InSingletonScope();
            kernel.Bind<IEffectSemantic>().To<WorldViewProjectionEffectSemantic>().DiscardNodeOnResolve();
            kernel.Bind<IEffectSemantic>().To<TextureEffectSemantic>().DiscardNodeOnResolve();
            kernel.Bind<IEffectSemantic>().To<BonesEffectSemantic>().DiscardNodeOnResolve();
            kernel.Bind<IEffectSemantic>().To<ColorDiffuseEffectSemantic>().DiscardNodeOnResolve();
            kernel.Bind<IEffectSemantic>().To<ScreenDimensionsEffectSemantic>().DiscardNodeOnResolve();
            kernel.Bind<IEffectSemantic>().To<NormalMapEffectSemantic>().DiscardNodeOnResolve();
            kernel.Bind<IEffectSemantic>().To<SpecularEffectSemantic>().DiscardNodeOnResolve();
            kernel.Bind<IEffectSemantic>().To<CameraPositionEffectSemantic>().DiscardNodeOnResolve();

            kernel.Bind<IAssetFs>().To<AssetFs>().InSingletonScope();
            kernel.Bind<ICompiledAssetFs>().To<RuntimeCompiledAssetFs>().InSingletonScope();
            kernel.Bind<IAssetFsLayer>().To<SourceEmbeddedAssetFsLayer>().InSingletonScope();
            kernel.Bind<IAssetFsLayer>().To<CompiledEmbeddedAssetFsLayer>().InSingletonScope();
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
#if DEBUG
            kernel.Bind<IAssetFsLayer>().To<SourceDirLocalFilesystemAssetFsLayer>().InSingletonScope();
            kernel.Bind<IAssetFsLayer>().To<CurrentDirLocalFilesystemAssetFsLayer>().InSingletonScope();
#endif
            kernel.Bind<IAssetFsLayer>().To<CurrentDirPlatformLocalFilesystemAssetFsLayer>().InSingletonScope();
#elif PLATFORM_ANDROID
#if DEBUG
            kernel.Bind<IRemoteClientInboundHandler>().To<RemoteClientAssetFsInboundHandler>().InSingletonScope();
            kernel.Bind<IAssetFsLayer>().To<RemoteClientAssetFsLayer>().InSingletonScope();
#endif
            kernel.Bind<IAssetFsLayer>().To<RootAndroidAssetFsLayer>().InSingletonScope();
            kernel.Bind<IAssetFsLayer>().To<PlatformAndroidAssetFsLayer>().InSingletonScope();
#endif

#if DEBUG
            LoadRawAssetStrategies(kernel);
#endif
        }

        public void LoadRawAssetStrategies(IKernel kernel)
        {
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
            // MonoGame compilation requires 64-bit for content compilation.
            if (IntPtr.Size == 8)
            {
                kernel.Bind<IAssetCompiler>().To<AudioAssetCompiler>().InSingletonScope();
#if PLATFORM_WINDOWS
                kernel.Bind<IAssetCompiler>().To<EffectAssetCompiler>().InSingletonScope();
                kernel.Bind<IAssetCompiler>().To<UberEffectAssetCompiler>().InSingletonScope();
                kernel.Bind<IAssetCompiler>().To<FontAssetCompiler>().InSingletonScope();
#endif
#if PLATFORM_WINDOWS || PLATFORM_LINUX
                kernel.Bind<IAssetCompiler>().To<ModelAssetCompiler>().InSingletonScope();
                kernel.Bind<IAssetCompiler>().To<TextureAssetCompiler>().InSingletonScope();
#endif
            }

            kernel.Bind<IAssetCompiler>().To<AtfLevelAssetCompiler>().InSingletonScope();
            kernel.Bind<IAssetCompiler>().To<OgmoLevelAssetCompiler>().InSingletonScope();
            kernel.Bind<IAssetCompiler>().To<UserInterfaceAssetCompiler>().InSingletonScope();
            kernel.Bind<IAssetCompiler>().To<TilesetAssetCompiler>().InSingletonScope();
#endif
        }
    }
}
