using System;
using Jitter.Collision.Shapes;
using Protoinject;

namespace Protogame
{
    public class Physical3DCapsuleComponent : PhysicalBaseRigidBodyComponent
    {
        private float _radius;
        private float _length;
        
        public Physical3DCapsuleComponent(INode node, IPhysicsEngine physicsEngine) : base(node, physicsEngine)
        {
            _radius = 1f;
            _length = 1f;
            UpdateRigidBodyShape();

            Enabled = true;
        }
        
        public float Radius
        {
            get { return _radius; }
            set { _radius = value; UpdateRigidBodyShape(); }
        }

        public float Length
        {
            get { return _length; }
            set { _length = value; UpdateRigidBodyShape(); }
        }

        protected override Shape GetShape(ITransform localTransform)
        {
            if (_length <= 0 || _radius <= 0)
            {
                throw new InvalidOperationException("Invalid length or radius.");
            }

            return new CapsuleShape(_length, _radius);
        }

        public override void DeclareSynchronisedProperties(ISynchronisationApi synchronisationApi)
        {
            base.DeclareSynchronisedProperties(synchronisationApi);
            
            synchronisationApi.Synchronise("radius", 60, Radius, x => Radius = x, null);
            synchronisationApi.Synchronise("length", 60, Length, x => Length = x, null);
        }
    }
}
