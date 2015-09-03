namespace Protogame
{
    /// <summary>
    /// The factory interface which is used to create render passes
    /// before they are added to the render pipeline.
    /// <para>
    /// Use these methods to construct render passes with the appropriate
    /// settings, and pass the resulting value into <see cref="IRenderPass.AddRenderPass"/>
    /// or <see cref="IRenderPass.AppendRenderPass"/>.
    /// </para>
    /// </summary>
    /// <module>Graphics</module>
    public interface IGraphicsFactory
    {
        I2DRenderPass Create2DRenderPass();
    }
}

