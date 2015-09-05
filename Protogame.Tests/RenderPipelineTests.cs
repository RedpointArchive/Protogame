using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

            private bool _invertEnabled;

            private bool _blurEnabled;

            public RenderPipelineWorld(IAssetManagerProvider assetManagerProvider, I2DRenderUtilities renderUtilities, IGraphicsFactory graphicsFactory)
            {
                _renderUtilities = renderUtilities;
                _texture = assetManagerProvider.GetAssetManager().Get<TextureAsset>("texture.Player");
                _invertPostProcess = graphicsFactory.CreateInvertPostProcessingRenderPass();
                _blurPostProcess = graphicsFactory.CreateBlurPostProcessingRenderPass();

                this.Entities = new List<IEntity>();
            }

            public IList<IEntity> Entities { get; private set; }

            public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
            {
            }

            public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
            {
                renderContext.GraphicsDevice.Clear(Color.Green);

                if (renderContext.IsCurrentRenderPass<I2DBatchedRenderPass>())
                {
                    _renderUtilities.RenderTexture(renderContext, Vector2.Zero, _texture);

                    if (_invertEnabled)
                    {
                        renderContext.AppendRenderPass(_invertPostProcess);
                    }

                    if (_blurEnabled)
                    {
                        renderContext.AppendRenderPass(_blurPostProcess);
                    }
                }
            }

            public void Update(IGameContext gameContext, IUpdateContext updateContext)
            {
                if (Keyboard.GetState().IsKeyChanged(this, Keys.Space) == KeyState.Up)
                {
                    _invertEnabled = !_invertEnabled;
                }

                if (Keyboard.GetState().IsKeyChanged(this, Keys.B) == KeyState.Up)
                {
                    _blurEnabled = !_blurEnabled;
                }
            }

            public void Dispose()
            {
            }
        }

        public class RenderPipelineGame : CoreGame<RenderPipelineWorld>
        {
            public RenderPipelineGame(IKernel kernel) : base(kernel)
            {
            }

            protected override void ConfigureRenderPipeline(IRenderPipeline pipeline, IKernel kernel)
            {
                var factory = kernel.Get<IGraphicsFactory>();

                //pipeline.AddRenderPass(factory.Create3DRenderPass());
                pipeline.AddRenderPass(factory.Create2DBatchedRenderPass());
                //pipeline.AddRenderPass();
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
