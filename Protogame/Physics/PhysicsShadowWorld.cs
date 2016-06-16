using System.Collections.Generic;
using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class PhysicsShadowWorld
    {
        private readonly IPhysicsEngine _physicsEngine;
        private readonly IDebugRenderer _debugRenderer;

        private readonly CollisionSystemPersistentSAP _collisionSystem;

        private readonly JitterWorld _physicsWorld;

        private readonly IWorld _gameSystemWorld;

        private readonly List<KeyValuePair<RigidBody, IHasTransform>> _rigidBodyMappings;

        private Dictionary<int, Quaternion> _lastFrameRotation;

        private Dictionary<int, Vector3> _lastFramePosition;

        public PhysicsShadowWorld(IPhysicsEngine physicsEngine, IDebugRenderer debugRenderer, IWorld world)
        {
            _gameSystemWorld = world;

            _physicsEngine = physicsEngine;
            _debugRenderer = debugRenderer;
            _collisionSystem = new CollisionSystemPersistentSAP
            {
                EnableSpeculativeContacts = true
            };

            _physicsWorld = new JitterWorld(_collisionSystem);
            _physicsWorld.ContactSettings.MaterialCoefficientMixing =
                ContactSettings.MaterialCoefficientMixingType.TakeMinimum;

            _physicsWorld.Gravity = new JVector(0, -4f, 0);

            _rigidBodyMappings = new List<KeyValuePair<RigidBody, IHasTransform>>();

            _lastFramePosition = new Dictionary<int, Vector3>();
            _lastFrameRotation = new Dictionary<int, Quaternion>();
        }
        
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var originalRotation = new Dictionary<int, Quaternion>();
            var originalPosition = new Dictionary<int, Vector3>();

            foreach (var kv in _rigidBodyMappings)
            {
                var rigidBody = kv.Key;
                var hasMatrix = kv.Value;

                // Sync game world to physics system.
                var rot = hasMatrix.FinalTransform.AbsoluteRotation;
                var pos = hasMatrix.FinalTransform.AbsolutePosition;
                originalRotation[rigidBody.GetHashCode()] = rot;
                originalPosition[rigidBody.GetHashCode()] = pos;

                // If the position of the entity differs from what we expect, then the user
                // probably explicitly set it and we need to sync the rigid body.
                if (_lastFramePosition.ContainsKey(rigidBody.GetHashCode()))
                {
                    var lastPosition = _lastFramePosition[rigidBody.GetHashCode()];
                    if ((lastPosition - hasMatrix.Transform.LocalPosition).LengthSquared() > 0.0001f)
                    {
                        rigidBody.Position = pos.ToJitterVector();
                    }
                }
                else
                {
                    rigidBody.Position = pos.ToJitterVector();
                }

                // If the rotation of the entity differs from what we expect, then the user
                // probably explicitly set it and we need to sync the rigid body.
                if (_lastFrameRotation.ContainsKey(rigidBody.GetHashCode()))
                {
                    var lastRotation = _lastFrameRotation[rigidBody.GetHashCode()];

                    var a = Quaternion.Normalize(lastRotation);
                    var b = Quaternion.Normalize(hasMatrix.Transform.LocalRotation);
                    var closeness = 1 - (a.X*b.X + a.Y*b.Y + a.Z*b.Z + a.W*b.W);

                    if (closeness > 0.0001f)
                    {
                       rigidBody.Orientation = JMatrix.CreateFromQuaternion(rot.ToJitterQuaternion());
                    }
                }
                else
                {
                    rigidBody.Orientation = JMatrix.CreateFromQuaternion(rot.ToJitterQuaternion());
                }
            }

            _lastFramePosition.Clear();
            _lastFrameRotation.Clear();

            _physicsWorld.Step((float)gameContext.GameTime.ElapsedGameTime.TotalSeconds, true);

            foreach (var kv in _rigidBodyMappings)
            {
                var rigidBody = kv.Key;
                var hasMatrix = kv.Value;

                // Calculate the changes that the physics system made in world space.
                var oldWorldRot = Quaternion.Normalize(originalRotation[rigidBody.GetHashCode()]);
                var oldWorldPos = originalPosition[rigidBody.GetHashCode()];
                var newWorldRot = Quaternion.Normalize(JQuaternion.CreateFromMatrix(rigidBody.Orientation).ToXNAQuaternion());
                var newWorldPos = rigidBody.Position.ToXNAVector();
                
                // Determine the localised differences in position.
                var localPos = newWorldPos - oldWorldPos;
                
                // Update the local components of the transform.
                hasMatrix.Transform.LocalPosition += localPos;
                hasMatrix.Transform.LocalRotation *= Quaternion.Inverse(oldWorldRot)*newWorldRot;

                // Save the current rotation / position for the next frame.
                _lastFramePosition[rigidBody.GetHashCode()] = hasMatrix.Transform.LocalPosition;
                _lastFrameRotation[rigidBody.GetHashCode()] = hasMatrix.Transform.LocalRotation;
            }
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.IsCurrentRenderPass<IDebugRenderPass>())
            {
                var world = renderContext.World;
                renderContext.World = Matrix.Identity;
                
                foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    foreach (var kv in _rigidBodyMappings)
                    {
                        if (!kv.Key.EnableDebugDraw)
                        {
                            kv.Key.EnableDebugDraw = true;
                        }

                        var drawer = new PhysicsDebugDraw(renderContext, _debugRenderer, !kv.Key.IsStaticOrInactive);
                        kv.Key.DebugDraw(drawer);
                    }
                }

                renderContext.World = world;
            }
        }

        public void Dispose()
        {
            
        }

        public void RegisterRigidBodyForHasMatrix(RigidBody rigidBody, IHasTransform hasTransform)
        {
            _rigidBodyMappings.Add(new KeyValuePair<RigidBody, IHasTransform>(rigidBody, hasTransform));
            _physicsWorld.AddBody(rigidBody);
        }

        public JitterWorld GetJitterWorld()
        {
            return _physicsWorld;
        }
    }
}