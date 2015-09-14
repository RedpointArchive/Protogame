namespace Protogame
{
    public class DefaultRequireComponent<T> : IRequireComponent<T>, IInternalHasComponent
    {
        public T Component { get { return (T)((IInternalHasComponent) this).Component; } }

        object IInternalHasComponent.Component { get; set; }
    }
}
