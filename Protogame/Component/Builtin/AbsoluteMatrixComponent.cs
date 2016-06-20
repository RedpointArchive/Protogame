namespace Protogame
{
    public class AbsoluteMatrixComponent : IHasTransform
    {
        public ITransform Transform { get; set; }

        public IFinalTransform FinalTransform
        {
            get { return this.GetDetachedFinalTransformImplementation(); }
        }
    }
}