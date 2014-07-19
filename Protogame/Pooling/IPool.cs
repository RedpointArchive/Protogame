namespace Protogame
{
    public interface IPool<T> : IRawPool
        where T : class
    {
        T Get();

        void Release(T instance);

        void ReleaseAll();
    }

    public interface IRawPool
    {
        string Name { get; }

        int NextAvailable { get; }

        int NextReturn { get; }

        int Free { get; }

        int Total { get; }
    }
}