// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class PhysicsShadowWorld : IDisposable
    {
        private readonly IEventEngine<IPhysicsEventContext> _physicsEventEngine;
        private readonly IHierarchy _hierarchy;

        private readonly IDebugRenderer _debugRenderer;
        private readonly IConsoleHandle _consoleHandle;

        private readonly JitterWorld _physicsWorld;

        private readonly List<RigidBodyMapping> _rigidBodyMappings;

        private readonly Dictionary<int, Quaternion> _lastFrameRotation;

        private readonly Dictionary<int, Vector3> _lastFramePosition;

        private readonly Dictionary<int, WeakReference<IHasTransform>> _transformCache;

        private readonly PhysicsMetrics _physicsMetrics = new PhysicsMetrics();

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly DefaultPhysicsEventContext _physicsEventContext = new DefaultPhysicsEventContext();

        private IGameContext _gameContext;

        private IServerContext _serverContext;

        private IUpdateContext _updateContext;

        public PhysicsShadowWorld(
            IEventEngine<IPhysicsEventContext> physicsEventEngine, 
            IHierarchy hierarchy,
            IDebugRenderer debugRenderer,
            IConsoleHandle consoleHandle)
        {
            _physicsEventEngine = physicsEventEngine;
            _hierarchy = hierarchy;
            _debugRenderer = debugRenderer;
            _consoleHandle = consoleHandle;

            var collisionSystem = new CollisionSystemPersistentSAP
            {
                EnableSpeculativeContacts = true
            };

            _physicsWorld = new JitterWorld(collisionSystem);
            _physicsWorld.ContactSettings.MaterialCoefficientMixing =
                ContactSettings.MaterialCoefficientMixingType.TakeMinimum;

            _physicsWorld.Gravity = new JVector(0, -10f, 0);

            _rigidBodyMappings = new List<RigidBodyMapping>();

            _lastFramePosition = new Dictionary<int, Vector3>();
            _lastFrameRotation = new Dictionary<int, Quaternion>();

            _physicsWorld.Events.BodiesBeginCollide += EventsOnBodiesBeginCollide;
            _physicsWorld.Events.BodiesEndCollide += EventsOnBodiesEndCollide;
        }

        private void EventsOnBodiesBeginCollide(RigidBody body1, RigidBody body2)
        {
            if (body1.WeakTagOrNull != null && body2.WeakTagOrNull != null)
            {
                var node1 = _hierarchy.Lookup(body1.WeakTagOrNull);
                var node2 = _hierarchy.Lookup(body2.WeakTagOrNull);

                if (node1 == null)
                {
                    _consoleHandle.LogWarning("Unable to find hierarchy node for physics rigid body: " + body1.WeakTagOrNull);
                }

                if (node2 == null)
                {
                    _consoleHandle.LogWarning("Unable to find hierarchy node for physics rigid body: " + body2.WeakTagOrNull);
                }

                if (node1 != null && node2 != null)
                {
                    // TODO: This is pretty silly.  It should just be the nodes, not their parents.
                    var parent1 = node1.Parent ?? node1;
                    var parent2 = node2.Parent ?? node2;

                    var owner1 = parent1.UntypedValue;
                    var owner2 = parent2.UntypedValue;

                    var @event = new PhysicsCollisionBeginEvent(_gameContext, _serverContext, _updateContext, body1,
                        body2, owner1, owner2);

                    _physicsEventEngine.Fire(_physicsEventContext, @event);
                }
            }
        }

        private void EventsOnBodiesEndCollide(RigidBody body1, RigidBody body2)
        {
            if (body1.WeakTagOrNull != null && body2.WeakTagOrNull != null)
            {
                var node1 = _hierarchy.Lookup(body1.WeakTagOrNull);
                var node2 = _hierarchy.Lookup(body2.WeakTagOrNull);

                if (node1 == null)
                {
                    _consoleHandle.LogWarning("Unable to find hierarchy node for physics rigid body: " + body1.WeakTagOrNull);
                }

                if (node2 == null)
                {
                    _consoleHandle.LogWarning("Unable to find hierarchy node for physics rigid body: " + body2.WeakTagOrNull);
                }

                if (node1 != null && node2 != null)
                {
                    // TODO: This is pretty silly.  It should just be the nodes, not their parents.
                    var parent1 = node1.Parent ?? node1;
                    var parent2 = node2.Parent ?? node2;

                    var owner1 = parent1.UntypedValue;
                    var owner2 = parent2.UntypedValue;

                    var @event = new PhysicsCollisionEndEvent(_gameContext, _serverContext, _updateContext, body1, body2,
                        owner1, owner2);

                    _physicsEventEngine.Fire(_physicsEventContext, @event);
                }
            }
        }

        private class RigidBodyMapping
        {
            public RigidBodyMapping(RigidBody rigidBody, IHasTransform hasTransform, bool staticAndImmovable)
            {
                RigidBody = rigidBody;
                HasTransform = new WeakReference<IHasTransform>(hasTransform);
                StaticAndImmovable = staticAndImmovable;
                PerformedInitialSync = false;
            }

            public RigidBody RigidBody { get; set; }

            public WeakReference<IHasTransform> HasTransform { get; set; }

            public bool StaticAndImmovable { get; set; }

            public bool PerformedInitialSync { get; set; }

            public override string ToString()
            {
                return StaticAndImmovable ? "(static and immovable)" : "(dynamic / static)";
            }
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            _serverContext = serverContext;
            _updateContext = updateContext;
            Update((float)serverContext.GameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _gameContext = gameContext;
            _updateContext = updateContext;
            Update((float) gameContext.GameTime.ElapsedGameTime.TotalSeconds);
        }

        private void Update(float totalSecondsElapsed)
        {
            var originalRotation = new Dictionary<int, Quaternion>();
            var originalPosition = new Dictionary<int, Vector3>();

            _stopwatch.Start();
            _physicsMetrics.StaticImmovableObjects = 0;
            _physicsMetrics.PhysicsObjects = 0;

            var gcRigidBodyMappings = new List<RigidBodyMapping>();

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
                IHasTransform hasTransform;

                if (!kv.HasTransform.TryGetTarget(out hasTransform))
                {
                    // The transform has been garbage collected.  We need to mark the rigid body mapping
                    // as pending deletion and skip it for now.
                    gcRigidBodyMappings.Add(kv);
                    continue;
                }
                
                // Sync game world to physics system.
                var rot = hasTransform.FinalTransform.AbsoluteRotation;
                var pos = hasTransform.FinalTransform.AbsolutePosition;
                originalRotation[rigidBody.GetHashCode()] = rot;
                originalPosition[rigidBody.GetHashCode()] = pos;

                // If the position of the entity differs from what we expect, then the user
                // probably explicitly set it and we need to sync the rigid body.
                if (_lastFramePosition.ContainsKey(rigidBody.GetHashCode()))
                {
                    var lastPosition = _lastFramePosition[rigidBody.GetHashCode()];
                    if ((lastPosition - hasTransform.Transform.LocalPosition).LengthSquared() > 0.0001f)
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
                    var b = Quaternion.Normalize(hasTransform.Transform.LocalRotation);
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

            if (gcRigidBodyMappings.Count > 0)
            {
                foreach (var kv in gcRigidBodyMappings)
                {
                    // Remove the rigid body whose transform has been garbage collected.
                    _physicsWorld.RemoveBody(kv.RigidBody);
                    _rigidBodyMappings.Remove(kv);
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
                IHasTransform hasTransform;

                if (!kv.HasTransform.TryGetTarget(out hasTransform))
                {
                    // The transform has been garbage collected.  Next step we'll detect it's been garbage
                    // collected and deal with it then.
                    continue;
                }

                // Calculate the changes that the physics system made in world space.
                var oldWorldRot = Quaternion.Normalize(originalRotation[rigidBody.GetHashCode()]);
                var oldWorldPos = originalPosition[rigidBody.GetHashCode()];
                var newWorldRot = Quaternion.Normalize(JQuaternion.CreateFromMatrix(rigidBody.Orientation).ToXNAQuaternion());
                var newWorldPos = rigidBody.Position.ToXNAVector();
                
                // Determine the localised differences in position.
                var localPos = newWorldPos - oldWorldPos;
                
                // Update the local components of the transform.
                hasTransform.Transform.LocalPosition += localPos;
                hasTransform.Transform.LocalRotation *= Quaternion.Inverse(oldWorldRot)*newWorldRot;

                // Save the current rotation / position for the next frame.
                _lastFramePosition[rigidBody.GetHashCode()] = hasTransform.Transform.LocalPosition;
                _lastFrameRotation[rigidBody.GetHashCode()] = hasTransform.Transform.LocalRotation;

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
            if (_rigidBodyMappings.Any(x =>
            {
                IHasTransform hasTransform2;
                if (x.HasTransform.TryGetTarget(out hasTransform2))
                {
                    return x.RigidBody == rigidBody && hasTransform2 == hasTransform;
                }
                return false;
            }))
            {
                throw new InvalidOperationException();
            }

            _rigidBodyMappings.Add(new RigidBodyMapping(rigidBody, hasTransform, staticAndImmovable));
            _physicsWorld.AddBody(rigidBody);
        }

        public JitterWorld GetJitterWorld()
        {
            return _physicsWorld;
        }

        public void UnregisterRigidBodyForHasMatrix(RigidBody rigidBody, IHasTransform hasTransform)
        {
            Func<RigidBodyMapping, bool> predicate1 = x =>
            {
                IHasTransform hasTransform2;
                if (x.HasTransform.TryGetTarget(out hasTransform2))
                {
                    return x.RigidBody != rigidBody || hasTransform2 != hasTransform;
                }
                return true;
            };

            if (_rigidBodyMappings.All(predicate1))
            {
                throw new InvalidOperationException();
            }

            Predicate<RigidBodyMapping> predicate2 = x =>
            {
                IHasTransform hasTransform2;
                if (x.HasTransform.TryGetTarget(out hasTransform2))
                {
                    return x.RigidBody == rigidBody && hasTransform2 == hasTransform;
                }
                return false;
            };

            _rigidBodyMappings.RemoveAll(predicate2);
            _physicsWorld.RemoveBody(rigidBody);
        }

        public PhysicsMetrics GetPhysicsMetrics()
        {
            return _physicsMetrics;
        }
    }
}