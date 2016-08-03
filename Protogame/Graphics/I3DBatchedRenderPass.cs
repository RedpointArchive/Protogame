namespace Protogame
{
    /// <summary>
    /// Indicates a 3D render pass in which calls to render data should go
    /// through the <see cref="IRenderBatcher"/> service for optimal
    /// rendering.
    /// </summary>
    /// <module>Graphics</module>
    public interface I3DBatchedRenderPass : I3DRenderPass
    {
    }
}
