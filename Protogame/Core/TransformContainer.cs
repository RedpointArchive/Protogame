namespace Protogame
{
    public class TransformContainer : IHasTransform
    {
        public TransformContainer(ITransform transform)
        {
            Transform = transform;
        }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform => this.GetDetachedFinalTransformImplementation();
    }
}
