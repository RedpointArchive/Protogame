namespace Protogame
{
    /// <summary>
    /// Provides depth values for SpriteBatch and other interlaced draw calls such that
    /// you can use SpriteBatch in deferred mode, draw non-SpriteBatch'd vertexes in the
    /// middle, and still have the correct result.
    /// </summary>
    /// <module>Graphics</module>
    public interface IInterlacedBatchingDepthProvider
    {
        void Reset();

        float GetDepthForCurrentInstance();
    }
}
