using System;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Protoinject;

namespace Protogame
{
    public abstract class PhysicalBaseRigidBodyComponent : IUpdatableComponent, IPhysicalComponent, IServerUpdatableComponent, IEnabledComponent, ISynchronisedObject, IHasTransform
    {
        private readonly INode _node;
        private readonly IFinalTransform _finalTransform;
        private readonly IPhysicsEngine _physicsEngine;

        private Shape _shape;

        private RigidBody _rigidBody;

        private bool _addedRigidBody;

        private bool _didUpdateSync;

        private bool _enabled;

        private bool _pureCollider;

        private PhysicsTarget _target;
        private BatchedControlEntity _batchedControlEntity;
        private bool _static;

        protected PhysicalBaseRigidBodyComponent(INode node, IPhysicsEngine physicsEngine)
        {
            _node = node;
            _finalTransform = new DefaultFinalTransform(this, node);
            _physicsEngine = physicsEngine;
            _addedRigidBody = false;

            Enabled = true;
            Transform = new DefaultTransform();
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

        /// <summary>
        /// By setting this flag, the physics engine won't move this rigid body, and this rigid
        /// body won't exert forces on any other rigid body, but collisions will still be detected
        /// and raised as events on the parent object in the hierarchy.
        /// </summary>
        public bool PureCollider
        {
            get { return _pureCollider; }
            set
            {
                if (_pureCollider != value)
                {
                    _pureCollider = value;
                    UpdatePureCollider();
                }
            }
        }

        private void UpdatePureCollider()
        {
            if (_pureCollider)
            {
                _rigidBody.PureCollider = true;
                _rigidBody.AffectedByGravity = false;
            }
            else
            {
                _rigidBody.PureCollider = false;
                _rigidBody.AffectedByGravity = true;
            }
        }

        public bool Static
        {
            get { return _static; }
            set
            {
                _static = value;
                if (_rigidBody != null)
                {
                    _rigidBody.IsStatic = value;
                }
            }
        }

        public float Mass
        {
            get { return _rigidBody.Mass; }
            set { _rigidBody.Mass = value; }
        }

        public PhysicsTarget Target
        {
            get { return _target; }
            set
            {
                if (_target != value)
                {
                    _target = value;

                    var transformComponent = _node.Parent?.UntypedValue as IHasTransform;
                    if (transformComponent != null)
                    {
                        if (_addedRigidBody)
                        {
                            IHasTransform targetTransform;
                            switch (_target)
                            {
                                case PhysicsTarget.ComponentOwner:
                                    targetTransform = transformComponent;
                                    break;
                                case PhysicsTarget.Component:
                                    targetTransform = this;
                                    break;
                                default:
                                    throw new InvalidOperationException();
                            }

                            _physicsEngine.UnregisterRigidBodyForHasMatrixInCurrentWorld(_rigidBody, targetTransform);
                            _addedRigidBody = false;
                        }
                    }
                }
            }
        }

        protected abstract Shape GetShape(ITransform localTransform);

        private void UpdateRigidBodyShape(ITransform localTransform)
        {
            _shape = GetShape(localTransform);
            if (_rigidBody == null)
            {
                _rigidBody = new RigidBody(_shape);
                _rigidBody.IsStatic = _static;
            }
            else
            {
                _rigidBody.Shape = _shape;
            }
            _rigidBody.WeakTag = new WeakReference<object>(this);
        }

        protected void UpdateRigidBodyShape()
        {
            UpdateRigidBodyShape(Transform);
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
            if (_didUpdateSync || _batchedControlEntity != null)
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
                        IHasTransform targetTransform;
                        switch (_target)
                        {
                            case PhysicsTarget.ComponentOwner:
                                targetTransform = transformComponent;
                                break;
                            case PhysicsTarget.Component:
                                targetTransform = this;
                                break;
                            default:
                                throw new InvalidOperationException();
                        }

                        _physicsEngine.UnregisterRigidBodyForHasMatrixInCurrentWorld(_rigidBody, targetTransform);
                        _addedRigidBody = false;
                    }
                }
                else
                {
                    if (!_addedRigidBody)
                    {
                        // We use the transform local to this component for scaling purposes.
                        UpdateRigidBodyShape(Transform);

                        IHasTransform targetTransform;
                        switch (_target)
                        {
                            case PhysicsTarget.ComponentOwner:
                                targetTransform = transformComponent;
                                break;
                            case PhysicsTarget.Component:
                                targetTransform = this;
                                break;
                            default:
                                throw new InvalidOperationException();
                        }

                        if (StaticAndImmovable && !Static)
                        {
                            throw new InvalidOperationException("Physics component is marked StaticAndImmovable, but Static is not true.");
                        }

                        _physicsEngine.RegisterRigidBodyForHasMatrixInCurrentWorld(_rigidBody, targetTransform, StaticAndImmovable);
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

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform => _finalTransform;

        public void SetBatchedEntity(BatchedControlEntity entity)
        {
            if (!StaticAndImmovable)
            {
                throw new InvalidOperationException(
                    "Attempted to set associated batched control entity for physics component that is not static and immovable!");
            }

            _batchedControlEntity = entity;
            
            IHasTransform targetTransform;
            switch (_target)
            {
                case PhysicsTarget.ComponentOwner:
                    targetTransform = _node.Parent?.UntypedValue as IHasTransform;
                    break;
                case PhysicsTarget.Component:
                    targetTransform = this;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            _physicsEngine.UnregisterRigidBodyForHasMatrixInCurrentWorld(_rigidBody, targetTransform);
            _addedRigidBody = false;
        }
    }
}
