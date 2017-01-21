using System;
using Jitter.Dynamics;
using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IPhysicsWorldControl"/>.
    /// </summary>
    /// <module>Physics</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IPhysicsWorldControl</interface_ref>
    public class DefaultPhysicsWorldControl : IPhysicsWorldControl
    {
        private readonly IPhysicsEngine _physicsEngine;

        private Vector3? _pendingGravity;
        private ContactSettings.MaterialCoefficientMixingType? _pendingCoefficientMixingType;
        private Func<RigidBody, RigidBody, bool> _pendingConsiderAngularVelocityCallback;
        private Func<RigidBody, RigidBody, float> _pendingCalculateStaticFrictionCallback;
        private Func<RigidBody, RigidBody, float> _pendingCalculateDynamicFrictionCallback;
        private Func<RigidBody, RigidBody, float> _pendingCalculateRestitutionCallback;

        public DefaultPhysicsWorldControl(IPhysicsEngine physicsEngine)
        {
            _physicsEngine = physicsEngine;

            _pendingGravity = null;
            _pendingCoefficientMixingType = null;
            _pendingConsiderAngularVelocityCallback = null;
            _pendingCalculateStaticFrictionCallback = null;
            _pendingCalculateDynamicFrictionCallback = null;
            _pendingCalculateRestitutionCallback = null;
        }

        public void SyncPendingChanges()
        {
            var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
            if (physicsWorld != null)
            {
                if (_pendingGravity != null)
                {
                    physicsWorld.Gravity = _pendingGravity.Value.ToJitterVector();
                    _pendingGravity = null;
                }

                if (_pendingCoefficientMixingType != null)
                {
                    physicsWorld.ContactSettings.MaterialCoefficientMixing = _pendingCoefficientMixingType.Value;
                    _pendingCoefficientMixingType = null;
                }

                if (_pendingConsiderAngularVelocityCallback != null)
                {
                    physicsWorld.ContactSettings.ConsiderAngularVelocityCallback = _pendingConsiderAngularVelocityCallback;
                    _pendingConsiderAngularVelocityCallback = null;
                }

                if (_pendingCalculateStaticFrictionCallback != null)
                {
                    physicsWorld.ContactSettings.CalculateStaticFrictionCallback = _pendingCalculateStaticFrictionCallback;
                    _pendingCalculateStaticFrictionCallback = null;
                }

                if (_pendingCalculateDynamicFrictionCallback != null)
                {
                    physicsWorld.ContactSettings.CalculateDynamicFrictionCallback = _pendingCalculateDynamicFrictionCallback;
                    _pendingCalculateDynamicFrictionCallback = null;
                }

                if (_pendingCalculateRestitutionCallback != null)
                {
                    physicsWorld.ContactSettings.CalculateRestitutionCallback = _pendingCalculateRestitutionCallback;
                    _pendingCalculateRestitutionCallback = null;
                }
            }
        }

        public Vector3 Gravity
        {
            get
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    return _pendingGravity ?? Vector3.Zero;
                }

                return physicsWorld.Gravity.ToXNAVector();
            }
            set
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    _pendingGravity = value;
                    return;
                }

                physicsWorld.Gravity = value.ToJitterVector();
            }
        }

        public ContactSettings.MaterialCoefficientMixingType MaterialCoefficientMixing
        {
            get
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    return _pendingCoefficientMixingType ?? ContactSettings.MaterialCoefficientMixingType.TakeMinimum;
                }

                return physicsWorld.ContactSettings.MaterialCoefficientMixing;
            }
            set
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    _pendingCoefficientMixingType = value;
                    return;
                }

                physicsWorld.ContactSettings.MaterialCoefficientMixing = value;
            }
        }

        public Func<RigidBody, RigidBody, bool> ConsiderAngularVelocityCallback
        {
            get
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    return _pendingConsiderAngularVelocityCallback;
                }

                return physicsWorld.ContactSettings.ConsiderAngularVelocityCallback;
            }
            set
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    _pendingConsiderAngularVelocityCallback = value;
                    return;
                }

                physicsWorld.ContactSettings.ConsiderAngularVelocityCallback = value;
            }
        }

        public Func<RigidBody, RigidBody, float> CalculateStaticFrictionCallback
        {
            get
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    return _pendingCalculateStaticFrictionCallback;
                }

                return physicsWorld.ContactSettings.CalculateStaticFrictionCallback;
            }
            set
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    _pendingCalculateStaticFrictionCallback = value;
                    return;
                }

                physicsWorld.ContactSettings.CalculateStaticFrictionCallback = value;
            }
        }

        public Func<RigidBody, RigidBody, float> CalculateDynamicFrictionCallback
        {
            get
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    return _pendingCalculateDynamicFrictionCallback;
                }

                return physicsWorld.ContactSettings.CalculateDynamicFrictionCallback;
            }
            set
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    _pendingCalculateDynamicFrictionCallback = value;
                    return;
                }

                physicsWorld.ContactSettings.CalculateDynamicFrictionCallback = value;
            }
        }

        public Func<RigidBody, RigidBody, float> CalculateRestitutionCallback
        {
            get
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    return _pendingCalculateRestitutionCallback;
                }

                return physicsWorld.ContactSettings.CalculateRestitutionCallback;
            }
            set
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld == null)
                {
                    _pendingCalculateRestitutionCallback = value;
                    return;
                }

                physicsWorld.ContactSettings.CalculateRestitutionCallback = value;
            }
        }
    }
}
