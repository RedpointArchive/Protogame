public class FixedRenderPassEntity : Entity
{
    private readonly IRenderPass _renderPass;
    
    private bool _someOnceOffField;
    
    public FixedRenderPassEntity(IGraphicsFactory graphicsFactory)
    {
        _renderPass = graphicsFactory.Create3DRenderPass();
        _someOnceOffField = true;
    }

    public override void Render(IGameContext gameContext, IRenderContext renderContext)
    {
        if (renderContext.IsFirstRenderPass() && _someOnceOffField)
        {
            // You only want to call this method once, since fixed render passes are
            // permanently in the pipeline until RemoveFixedRenderPass is called.  If
            // this block of code were allowed to execute every frame, the render
            // pipeline would become infinitely long.
            renderContext.AddFixedRenderPass(_renderPass);
            _someOnceOffField = false;
        }
    }
}