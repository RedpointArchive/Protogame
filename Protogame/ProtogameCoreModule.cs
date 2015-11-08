using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The core Protogame dependency injection module, which loads all of the core
    /// classes required for basic game functionality.  You must load this module
    /// for Protogame to work.
    /// </summary>
    /// <module>Core API</module>
    public class ProtogameCoreModule : IProtoinjectModule
    {
        /// <summary>
        /// You should call <see cref="ModuleLoadExtensions.Load{ProtogameCoreModule}"/> 
        /// instead of calling this method directly.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<I2DRenderUtilities>().To<Default2DRenderUtilities>();
            kernel.Bind<I3DRenderUtilities>().To<Default3DRenderUtilities>();
            kernel.Bind<IAudioUtilities>().To<DefaultAudioUtilities>();
            kernel.Bind<ITileUtilities>().To<DefaultTileUtilities>();
            kernel.Bind<IBoundingBoxUtilities>().To<DefaultBoundingBoxUtilities>();
            kernel.Bind<IGameContext>().To<DefaultGameContext>();
            kernel.Bind<IUpdateContext>().To<DefaultUpdateContext>();
            kernel.Bind<IKeyboardStringReader>().To<DefaultKeyboardStringReader>();
            kernel.Bind<IConsole>().To<DefaultConsole>().InSingletonScope();
            kernel.Bind<ICommand>().To<ExitCommand>();
            kernel.Bind<ICommand>().To<HelpCommand>();
            kernel.Bind<ICommand>().To<GCCommand>();
            kernel.Bind<IStringSanitizer>().To<DefaultStringSanitizer>();

            kernel.Bind<IRenderContext>().To<RenderPipelineRenderContext>();
            kernel.Bind<IRenderPipeline>().To<DefaultRenderPipeline>();
            kernel.Bind<IGraphicsBlit>().To<DefaultGraphicsBlit>();
            kernel.Bind<IGraphicsFactory>().ToFactory();
            kernel.Bind<IRenderTargetBackBufferUtilities>().To<DefaultRenderTargetBackBufferUtilities>();

            kernel.Bind<I2DDirectRenderPass>().To<Default2DDirectRenderPass>();
            kernel.Bind<I2DBatchedRenderPass>().To<Default2DBatchedRenderPass>();
            kernel.Bind<I3DRenderPass>().To<Default3DRenderPass>();
            kernel.Bind<IInvertPostProcessingRenderPass>().To<DefaultInvertPostProcessingRenderPass>();
            kernel.Bind<IBlurPostProcessingRenderPass>().To<DefaultBlurPostProcessingRenderPass>();
            kernel.Bind<ICustomPostProcessingRenderPass>().To<DefaultCustomPostProcessingRenderPass>();
            kernel.Bind<ICaptureCopyPostProcessingRenderPass>().To<DefaultCaptureCopyPostProcessingRenderPass>();
            kernel.Bind<ICaptureInlinePostProcessingRenderPass>().To<DefaultCaptureInlinePostProcessingRenderPass>();
        }
    }
}

