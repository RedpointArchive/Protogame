using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Protoinject;

namespace Protogame
{
    public class Physical3DCapsuleComponent : IUpdatableComponent, IPhysicalComponent, IServerUpdatableComponent
    {
        private readonly INode _node;
        private readonly IPhysicsEngine _physicsEngine;

        private CapsuleShape _capsuleShape;

        private RigidBody _rigidBody;

        private bool _addedRigidBody;

        private bool _hasSetRotationOnRigidBody;

        private float _radius;

        private float _length;

        public Physical3DCapsuleComponent(INode node, IPhysicsEngine physicsEngine)
        {
            _node = node;
            _physicsEngine = physicsEngine;
            _addedRigidBody = false;
            _radius = 1f;
            _length = 1f;
            UpdateRigidBodyShape();
        }

        public bool Static
        {
            get { return _rigidBody.IsStatic; }
            set { _rigidBody.IsStatic = value; }
        }

        public float Mass
        {
            get { return _rigidBody.Mass; }
            set { _rigidBody.Mass = value; }
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

        private void UpdateRigidBodyShape()
        {
            _capsuleShape = new CapsuleShape(
                _length,
                _radius);
            if (_rigidBody == null)
            {
                _rigidBody = new RigidBody(_capsuleShape);
            }
            else
            {
                _rigidBody.Shape = _capsuleShape;
            }
        }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            Update();
        }

        public RigidBody[] RigidBodies
        {
            get { return new[] {_rigidBody}; }
        }

        public void Update(ComponentizedEntity entity, IServerContext serverContext, IUpdateContext updateContext)
        {
            Update();
        }

        private void Update()
        {
            // Update the parent node's matrix based on the rigid body's state.
            var transformComponent = _node.Parent?.UntypedValue as IHasTransform;
            if (transformComponent != null)
            {
                if (!_addedRigidBody)
                {
                    UpdateRigidBodyShape();

                    _physicsEngine.RegisterRigidBodyForHasMatrixInCurrentWorld(_rigidBody, transformComponent);
                    _addedRigidBody = true;
                }
            }
        }
    }
}
