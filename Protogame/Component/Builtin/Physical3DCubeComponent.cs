using Jitter.Collision.Shapes;
using Protoinject;

namespace Protogame
{
    public class Physical3DCubeComponent : PhysicalBaseRigidBodyComponent
    {
        public Physical3DCubeComponent(INode node, IPhysicsEngine physicsEngine) : base(node, physicsEngine)
        {
            UpdateRigidBodyShape(new DefaultTransform());

            Enabled = true;
        }
        
        protected override Shape GetShape(ITransform localTransform)
        {
            return new BoxShape(localTransform.LocalScale.ToJitterVector());
        }
    }
}
