using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class PhysicsShadowWorld : IDisposable
    {
        private readonly IPhysicsEngine _physicsEngine;
        private readonly IDebugRenderer _debugRenderer;

        private readonly CollisionSystemPersistentSAP _collisionSystem;

        private readonly JitterWorld _physicsWorld;

        private readonly List<RigidBodyMapping> _rigidBodyMappings;

        private Dictionary<int, Quaternion> _lastFrameRotation;

        private Dictionary<int, Vector3> _lastFramePosition;

        private PhysicsMetrics _physicsMetrics = new PhysicsMetrics();

        private Stopwatch _stopwatch = new Stopwatch();

        private Thread _physicsThread;

        public PhysicsShadowWorld(IPhysicsEngine physicsEngine, IDebugRenderer debugRenderer)
        {
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

            _rigidBodyMappings = new List<RigidBodyMapping>();

            _lastFramePosition = new Dictionary<int, Vector3>();
            _lastFrameRotation = new Dictionary<int, Quaternion>();
        }

        private class RigidBodyMapping
        {
            public RigidBodyMapping(RigidBody rigidBody, IHasTransform hasTransform, bool staticAndImmovable)
            {
                RigidBody = rigidBody;
                HasTransform = hasTransform;
                StaticAndImmovable = staticAndImmovable;
                PerformedInitialSync = false;
            }

            public RigidBody RigidBody { get; set; }

            public IHasTransform HasTransform { get; set; }

            public bool StaticAndImmovable { get; set; }

            public bool PerformedInitialSync { get; set; }

            public override string ToString()
            {
                return StaticAndImmovable ? "(static and immovable)" : "(dynamic / static)";
            }
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            Update((float)serverContext.GameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            Update((float) gameContext.GameTime.ElapsedGameTime.TotalSeconds);
        }

        private void Update(float totalSecondsElapsed)
        {
            var originalRotation = new Dictionary<int, Quaternion>();
            var originalPosition = new Dictionary<int, Vector3>();

            _stopwatch.Start();
            _physicsMetrics.StaticImmovableObjects = 0;
            _physicsMetrics.PhysicsObjects = 0;

            foreach (var kv in _rigidBodyMappings)
            {
                if (kv.StaticAndImmovable)
                {
                    _physicsMetrics.StaticImmovableObjects++;
                }
                else
                {
                    _physicsMetrics.PhysicsObjects++;
                }

                if (kv.StaticAndImmovable && kv.PerformedInitialSync)
                {
                    continue;
                }

                var rigidBody = kv.RigidBody;
                var hasMatrix = kv.HasTransform;

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

            _stopwatch.Stop();
            _physicsMetrics.SyncToPhysicsTime = _stopwatch.Elapsed.TotalMilliseconds;
            _stopwatch.Restart();

            _physicsWorld.Step(totalSecondsElapsed, true);

            _stopwatch.Stop();
            _physicsMetrics.PhysicsStepTime = _stopwatch.Elapsed.TotalMilliseconds;
            _stopwatch.Restart();

            foreach (var kv in _rigidBodyMappings)
            {
                if (kv.StaticAndImmovable && kv.PerformedInitialSync)
                {
                    continue;
                }

                var rigidBody = kv.RigidBody;
                var hasMatrix = kv.HasTransform;

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

                if (kv.StaticAndImmovable && !kv.PerformedInitialSync)
                {
                    kv.PerformedInitialSync = true;
                }
            }

            _stopwatch.Stop();
            _physicsMetrics.SyncFromPhysicsTime = _stopwatch.Elapsed.TotalMilliseconds;
            _stopwatch.Reset();
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.IsCurrentRenderPass<IDebugRenderPass>())
            {
                var debugRenderPass = renderContext.GetCurrentRenderPass<IDebugRenderPass>();

                if (debugRenderPass.EnabledLayers.OfType<PhysicsDebugLayer>().Any())
                {
                    foreach (var kv in _rigidBodyMappings)
                    {
                        if (!kv.RigidBody.EnableDebugDraw)
                        {
                            kv.RigidBody.EnableDebugDraw = true;
                        }

                        var drawer = new PhysicsDebugDraw(renderContext, _debugRenderer, !kv.RigidBody.IsStaticOrInactive);
                        kv.RigidBody.DebugDraw(drawer);
                    }
                }
            }
        }

        public void Dispose()
        {
            
        }

        public void RegisterRigidBodyForHasMatrix(RigidBody rigidBody, IHasTransform hasTransform, bool staticAndImmovable)
        {
            _rigidBodyMappings.Add(new RigidBodyMapping(rigidBody, hasTransform, staticAndImmovable));
            _physicsWorld.AddBody(rigidBody);
        }

        public JitterWorld GetJitterWorld()
        {
            return _physicsWorld;
        }

        public void UnregisterRigidBodyForHasMatrix(RigidBody rigidBody, IHasTransform hasTransform)
        {
            _rigidBodyMappings.RemoveAll(x => x.RigidBody == rigidBody && x.HasTransform == hasTransform);
            _physicsWorld.RemoveBody(rigidBody);
        }

        public PhysicsMetrics GetPhysicsMetrics()
        {
            return _physicsMetrics;
        }
    }
}