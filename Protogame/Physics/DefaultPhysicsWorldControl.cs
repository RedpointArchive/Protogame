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
        private Func<RigidBody, RigidBody, bool> _pendingConsiderAngularVelocityCallback;

        public DefaultPhysicsWorldControl(IPhysicsEngine physicsEngine)
        {
            _physicsEngine = physicsEngine;

            _pendingGravity = null;
            _pendingConsiderAngularVelocityCallback = null;
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

        public void SyncPendingChanges()
        {
            if (_pendingGravity != null)
            {
                var physicsWorld = _physicsEngine.GetInternalPhysicsWorld();
                if (physicsWorld != null)
                {
                    physicsWorld.Gravity = _pendingGravity.Value.ToJitterVector();
                    _pendingGravity = null;
                }
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
    }
}
