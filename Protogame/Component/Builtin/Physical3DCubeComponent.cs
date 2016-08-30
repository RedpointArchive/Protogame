using Jitter.Collision.Shapes;
using Protoinject;

namespace Protogame
{
    public class Physical3DCubeComponent : PhysicalBaseRigidBodyComponent
    {
        public Physical3DCubeComponent(INode node, IPhysicsEngine physicsEngine) : base(node, physicsEngine)
        {
            Transform.Modified += (sender, args) =>
            {
                UpdateRigidBodyShape();
            };
            UpdateRigidBodyShape();

            Enabled = true;
        }
        
        protected override Shape GetShape(ITransform localTransform)
        {
            return new BoxShape(localTransform.LocalScale.ToJitterVector());
        }
    }
}
