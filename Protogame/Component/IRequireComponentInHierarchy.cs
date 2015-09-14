namespace Protogame
{
    public interface IRequireComponentInHierarchy<T>
    {
        T Component { get; }
    }
}
