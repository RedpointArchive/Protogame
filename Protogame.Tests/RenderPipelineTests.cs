using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Ninject;
using Xunit;

namespace Protogame.Tests
{
    public class RenderPipelineTests
    {
        public class RenderPipelineWorld : IWorld
        {
            private readonly I2DRenderUtilities _renderUtilities;

            private readonly TextureAsset _texture;

            private readonly IInvertPostProcessingRenderPass _invertPostProcess;

            private readonly IBlurPostProcessingRenderPass _blurPostProcess;

            private readonly ICustomPostProcessingRenderPass _customPostProcess;

            private readonly ICaptureInlinePostProcessingRenderPass _captureInlinePostProcess;

            private List<Tuple<bool, bool, bool>> _combinations = new List<Tuple<bool, bool, bool>>
            {
                new Tuple<bool, bool, bool>(false, false, false),
                new Tuple<bool, bool, bool>(false, false, true),
                new Tuple<bool, bool, bool>(false, true, false),
                new Tuple<bool, bool, bool>(false, true, true),
                new Tuple<bool, bool, bool>(true, false, false),
                new Tuple<bool, bool, bool>(true, false, true),
                new Tuple<bool, bool, bool>(true, true, false),
                new Tuple<bool, bool, bool>(true, true, true),
            };

            private const int Width = 800;

            private const int Height = 480;
            
            private int _frame;

            private int _manualTest;

            private bool _didExit;

            public RenderPipelineWorld(IAssetManagerProvider assetManagerProvider, I2DRenderUtilities renderUtilities, IGraphicsFactory graphicsFactory)
            {
                _renderUtilities = renderUtilities;
                _texture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture.Player");
                _invertPostProcess = graphicsFactory.CreateInvertPostProcessingRenderPass();
                _blurPostProcess = graphicsFactory.CreateBlurPostProcessingRenderPass();
                _customPostProcess = graphicsFactory.CreateCustomPostProcessingRenderPass("effect.MakeRed");
                _captureInlinePostProcess = graphicsFactory.CreateCaptureInlinePostProcessingRenderPass();
                _captureInlinePostProcess.RenderPipelineStateAvailable = d =>
                {
#if MANUAL_TEST
#elif RECORDING
                    using (var writer = new StreamWriter("output" + _frame + ".png"))
                    {
                        d.SaveAsPng(writer.BaseStream, Width, Height);
                    }
#else
                    var baseStream =
                        typeof (RenderPipelineWorld).Assembly.GetManifestResourceStream(
                            "Protogame.Tests.Expected.RenderPipeline.output" + _frame + ".png");
                    var baseBytes = new byte[baseStream.Length];
                    baseStream.Read(baseBytes, 0, baseBytes.Length);
                    var memoryStream = new MemoryStream();
                    d.SaveAsPng(memoryStream, Width, Height);
                    var memoryBytes = new byte[memoryStream.Position];
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.Read(memoryBytes, 0, memoryBytes.Length);
                    memoryStream.Dispose();
                    baseStream.Dispose();

                    Assert.Equal(baseBytes, memoryBytes);
#endif

#if MANUAL_TEST
                    _manualTest++;
                    if (_manualTest % 60 == 0)
                    {
                        _frame++;
                    }
#else
                    _frame++;
#endif
                };

                this.Entities = new List<IEntity>();
            }

            public IList<IEntity> Entities { get; private set; }

            public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
            {
            }

            public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
            {
                renderContext.GraphicsDevice.Clear(Color.Green);

#if !MANUAL_TEST
                if (_frame >= _combinations.Count)
                {
                    return;
                }
#endif

                if (renderContext.IsCurrentRenderPass<I2DBatchedRenderPass>())
                {
                    _renderUtilities.RenderTexture(renderContext, Vector2.Zero, _texture);

                    var combination = _combinations[_frame % _combinations.Count];

                    if (combination.Item1)
                    {
                        renderContext.AppendTransientRenderPass(_invertPostProcess);
                    }

                    if (combination.Item2)
                    {
                        renderContext.AppendTransientRenderPass(_blurPostProcess);
                    }

                    if (combination.Item3)
                    {
                        renderContext.AppendTransientRenderPass(_customPostProcess);
                    }

                    renderContext.AppendTransientRenderPass(_captureInlinePostProcess);
                }
            }

            public void Update(IGameContext gameContext, IUpdateContext updateContext)
            {
#if !MANUAL_TEST
                if (_didExit)
                {
                    return;
                }

                if (_frame == _combinations.Count)
                {
                    gameContext.Game.Exit();
                    _didExit = true;
                }
#endif
            }

            public void Dispose()
            {
            }
        }

        public class RenderPipelineGame : CoreGame<RenderPipelineWorld>
        {
            private const int Width = 800;

            private const int Height = 480;

            public RenderPipelineGame(IKernel kernel) : base(kernel)
            {
            }

            protected override void ConfigureRenderPipeline(IRenderPipeline pipeline, IKernel kernel)
            {
                var factory = kernel.Get<IGraphicsFactory>();
                
                pipeline.AddFixedRenderPass(factory.Create2DBatchedRenderPass());
            }

            protected override void PrepareDeviceSettings(GraphicsDeviceInformation deviceInformation)
            {
                base.PrepareDeviceSettings(deviceInformation);

                this.GraphicsDeviceManager.PreferredBackBufferWidth = Width;
                this.GraphicsDeviceManager.PreferredBackBufferHeight = Height;
            }
        }

        [Fact, Trait("IsFunctional", "True")]
        public void PerformRenderPipelineTest()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameCoreModule>();
            kernel.Load<ProtogameAssetIoCModule>();
            AssetManagerClient.AcceptArgumentsAndSetup<GameAssetManagerProvider>(kernel, new string[0]);

            using (var game = new RenderPipelineGame(kernel))
            {
                game.Run();
            }
        }
    }
}
