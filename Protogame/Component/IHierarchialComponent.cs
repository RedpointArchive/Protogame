namespace Protogame
{
    public interface IHierarchialComponent : IContainsComponents
    {
        object Parent { get; }
    }
}
