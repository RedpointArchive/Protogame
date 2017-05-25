namespace Protogame
{
    public class TransformContainer : IHasTransform
    {
        private readonly IFinalTransform _finalTransform;

        public TransformContainer(ITransform transform)
        {
            _finalTransform = new DefaultFinalTransform(this, null);
            Transform = transform;
        }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform => _finalTransform;
    }
}
