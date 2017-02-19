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

            kernel.Bind<IAssetManager>().To<DefaultAssetManager>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<FontAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<LanguageAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<TextureAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<LevelAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<AudioAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<TilesetAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<EffectAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<UberEffectAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<AIAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<ModelAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<VariableAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<ConfigurationAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetLoader>().To<UserInterfaceAssetLoader>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<FontAssetSaver>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<LanguageAssetSaver>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<TextureAssetSaver>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<LevelAssetSaver>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<AudioAssetSaver>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<TilesetAssetSaver>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<EffectAssetSaver>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<UberEffectAssetSaver>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<ModelAssetSaver>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<VariableAssetSaver>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<ConfigurationAssetSaver>().InSingletonScope();
            kernel.Bind<IAssetSaver>().To<UserInterfaceAssetSaver>().InSingletonScope();
            kernel.Bind<IRawAssetLoader>().To<RawAssetLoader>().InSingletonScope();
            kernel.Bind<IRawAssetSaver>().To<RawAssetSaver>().InSingletonScope();
            kernel.Bind<ITransparentAssetCompiler>().To<DefaultTransparentAssetCompiler>().InSingletonScope();
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

#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
#if DEBUG
            this.LoadRawAssetStrategies(kernel);
#endif
            kernel.Bind<ILoadStrategy>().To<LocalSourceLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<EmbeddedSourceLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<LocalCompiledLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<EmbeddedCompiledLoadStrategy>().InSingletonScope();
#elif PLATFORM_ANDROID || PLATFORM_OUYA
            kernel.Bind<ILoadStrategy>().To<AndroidSourceLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<AndroidCompiledLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<EmbeddedCompiledLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<EmbeddedSourceLoadStrategy>().InSingletonScope();
#elif PLATFORM_IOS || PLATFORM_TVOS
            // TODO: We still need to implement load strategies for normal content on iOS.
            kernel.Bind<ILoadStrategy>().To<EmbeddedCompiledLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<EmbeddedSourceLoadStrategy>().InSingletonScope();
#endif

            // MonoGame compilation requires 64-bit for content compilation.
            if (IntPtr.Size == 8)
            {
#if PLATFORM_WINDOWS || PLATFORM_LINUX
                kernel.Bind<IAssetCompiler<TextureAsset>>().To<TextureAssetCompiler>().InSingletonScope();
                kernel.Bind<IAssetCompiler<ModelAsset>>().To<ModelAssetCompiler>().InSingletonScope();
                kernel.Bind<IAssetCompiler<AudioAsset>>().To<AudioAssetCompiler>().InSingletonScope();
#if PLATFORM_WINDOWS
                kernel.Bind<IAssetCompiler<EffectAsset>>().To<EffectAssetCompiler>().InSingletonScope();
                kernel.Bind<IAssetCompiler<UberEffectAsset>>().To<UberEffectAssetCompiler>().InSingletonScope();
                kernel.Bind<IAssetCompiler<FontAsset>>().To<FontAssetCompiler>().InSingletonScope();
#elif PLATFORM_LINUX
                kernel.Bind<IAssetCompiler<EffectAsset>>().To<EffectAssetRemoteCompiler>().InSingletonScope();
                kernel.Bind<IAssetCompiler<FontAsset>>().To<FontAssetRemoteCompiler>().InSingletonScope();
#endif
#endif
            }
        }

        public void LoadRawAssetStrategies(IKernel kernel)
        {
#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX
            kernel.Bind<ILoadStrategy>().To<RawTextureLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<RawEffectLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<RawModelLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<RawAudioLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<RawATFLevelLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<RawOgmoEditorLevelLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<RawLogicControlScriptLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<RawConfigurationLoadStrategy>().InSingletonScope();
            kernel.Bind<ILoadStrategy>().To<RawUserInterfaceLoadStrategy>().InSingletonScope();
#endif
        }
    }
}
