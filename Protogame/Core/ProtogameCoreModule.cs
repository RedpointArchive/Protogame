using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The core Protogame dependency injection module, which loads all of the core
    /// classes required for basic game functionality.  You must load this module
    /// for Protogame to work.
    /// </summary>
    /// <module>Core API</module>
    public class ProtogameCoreModule : ProtogameBaseModule
    {
        /// <summary>
        /// You should call <see cref="Protoinject.StandardKernel.Load{ProtogameCoreModule}"/> 
        /// instead of calling this method directly.
        /// </summary>
        public override void Load(IKernel kernel)
        {
            base.Load(kernel);

            kernel.Bind<I2DRenderUtilities>().To<Default2DRenderUtilities>();
            kernel.Bind<I3DRenderUtilities>().To<Default3DRenderUtilities>();
            kernel.Bind<IAudioUtilities>().To<DefaultAudioUtilities>();
            kernel.Bind<IGameContext>().To<DefaultGameContext>();
            kernel.Bind<IUpdateContext>().To<DefaultUpdateContext>();
            kernel.Bind<IKeyboardStringReader>().To<DefaultKeyboardStringReader>();
            kernel.Bind<IConsole>().To<DefaultConsole>().InSingletonScope();
            kernel.Bind<ICommand>().To<ExitCommand>();
            kernel.Bind<ICommand>().To<HelpCommand>();
            kernel.Bind<ICommand>().To<GCCommand>();

            kernel.Bind<IRenderContext>().To<RenderPipelineRenderContext>();
            kernel.Bind<IRenderPipeline>().To<DefaultRenderPipeline>();
            kernel.Bind<IGraphicsBlit>().To<DefaultGraphicsBlit>();
            kernel.Bind<IGraphicsFactory>().ToFactory();
            kernel.Bind<IRenderTargetBackBufferUtilities>().To<DefaultRenderTargetBackBufferUtilities>();
            kernel.Bind<IRenderContextImplementationUtilities>()
                .To<DefaultRenderContextImplementationUtilities>()
                .InSingletonScope();
            kernel.Bind<IModelRenderConfiguration>().To<DefaultModelRenderConfiguration>().InSingletonScope();

            kernel.Bind<I2DDirectRenderPass>().To<Default2DDirectRenderPass>().AllowManyPerScope();
            kernel.Bind<I2DBatchedRenderPass>().To<Default2DBatchedRenderPass>().AllowManyPerScope();
            kernel.Bind<ICanvasRenderPass>().To<DefaultCanvasRenderPass>().AllowManyPerScope();
            kernel.Bind<I3DRenderPass>().To<Default3DForwardRenderPass>().AllowManyPerScope();
            kernel.Bind<I3DForwardRenderPass>().To<Default3DForwardRenderPass>().AllowManyPerScope();
            kernel.Bind<I3DDeferredRenderPass>().To<Default3DDeferredRenderPass>().AllowManyPerScope();
            kernel.Bind<IDebugRenderPass>().To<DefaultDebugRenderPass>().AllowManyPerScope();
            kernel.Bind<IConsoleRenderPass>().To<DefaultConsoleRenderPass>().AllowManyPerScope();
            kernel.Bind<IInvertPostProcessingRenderPass>().To<DefaultInvertPostProcessingRenderPass>().AllowManyPerScope();
            kernel.Bind<IBlurPostProcessingRenderPass>().To<DefaultBlurPostProcessingRenderPass>().AllowManyPerScope();
            kernel.Bind<ICustomPostProcessingRenderPass>().To<DefaultCustomPostProcessingRenderPass>().AllowManyPerScope();
            kernel.Bind<ICaptureCopyPostProcessingRenderPass>().To<DefaultCaptureCopyPostProcessingRenderPass>().AllowManyPerScope();
            kernel.Bind<ICaptureInlinePostProcessingRenderPass>().To<DefaultCaptureInlinePostProcessingRenderPass>().AllowManyPerScope();

            kernel.Bind<ILightFactory>().ToFactory();
            kernel.Bind<IStandardDirectionalLight>().To<DefaultStandardDirectionalLight>().AllowManyPerScope();
            kernel.Bind<IStandardPointLight>().To<DefaultStandardPointLight>().AllowManyPerScope();
            
            kernel.Bind<IDebugRenderer>().To<NullDebugRenderer>().InSingletonScope();
        }
    }
}

