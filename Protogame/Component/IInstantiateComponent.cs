namespace Protogame
{
    public interface IInstantiateComponent<T>
    {
        T Component { get; }
    }
}
