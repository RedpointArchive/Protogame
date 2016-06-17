using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The Protoinject module to load when using the server services in Protogame.
    /// </summary>
    public class ProtogameServerModule : ProtogameBaseModule
    {
        /// <summary>
        /// You should call <see cref="Protoinject.StandardKernel.Load{ProtogameCoreModule}"/> 
        /// instead of calling this method directly.
        /// </summary>
        public override void Load(IKernel kernel)
        {
            base.Load(kernel);

            kernel.Bind<ITickRegulator>().To<DefaultTickRegulator>().InSingletonScope();
            kernel.Bind<IUniqueIdentifierAllocator>().To<DefaultUniqueIdentifierAllocator>().InSingletonScope();

            kernel.Bind<IServerContext>().To<DefaultServerContext>();
            kernel.Bind<IUpdateContext>().To<DefaultUpdateContext>();

            // Binds all of the services normally available under ProtogameCoreModule to
            // dummy implementations, because the GPU and various other API calls that
            // would normally be available with the MonoGame game framework running aren't
            // available in a server.
            LoadNullServices(kernel);
        }

        private void LoadNullServices(IKernel kernel)
        {
            kernel.Bind<IAssetContentManager>().To<NullAssetContentManager>().InSingletonScope();

            kernel.Bind<I2DRenderUtilities>().To<Null2DRenderUtilities>().InSingletonScope();
            kernel.Bind<I3DRenderUtilities>().To<Null3DRenderUtilities>().InSingletonScope();
            kernel.Bind<IAudioUtilities>().To<NullAudioUtilities>().InSingletonScope();
            kernel.Bind<IKeyboardStringReader>().To<NullKeyboardStringReader>().InSingletonScope();
            kernel.Bind<IConsole>().To<ServerConsole>().InSingletonScope();

            kernel.Bind<IRenderContext>().To<NullRenderContext>();

            kernel.Bind<IGraphicsFactory>().ToFactoryNotSupported();
            kernel.Bind<ILightFactory>().ToFactoryNotSupported();

            //kernel.Bind<IRenderPipeline>().To<DefaultRenderPipeline>();
            //kernel.Bind<IGraphicsBlit>().To<DefaultGraphicsBlit>();
            //kernel.Bind<IRenderTargetBackBufferUtilities>().To<DefaultRenderTargetBackBufferUtilities>();
            //kernel.Bind<IRenderContextImplementationUtilities>()
            //    .To<DefaultRenderContextImplementationUtilities>()
            //    .InSingletonScope();
            //kernel.Bind<IModelRenderConfiguration>().To<DefaultModelRenderConfiguration>().InSingletonScope();

            /*
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

            kernel.Bind<IStandardDirectionalLight>().To<DefaultStandardDirectionalLight>().AllowManyPerScope();
            kernel.Bind<IStandardPointLight>().To<DefaultStandardPointLight>().AllowManyPerScope();

            kernel.Bind<IDebugRenderer>().To<NullDebugRenderer>().InSingletonScope();
            */
        }
    }

    public class NullRenderContext : IRenderContext
    {
        public BoundingFrustum BoundingFrustum { get; }
        public Effect Effect { get; }
        public GraphicsDevice GraphicsDevice { get; }
        public bool Is3DContext { get; set; }
        public bool IsRendering { get; set; }
        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraLookAt { get; set; }
        public Matrix Projection { get; set; }
        public Texture2D SingleWhitePixel { get; }
        public SpriteBatch SpriteBatch { get; }
        public Matrix View { get; set; }
        public Matrix World { get; set; }
        public void EnableTextures()
        {
            throw new NotSupportedException();
        }

        public void EnableVertexColors()
        {
            throw new NotSupportedException();
        }

        public Effect PopEffect()
        {
            throw new NotSupportedException();
        }

        public void PopRenderTarget()
        {
            throw new NotSupportedException();
        }

        public void PushEffect(Effect effect)
        {
            throw new NotSupportedException();
        }

        public void PushRenderTarget(RenderTargetBinding renderTarget)
        {
            throw new NotSupportedException();
        }

        public void PushRenderTarget(params RenderTargetBinding[] renderTargets)
        {
            throw new NotSupportedException();
        }

        public void Render(IGameContext context)
        {
            throw new NotSupportedException();
        }

        public void SetActiveTexture(Texture2D texture)
        {
            throw new NotSupportedException();
        }

        public IRenderPass AddFixedRenderPass(IRenderPass renderPass)
        {
            throw new NotSupportedException();
        }

        public void RemoveFixedRenderPass(IRenderPass renderPass)
        {
            throw new NotSupportedException();
        }

        public IRenderPass AppendTransientRenderPass(IRenderPass renderPass)
        {
            throw new NotSupportedException();
        }

        public IRenderPass CurrentRenderPass { get; }
        public bool IsCurrentRenderPass<T>() where T : class, IRenderPass
        {
            throw new NotSupportedException();
        }

        public bool IsCurrentRenderPass<T>(out T currentRenderPass) where T : class, IRenderPass
        {
            throw new NotSupportedException();
        }

        public T GetCurrentRenderPass<T>() where T : class, IRenderPass
        {
            throw new NotSupportedException();
        }

        public bool IsFirstRenderPass()
        {
            throw new NotSupportedException();
        }
    }

    public class ServerConsole : IConsole
    {
        public bool Open => true;

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            throw new NotSupportedException();
        }

        public void Toggle()
        {
            throw new NotSupportedException();
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            throw new NotSupportedException();
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class NullKeyboardStringReader : IKeyboardStringReader
    {
        public int FirstRepeatKeyInterval { get; set; }
        public int RepeatKeyInterval { get; set; }
        public void Process(KeyboardState keyboard, GameTime time, StringBuilder text)
        {
            throw new NotSupportedException();
        }
    }
}

