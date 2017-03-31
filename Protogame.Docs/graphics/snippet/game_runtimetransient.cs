public class TransientRenderPassEntity : Entity
{
    private readonly IRenderPass _renderPass;
    
    public TransientRenderPassEntity(IGraphicsFactory graphicsFactory)
    {
        _renderPass = graphicsFactory.CreateBlurPostProcessingRenderPass();
    }

    public override void Render(IGameContext gameContext, IRenderContext renderContext)
    {
        if (renderContext.IsFirstRenderPass())
        {
            renderContext.AppendTransientRenderPass(_renderPass);
        }
    }
}