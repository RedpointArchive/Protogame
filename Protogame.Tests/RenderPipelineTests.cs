using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Protoinject;
using Prototest.Library.Version1;
using Prototest.Library.Version13;
using ICategorize = Prototest.Library.Version13.ICategorize;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame.Tests
{
    public class RenderPipelineTests
    {
        private readonly IAssert _assert;
        private readonly ITestAttachment _testAttachment;
        private readonly ICategorize _categorize;

        public class RenderPipelineWorld : IWorld
        {
            private readonly I2DRenderUtilities _renderUtilities;
            private readonly IAssert _assert;
            private readonly ITestAttachment _testAttachment;

            private readonly IAssetReference<TextureAsset> _texture;

            private readonly IInvertPostProcessingRenderPass _invertPostProcess;

            private readonly IBlurPostProcessingRenderPass _blurPostProcess;

            private readonly ICustomPostProcessingRenderPass _customPostProcess;

            private readonly ICaptureInlinePostProcessingRenderPass _captureInlinePostProcess;

            private readonly IGraphicsBlit _graphicsBlit;

            private readonly IRenderTargetBackBufferUtilities _renderTargetBackBufferUtilities;

            private RenderTarget2D _renderTarget;

            private class Combination
            {
                public Combination(string id, string name, bool invert, bool blur, bool makeRed)
                {
                    Id = id;
                    Name = name;
                    Invert = invert;
                    Blur = blur;
                    MakeRed = makeRed;
                }

                public bool Invert { get; set; }

                public bool Blur { get; set; }

                public bool MakeRed { get; set; }

                public string Id { get; set; }

                public string Name { get; set; }

                public string FailureMessage { get; set; }
            }

            private List<Combination> _combinations = new List<Combination>
            {
                new Combination("noeffect", "No Effects", false, false, false),
                new Combination("makered", "Make Red", false, false, true),
                new Combination("blur", "Blur", false, true, false),
                new Combination("makeredblur", "Blur + Make Red", false, true, true),
                new Combination("invert", "Invert", true, false, false),
                new Combination("invertmakered", "Invert + Make Red", true, false, true),
                new Combination("invertblur", "Invert + Blur", true, true, false),
                new Combination("invertblurmakered", "Invert + Blur + Make Red", true, true, true),
            };

            private const int Width = 800;

            private const int Height = 480;
            
            private int _frame;

            private int _manualTest;

            private bool _didExit;

            private bool _isValidRun;

            public RenderPipelineWorld(
                IAssetManager assetManager, 
                I2DRenderUtilities renderUtilities, 
                IGraphicsFactory graphicsFactory, 
                IAssert assert, 
                ITestAttachment testAttachment, 
                IRenderTargetBackBufferUtilities renderTargetBackBufferUtilities, 
                IGraphicsBlit graphicsBlit)
            {
                _renderUtilities = renderUtilities;
                _assert = assert;
                _testAttachment = testAttachment;
                _texture = assetManager.Get<TextureAsset>("texture.Player");
                _invertPostProcess = graphicsFactory.CreateInvertPostProcessingRenderPass();
                _blurPostProcess = graphicsFactory.CreateBlurPostProcessingRenderPass();
                _customPostProcess = graphicsFactory.CreateCustomPostProcessingRenderPass("effect.MakeRed");
                _captureInlinePostProcess = graphicsFactory.CreateCaptureInlinePostProcessingRenderPass();
                _renderTargetBackBufferUtilities = renderTargetBackBufferUtilities;
                _graphicsBlit = graphicsBlit;
                _captureInlinePostProcess.RenderPipelineStateAvailable = (gameContext, renderContext, previousPass, d) =>
                {
                    if (!_isValidRun)
                    {
                        return;
                    }

                    _renderTarget = _renderTargetBackBufferUtilities.UpdateCustomRenderTarget(_renderTarget, renderContext, SurfaceFormat.Color, DepthFormat.None, 1);

                    // Blit to the capture target.
                    _graphicsBlit.Blit(renderContext, d, _renderTarget);

#if MANUAL_TEST
#elif RECORDING
                    using (var writer = new StreamWriter("output" + _frame + ".png"))
                    {
                        _renderTarget.SaveAsPng(writer.BaseStream, Width, Height);
                    }
#else
                    var baseStream =
                        typeof (RenderPipelineWorld).Assembly.GetManifestResourceStream(
                            "Protogame.Tests.Expected.RenderPipeline.output" + _frame + ".png");
                    _assert.NotNull(baseStream);
                    var memoryStream = new MemoryStream();
                    _renderTarget.SaveAsPng(memoryStream, Width, Height);
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var expected = new Bitmap(baseStream);
                    var actual = new Bitmap(memoryStream);

                    _assert.Equal(expected.Height, actual.Height);
                    _assert.Equal(expected.Width, actual.Width);
                    var totalPixelValues = 0L;
                    var incorrectPixelValues = 0L;
                    for (var x = 0; x < expected.Width; x++)
                    {
                        for (var y = 0; y < expected.Height; y++)
                        {
                            var expectedPixel = expected.GetPixel(x, y);
                            var actualPixel = actual.GetPixel(x, y);

                            totalPixelValues += 255 * 4;

                            if (expectedPixel != actualPixel)
                            {
                                var diffA = System.Math.Abs((int) actualPixel.A - (int) expectedPixel.A);
                                var diffR = System.Math.Abs((int) actualPixel.R - (int) expectedPixel.R);
                                var diffG = System.Math.Abs((int) actualPixel.G - (int) expectedPixel.G);
                                var diffB = System.Math.Abs((int) actualPixel.B - (int) expectedPixel.B);

                                incorrectPixelValues += (diffA + diffR + diffG + diffB);
                            }
                        }
                    }

                    var percentage = (100 - ((incorrectPixelValues / (double)totalPixelValues) * 100f));
                    
                    var combination = _combinations[_frame % _combinations.Count];
                    _testAttachment.Attach("name-" + combination.Id, combination.Name);
                    _testAttachment.Attach("expected-" + combination.Id, baseStream);
                    _testAttachment.Attach("actual-" + combination.Id, memoryStream);
                    _testAttachment.Attach("threshold-" + combination.Id, 99.9);
                    _testAttachment.Attach("measured-" + combination.Id, percentage);

                    if (percentage <= 99.9f)
                    {
                        combination.FailureMessage = "The actual rendered image did not match the expected image close enough (99.9%).";
                    }

                    //memoryStream.Dispose();
                    //baseStream.Dispose();
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
                _isValidRun = _texture.IsReady;

                if (_texture.State == AssetReferenceState.Unavailable)
                {
                    throw new AggregateException(_texture.LoadingException);
                }

                if (_customPostProcess.Effect.State == AssetReferenceState.Unavailable)
                {
                    throw new AggregateException(_customPostProcess.Effect.LoadingException);
                }

                if (!_texture.IsReady || !_customPostProcess.Effect.IsReady)
                {
                    return;
                }

                renderContext.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Green);

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

                    if (combination.Invert)
                    {
                        renderContext.AppendTransientRenderPass(_invertPostProcess);
                    }

                    if (combination.Blur)
                    {
                        renderContext.AppendTransientRenderPass(_blurPostProcess);
                    }

                    if (combination.MakeRed)
                    {
                        renderContext.AppendTransientRenderPass(_customPostProcess);
                    }

                    renderContext.AppendTransientRenderPass(_captureInlinePostProcess);
                }
            }

            public void Update(IGameContext gameContext, IUpdateContext updateContext)
            {
                if (!_isValidRun)
                {
                    return;
                }

#if !MANUAL_TEST
                if (_didExit)
                {
                    return;
                }

                if (_frame == _combinations.Count)
                {
                    gameContext.Game.Exit();
                    _didExit = true;

                    var failureMessages = string.Empty;
                    foreach (var c in _combinations)
                    {
                        if (c.FailureMessage != null)
                        {
                            failureMessages += c.Name + ": " + c.FailureMessage + "\r\n";
                        }
                    }
                    failureMessages = failureMessages.Trim();

                    if (!string.IsNullOrWhiteSpace(failureMessages))
                    {
                        _assert.True(false, failureMessages);
                    }
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

            public override void PrepareDeviceSettings(GraphicsDeviceInformation deviceInformation)
            {
                base.PrepareDeviceSettings(deviceInformation);

                deviceInformation.PresentationParameters.BackBufferWidth = Width;
                deviceInformation.PresentationParameters.BackBufferHeight = Height;
            }
        }

        public RenderPipelineTests(IAssert assert, IThreadControl threadControl, ITestAttachment testAttachment)
        {
            _assert = assert;
            _testAttachment = testAttachment;

            threadControl.RequireTestsToRunOnMainThread();
        }
        
        public void PerformRenderPipelineTest()
        {
            // We must change directory to the location of the assembly, because
            // Linux doesn't load SDL DLLs properly.
            Environment.CurrentDirectory = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;

            var kernel = new StandardKernel();
            kernel.Load<ProtogameCoreModule>();
            kernel.Load<ProtogameAssetModule>();
            kernel.Bind<IAssert>().ToMethod(x => _assert);
            kernel.Bind<ITestAttachment>().ToMethod(x => _testAttachment);
            kernel.Bind<IRawLaunchArguments>().ToMethod(x => new DefaultRawLaunchArguments(new string[0]));

            try
            {
                using (var game = new HostGame(new RenderPipelineGame(kernel)))
                {
                    game.Run();
                }
            }
            catch (Microsoft.Xna.Framework.Graphics.NoSuitableGraphicsDeviceException)
            {
                System.Console.Error.WriteLine("WARNING: Unable to perform render pipeline tests as no graphics device could be found!");
            }
        }
    }
}
