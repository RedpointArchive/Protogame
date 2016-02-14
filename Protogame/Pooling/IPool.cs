namespace Protogame
{
    /// <summary>
    /// A memory pool of objects.
    /// </summary>
    /// <module>Pooling</module>
    public interface IPool<T> : IRawPool
        where T : class
    {
        T Get();

        void Release(T instance);

        void ReleaseAll();
    }
}