namespace Protogame
{
    public class AbsoluteMatrixComponent : IHasTransform
    {
        public AbsoluteMatrixComponent()
        {
            Transform = new DefaultTransform();
        }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform
        {
            get { return this.GetDetachedFinalTransformImplementation(); }
        }
    }
}