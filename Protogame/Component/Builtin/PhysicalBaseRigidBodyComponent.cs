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

        private bool _didUpdateSync;
        private bool _enabled;

        internal PhysicalBaseRigidBodyComponent(INode node, IPhysicsEngine physicsEngine)
        {
            _node = node;
            _physicsEngine = physicsEngine;
            _addedRigidBody = false;

            Enabled = true;
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    _didUpdateSync = false;
                }
            }
        }

        /// <summary>
        /// By setting this flag, the physics engine won't synchronise this physics component
        /// beyond on the initial synchronisation.  If you have static geometry in your scene
        /// which is flagged as <see cref="Static"/> and which you know will not move, you should
        /// set this option to true to optimize game performance.
        /// </summary>
        public bool StaticAndImmovable { get; set; }

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
            if (_didUpdateSync)
            {
                return;
            }

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
            if (_didUpdateSync)
            {
                return;
            }

            Update();
        }

        private void Update()
        {
            if (_didUpdateSync)
            {
                return;
            }

            // Update the parent node's matrix based on the rigid body's state.
            var transformComponent = _node.Parent?.UntypedValue as IHasTransform;
            if (transformComponent != null)
            {
                if (!Enabled)
                {
                    if (_addedRigidBody)
                    {
                        _physicsEngine.UnregisterRigidBodyForHasMatrixInCurrentWorld(_rigidBody, transformComponent);
                        _addedRigidBody = false;
                    }
                }
                else
                {
                    if (!_addedRigidBody)
                    {
                        UpdateRigidBodyShape(transformComponent.Transform);

                        _physicsEngine.RegisterRigidBodyForHasMatrixInCurrentWorld(_rigidBody, transformComponent, StaticAndImmovable);
                        _addedRigidBody = true;
                    }
                }
            }

            _didUpdateSync = true;
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
