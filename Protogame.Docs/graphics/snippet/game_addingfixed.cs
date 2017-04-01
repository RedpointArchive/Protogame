public class AddingFixedRenderPassExampleGame : CoreGame<RenderPipelineWorld>
{
    public AddingFixedRenderPassExampleGame(IKernel kernel) : base(kernel)
    {
    }

    protected override void ConfigureRenderPipeline(IRenderPipeline pipeline, IKernel kernel)
    {
        var factory = kernel.Get<IGraphicsFactory>();
        
        // Add a 3D render pass in which we render the main game world.
        pipeline.AddFixedRenderPass(factory.Create3DRenderPass());
        
        // Add a 2D batched render pass in which we render the UI.
        pipeline.AddFixedRenderPass(factory.Create2DBatchedRenderPass());
    }
}