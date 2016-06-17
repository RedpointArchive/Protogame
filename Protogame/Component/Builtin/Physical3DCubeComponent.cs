using System;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Protoinject;

namespace Protogame
{
    public class Physical3DCubeComponent : IUpdatableComponent, IPhysicalComponent, IServerUpdatableComponent
    {
        private readonly INode _node;
        private readonly IPhysicsEngine _physicsEngine;

        private BoxShape _boxShape;

        private RigidBody _rigidBody;

        private bool _addedRigidBody;

        private bool _hasSetRotationOnRigidBody;

        public Physical3DCubeComponent(INode node, IPhysicsEngine physicsEngine)
        {
            _node = node;
            _physicsEngine = physicsEngine;
            _addedRigidBody = false;
            UpdateRigidBodyShape(new DefaultTransform());
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

        private void UpdateRigidBodyShape(ITransform localTransform)
        {
            _boxShape = new BoxShape(localTransform.LocalScale.ToJitterVector());
            if (_rigidBody == null)
            {
                _rigidBody = new RigidBody(_boxShape);
            }
            else
            {
                _rigidBody.Shape = _boxShape;
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
                    UpdateRigidBodyShape(transformComponent.Transform);

                    _physicsEngine.RegisterRigidBodyForHasMatrixInCurrentWorld(_rigidBody, transformComponent);
                    _addedRigidBody = true;
                }
            }
        }
    }
}
