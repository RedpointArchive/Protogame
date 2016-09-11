// ReSharper disable CheckNamespace

using Jitter.Collision.Shapes;
using Protoinject;

namespace Protogame
{
    public class PhysicalBatchedStaticCompoundComponent : PhysicalBaseRigidBodyComponent
    {
        private readonly CompoundShape _shape;

        public PhysicalBatchedStaticCompoundComponent(INode node, IPhysicsEngine physicsEngine, CompoundShape shape) : base(node, physicsEngine)
        {
            _shape = shape;
        }

        protected override Shape GetShape(ITransform localTransform)
        {
            return _shape;
        }
    }
}
