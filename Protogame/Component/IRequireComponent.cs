namespace Protogame
{
    public interface IRequireComponent<T>
    {
        T Component { get; }
    }
}
