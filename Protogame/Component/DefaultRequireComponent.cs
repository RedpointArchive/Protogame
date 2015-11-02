namespace Protogame
{
    public class DefaultRequireComponent<T> : IRequireComponent<T>, IInternalHasComponent, IRequireComponentInDescendants<T>, IRequireComponentInHierarchy<T>
    {
        public T Component { get { return (T)((IInternalHasComponent) this).Component; } }

        object IInternalHasComponent.Component { get; set; }
    }
}
