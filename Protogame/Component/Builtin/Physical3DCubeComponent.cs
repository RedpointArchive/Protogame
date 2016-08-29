using Jitter.Collision.Shapes;
using Protoinject;

namespace Protogame
{
    public class Physical3DCubeComponent : PhysicalBaseRigidBodyComponent
    {
        public Physical3DCubeComponent(INode node, IPhysicsEngine physicsEngine) : base(node, physicsEngine)
        {
            Transform = new DefaultTransform();
            Transform.Modified += (sender, args) =>
            {
                UpdateRigidBodyShape(Transform);
            };
            UpdateRigidBodyShape(Transform);

            Enabled = true;
        }
        
        protected override Shape GetShape(ITransform localTransform)
        {
            return new BoxShape(localTransform.LocalScale.ToJitterVector());
        }

        public ITransform Transform { get; set; }
    }
}
