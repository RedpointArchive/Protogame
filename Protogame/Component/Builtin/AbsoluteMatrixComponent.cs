using Protoinject;

namespace Protogame
{
    public class AbsoluteMatrixComponent : IHasTransform
    {
        private readonly IFinalTransform _finalTransform;

        public AbsoluteMatrixComponent(INode node)
        {
            _finalTransform = new DefaultFinalTransform(this, node);

            Transform = new DefaultTransform();
        }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform => _finalTransform;
    }
}