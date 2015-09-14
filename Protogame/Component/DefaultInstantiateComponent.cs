namespace Protogame
{
    public class DefaultInstantiateComponent<T> : IInstantiateComponent<T>, IInternalHasComponent
    {
        public T Component { get { return (T)((IInternalHasComponent) this).Component; } }

        object IInternalHasComponent.Component { get; set; }
    }
}
