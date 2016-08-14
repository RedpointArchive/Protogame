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
            if (kernel.TryGet<IRawLaunchArguments>() == null)
            {
                kernel.Bind<IRawLaunchArguments>()
                    .ToMethod(x => new DefaultRawLaunchArguments(new string[0]))
                    .InSingletonScope();
            }

            kernel.Bind<IAssetLoader>().To<FontAssetLoader>();
            kernel.Bind<IAssetLoader>().To<LanguageAssetLoader>();
            kernel.Bind<IAssetLoader>().To<TextureAssetLoader>();
            kernel.Bind<IAssetLoader>().To<LevelAssetLoader>();
            kernel.Bind<IAssetLoader>().To<AudioAssetLoader>();
            kernel.Bind<IAssetLoader>().To<TilesetAssetLoader>();
            kernel.Bind<IAssetLoader>().To<EffectAssetLoader>();
            kernel.Bind<IAssetLoader>().To<UberEffectAssetLoader>();
            kernel.Bind<IAssetLoader>().To<AIAssetLoader>();
            kernel.Bind<IAssetLoader>().To<ModelAssetLoader>();
            kernel.Bind<IAssetLoader>().To<TextureAtlasAssetLoader>();
            kernel.Bind<IAssetLoader>().To<VariableAssetLoader>();
            kernel.Bind<IAssetLoader>().To<ConfigurationAssetLoader>();
            kernel.Bind<IAssetLoader>().To<UserInterfaceAssetLoader>();
            kernel.Bind<IAssetSaver>().To<FontAssetSaver>();
            kernel.Bind<IAssetSaver>().To<LanguageAssetSaver>();
            kernel.Bind<IAssetSaver>().To<TextureAssetSaver>();
            kernel.Bind<IAssetSaver>().To<LevelAssetSaver>();
            kernel.Bind<IAssetSaver>().To<AudioAssetSaver>();
            kernel.Bind<IAssetSaver>().To<TilesetAssetSaver>();
            kernel.Bind<IAssetSaver>().To<EffectAssetSaver>();
            kernel.Bind<IAssetSaver>().To<UberEffectAssetSaver>();
            kernel.Bind<IAssetSaver>().To<ModelAssetSaver>();
            kernel.Bind<IAssetSaver>().To<TextureAtlasAssetSaver>();
            kernel.Bind<IAssetSaver>().To<VariableAssetSaver>();
            kernel.Bind<IAssetSaver>().To<ConfigurationAssetSaver>();
            kernel.Bind<IAssetSaver>().To<UserInterfaceAssetSaver>();
            kernel.Bind<IRawAssetLoader>().To<RawAssetLoader>();
            kernel.Bind<IRawAssetSaver>().To<RawAssetSaver>();
            kernel.Bind<ITransparentAssetCompiler>().To<DefaultTransparentAssetCompiler>();
            kernel.Bind<IModelSerializer>().To<ModelSerializerGeneric>();
            kernel.Bind<ITextureFromHintPath>().To<TextureFromHintPath>().InSingletonScope();
            kernel.Bind<IEffectSemantic>().To<WorldViewProjectionEffectSemantic>();
            kernel.Bind<IEffectSemantic>().To<TextureEffectSemantic>();
            kernel.Bind<IEffectSemantic>().To<BonesEffectSemantic>();
            kernel.Bind<IEffectSemantic>().To<ColorDiffuseEffectSemantic>();
            kernel.Bind<IEffectSemantic>().To<ScreenDimensionsEffectSemantic>();
            kernel.Bind<IEffectSemantic>().To<NormalMapEffectSemantic>();
            kernel.Bind<IEffectSemantic>().To<SpecularEffectSemantic>();
            kernel.Bind<IEffectSemantic>().To<CameraPositionEffectSemantic>();

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
            kernel.Bind<IAutomaticAssetReload>().To<DefaultAutomaticAssetReload>().InSingletonScope();
#if DEBUG
            this.LoadRawAssetStrategies(kernel);
#endif
            kernel.Bind<ILoadStrategy>().To<LocalSourceLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<EmbeddedSourceLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<LocalCompiledLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<EmbeddedCompiledLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<AssemblyLoadStrategy>();
#elif PLATFORM_ANDROID || PLATFORM_OUYA
            kernel.Bind<IAutomaticAssetReload>().To<DisabledAutomaticAssetReload>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<AndroidSourceLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<AndroidCompiledLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<EmbeddedCompiledLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<EmbeddedSourceLoadStrategy>();
#elif PLATFORM_IOS || PLATFORM_TVOS
            kernel.Bind<IAutomaticAssetReload>().To<DisabledAutomaticAssetReload>().InSingletonScope();
            // TODO: We still need to implement load strategies for normal content on iOS.
            kernel.Bind<ILoadStrategy>().To<EmbeddedCompiledLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<EmbeddedSourceLoadStrategy>();
#endif

            // MonoGame compilation requires 64-bit for content compilation.
            if (IntPtr.Size == 8)
            {
#if PLATFORM_WINDOWS || PLATFORM_LINUX
                kernel.Bind<IAssetCompiler<TextureAsset>>().To<TextureAssetCompiler>();
                kernel.Bind<IAssetCompiler<ModelAsset>>().To<ModelAssetCompiler>();
                kernel.Bind<IAssetCompiler<AudioAsset>>().To<AudioAssetCompiler>();
                kernel.Bind<IAssetCompiler<TextureAtlasAsset>>().To<TextureAtlasAssetCompiler>();
#if PLATFORM_WINDOWS
                kernel.Bind<IAssetCompiler<EffectAsset>>().To<EffectAssetCompiler>();
                kernel.Bind<IAssetCompiler<UberEffectAsset>>().To<UberEffectAssetCompiler>();
                kernel.Bind<IAssetCompiler<FontAsset>>().To<FontAssetCompiler>();
#elif PLATFORM_LINUX
                kernel.Bind<IAssetCompiler<EffectAsset>>().To<EffectAssetRemoteCompiler>();
                kernel.Bind<IAssetCompiler<FontAsset>>().To<FontAssetRemoteCompiler>();
#endif
#endif
            }
        }

        public void LoadRawAssetStrategies(IKernel kernel)
        {
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
            kernel.Bind<ILoadStrategy>().To<RawTextureLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawEffectLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawModelLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawAudioLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawATFLevelLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawOgmoEditorLevelLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawLogicControlScriptLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawConfigurationLoadStrategy>();
            kernel.Bind<ILoadStrategy>().To<RawUserInterfaceLoadStrategy>();
#endif
        }
    }
}
