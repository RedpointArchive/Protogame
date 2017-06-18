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
            
            kernel.Bind<I2DRenderUtilities>().To<Default2DRenderUtilities>().InSingletonScope();
            kernel.Bind<I3DRenderUtilities>().To<Default3DRenderUtilities>().InSingletonScope();
            kernel.Bind<IAudioUtilities>().To<DefaultAudioUtilities>().InSingletonScope();
            kernel.Bind<IGameContext>().To<DefaultGameContext>();
            kernel.Bind<IUpdateContext>().To<DefaultUpdateContext>();
            kernel.Bind<IKeyboardStringReader>().To<DefaultKeyboardStringReader>().InSingletonScope();
            kernel.Bind<IConsole>().To<ClientConsole>().InSingletonScope();
            kernel.Bind<IConsoleRender>().To<ClientConsoleRender>().InSingletonScope();
            kernel.Bind<IConsoleInput>().To<ClientConsoleInput>().InSingletonScope();
            kernel.Bind<IConsoleHandle>().To<DefaultConsoleHandle>().InParentScope();
            kernel.Bind<ICommand>().To<ExitCommand>().DiscardNodeOnResolve();
            kernel.Bind<ICommand>().To<HelpCommand>().DiscardNodeOnResolve();
            kernel.Bind<ICommand>().To<GCCommand>().DiscardNodeOnResolve();

            kernel.Bind<IRenderContext>().To<RenderPipelineRenderContext>().InParentScope();
            kernel.Bind<IRenderPipeline>().To<DefaultRenderPipeline>().InParentScope();
            kernel.Bind<IGraphicsBlit>().To<DefaultGraphicsBlit>().InParentScope();
            kernel.Bind<IGraphicsFactory>().ToFactory().DiscardNodeOnResolve();
            kernel.Bind<IRenderTargetBackBufferUtilities>().To<DefaultRenderTargetBackBufferUtilities>().InSingletonScope();
            kernel.Bind<IModelRenderConfiguration>().To<DefaultModelRenderConfiguration>().InSingletonScope();

            kernel.Bind<I2DDirectRenderPass>().To<Default2DDirectRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<I2DBatchedRenderPass>().To<Default2DBatchedRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<I2DBatchedLoadingScreenRenderPass>().To<Default2DBatchedLoadingScreenRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<ICanvasRenderPass>().To<DefaultCanvasRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<I3DRenderPass>().To<Default3DForwardRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<I3DForwardRenderPass>().To<Default3DForwardRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<I3DDeferredRenderPass>().To<Default3DDeferredRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<IDebugRenderPass>().To<DefaultDebugRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<IConsoleRenderPass>().To<DefaultConsoleRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<IInvertPostProcessingRenderPass>().To<DefaultInvertPostProcessingRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<IBlurPostProcessingRenderPass>().To<DefaultBlurPostProcessingRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<ICustomPostProcessingRenderPass>().To<DefaultCustomPostProcessingRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<ICaptureCopyPostProcessingRenderPass>().To<DefaultCaptureCopyPostProcessingRenderPass>().DiscardNodeOnResolve();
            kernel.Bind<ICaptureInlinePostProcessingRenderPass>().To<DefaultCaptureInlinePostProcessingRenderPass>().DiscardNodeOnResolve();

            kernel.Bind<ILightFactory>().ToFactory().DiscardNodeOnResolve();
            kernel.Bind<IStandardDirectionalLight>().To<DefaultStandardDirectionalLight>().InParentScope().AllowManyPerScope();
            kernel.Bind<IStandardPointLight>().To<DefaultStandardPointLight>().InParentScope().AllowManyPerScope();
            kernel.Bind<ILightRenderer<IStandardDirectionalLight>>().To<DefaultStandardDirectionalLightRenderer>().InSingletonScope();
            kernel.Bind<ILightRenderer<IStandardPointLight>>().To<DefaultStandardPointLightRenderer>().InSingletonScope();

            kernel.Bind<IDebugRenderer>().To<NullDebugRenderer>().InSingletonScope();

            kernel.Bind<IRenderBatcher>().To<DefaultRenderBatcher>().InSingletonScope();

            kernel.Bind<ILoadingScreen>().To<DefaultLoadingScreen>().InSingletonScope();

            kernel.Bind<IInterlacedBatchingDepthProvider>().To<DefaultInterlacedBatchingDepthProvider>().InSingletonScope();
        }
    }
}

