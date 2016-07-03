using System;
using Jitter;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class FirstPersonControllerPhysicsComponent : IUpdatableComponent, IRenderableComponent, IEnabledComponent
    {
        private readonly IPhysicsEngine _physicsEngine;
        private readonly IDebugRenderer _debugRenderer;
        private readonly IPhysicalComponent _physicalComponent;

        private JitterWorld _jitterWorld;
        private PhysicsControllerConstraint _physicsControllerConstraint;
        private RigidBody _rigidBody;

        public FirstPersonControllerPhysicsComponent(
            IPhysicsEngine physicsEngine,
            IDebugRenderer debugRenderer,
            [FromParent, RequireExisting] IPhysicalComponent physicalComponent)
        {
            _physicsEngine = physicsEngine;
            _debugRenderer = debugRenderer;
            _physicalComponent = physicalComponent;

            TargetVelocity = Vector3.Zero;
            TryJump = false;
            JumpVelocity = 0.5f;
            Stiffness = 0.02f;

            Enabled = true;
        }

        public bool Enabled { get; set; }

        /// <summary>
        /// The target velocity that this controller should attempt to achieve.
        /// </summary>
        public Vector3 TargetVelocity { get; set; }

        /// <summary>
        /// Whether the controller should attempt to jump.
        /// </summary>
        public bool TryJump { get; set; }

        /// <summary>
        /// The rigid body that the controller is currently walking on, or null.
        /// </summary>
        public RigidBody BodyWalkingOn
        {
            get { return _physicsControllerConstraint?.BodyWalkingOn; }
        }

        /// <summary>
        /// The jump velocity to apply when jumping.
        /// </summary>
        public float JumpVelocity { get; set; }

        /// <summary>
        /// The amount of stiffness there is to increasing acceleration as the
        /// controller starts walking.
        /// </summary>
        public float Stiffness { get; set; }

        /// <summary>
        /// Whether the controller is currently touching the floor.
        /// </summary>
        public bool OnFloor
        {
            get { return _physicsControllerConstraint?.OnFloor ?? false; }
        }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            if (!Enabled)
            {
                return;
            }

            if (_jitterWorld != _physicsEngine.GetInternalPhysicsWorld())
            {
                // TODO: Deregister rigid bodies from old world.
                if (_jitterWorld != null && _physicsControllerConstraint != null)
                {
                    _jitterWorld.RemoveConstraint(_physicsControllerConstraint);
                    _physicsControllerConstraint = null;
                }
                _jitterWorld = _physicsEngine.GetInternalPhysicsWorld();
            }

            if (_physicalComponent.RigidBodies.Length > 0 && _physicalComponent.RigidBodies[0] != _rigidBody)
            {
                if (_physicsControllerConstraint != null)
                {
                    _jitterWorld.RemoveConstraint(_physicsControllerConstraint);
                    _physicsControllerConstraint = null;
                }
            }

            if (_physicalComponent.RigidBodies.Length > 0)
            {
                if (_physicsControllerConstraint == null)
                {
                    _physicsControllerConstraint = new PhysicsControllerConstraint(
                        _jitterWorld,
                        _physicalComponent.RigidBodies[0]);
                    _jitterWorld.AddConstraint(_physicsControllerConstraint);
                }

                _physicsControllerConstraint.TargetVelocity = TargetVelocity.ToJitterVector();
                _physicsControllerConstraint.TryJump = TryJump;
                _physicsControllerConstraint.JumpVelocity = JumpVelocity;
                _physicsControllerConstraint.Stiffness = Stiffness;

                if (TargetVelocity.LengthSquared() > 0f)
                {
                    // Wake up the rigid body.
                    _physicalComponent.RigidBodies[0].IsActive = true;
                }
            }
        }

        private class PhysicsControllerConstraint : Constraint
        {
            private JitterWorld _world;
            private float _feetPosition;
            private JVector _deltaVelocity;
            private bool _shouldIJump;

            public PhysicsControllerConstraint(JitterWorld world, RigidBody body) : base(body, null)
            {
                _world = world;

                var vec = JVector.Down;
                var result = JVector.Zero;
                body.Shape.SupportMapping(ref vec, out result);

                _feetPosition = result*JVector.Down;
            }

            public JVector TargetVelocity { get; set; }
            public bool TryJump { get; set; }
            public float JumpVelocity { get; set; }
            public float Stiffness { get; set; }
            public RigidBody BodyWalkingOn { get; private set; }
            public bool OnFloor { get; private set; }

            public override void PrepareForIteration(float timestep)
            {
                RigidBody resultingBody = null;
                JVector normal;
                float frac;

                bool result = _world.CollisionSystem.Raycast(
                    Body1.Position + JVector.Down*(_feetPosition - 0.1f),
                    JVector.Down,
                    PerformRaycastCallback,
                    out resultingBody,
                    out normal,
                    out frac);
                OnFloor = result && frac <= 0.2f;
                BodyWalkingOn = (result && frac <= 0.2f) ? resultingBody : null;
                _shouldIJump = (result && frac <= 0.2f && Body1.LinearVelocity.Y < JumpVelocity && TryJump);
            }

            private bool PerformRaycastCallback(RigidBody rigidBody, JVector normal, float fraction)
            {
                return rigidBody != Body1;
            }
            
            public override void Iterate()
            {
                _deltaVelocity = TargetVelocity - Body1.LinearVelocity;
                _deltaVelocity.Y = 0.0f;

                _deltaVelocity *= Stiffness;

                if (Math.Abs(_deltaVelocity.LengthSquared()) > 0.00001f)
                {
                    Body1.IsActive = true;
                    Body1.ApplyImpulse(_deltaVelocity * Body1.Mass);
                }

                if (_shouldIJump)
                {
                    Body1.IsActive = true;
                    Body1.ApplyImpulse(JumpVelocity * JVector.Up * Body1.Mass);

                    if (!BodyWalkingOn.IsStatic)
                    {
                        BodyWalkingOn.IsActive = true;
                        BodyWalkingOn.ApplyImpulse(-1.0f * JumpVelocity * JVector.Up * Body1.Mass);
                    }
                }
            }
        }

        public void Render(ComponentizedEntity entity, IGameContext gameContext, IRenderContext renderContext)
        {
            if (!Enabled)
            {
                return;
            }

            if (_physicsControllerConstraint != null && _physicalComponent.RigidBodies.Length > 0)
            {
                _debugRenderer.RenderDebugLine(
                    renderContext,
                    _physicalComponent.RigidBodies[0].Position.ToXNAVector(),
                    _physicalComponent.RigidBodies[0].Position.ToXNAVector() + TargetVelocity,
                    Color.Yellow,
                    Color.Yellow);
            }
        }
    }
}
