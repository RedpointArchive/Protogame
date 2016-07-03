using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Protoinject;

namespace Protogame
{
    public abstract class PhysicalBaseRigidBodyComponent : IUpdatableComponent, IPhysicalComponent, IServerUpdatableComponent, IEnabledComponent, ISynchronisedObject
    {
        private readonly INode _node;
        private readonly IPhysicsEngine _physicsEngine;

        private Shape _shape;

        private RigidBody _rigidBody;

        private bool _addedRigidBody;

        private bool _hasSetRotationOnRigidBody;

        internal PhysicalBaseRigidBodyComponent(INode node, IPhysicsEngine physicsEngine)
        {
            _node = node;
            _physicsEngine = physicsEngine;
            _addedRigidBody = false;

            Enabled = true;
        }

        public bool Enabled { get; set; }

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

        protected abstract Shape GetShape(ITransform localTransform);

        protected void UpdateRigidBodyShape(ITransform localTransform)
        {
            _shape = GetShape(localTransform);
            if (_rigidBody == null)
            {
                _rigidBody = new RigidBody(_shape);
            }
            else
            {
                _rigidBody.Shape = _shape;
            }
            _rigidBody.Tag = this;
        }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            Update();
        }

        public RigidBody[] RigidBodies
        {
            get
            {
                if (Enabled)
                {
                    return new[] {_rigidBody};
                }
                else
                {
                    return new RigidBody[0];
                }
            }
        }

        public void Update(ComponentizedEntity entity, IServerContext serverContext, IUpdateContext updateContext)
        {
            Update();
        }

        private void Update()
        {
            if (!Enabled)
            {
                return;
            }
            
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

        public virtual void DeclareSynchronisedProperties(ISynchronisationApi synchronisationApi)
        {
            synchronisationApi.Synchronise("static", 60, Static, x => Static = x, null);
            synchronisationApi.Synchronise("mass", 60, Mass, x => Mass = x <= 0 ? 1 : x, null);
            synchronisationApi.Synchronise("active", 60, _rigidBody.IsActive, x => _rigidBody.IsActive = x, 500);
            synchronisationApi.Synchronise("linvel", 60, _rigidBody.LinearVelocity.ToXNAVector(), x =>
            {
                if (!Static)
                {
                    _rigidBody.LinearVelocity = x.ToJitterVector();
                }
            }, 500);
            synchronisationApi.Synchronise("angvel", 60, _rigidBody.AngularVelocity.ToXNAVector(), x =>
            {
                if (!Static)
                {
                    _rigidBody.AngularVelocity = x.ToJitterVector();
                }
            }, 500);
        }
    }
}
