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

        private readonly CollisionSystemPersistentSAP _collisionSystem;

        private readonly JitterWorld _physicsWorld;

        private readonly IWorld _gameSystemWorld;

        private readonly List<KeyValuePair<RigidBody, IHasMatrix>> _rigidBodyMappings;

        public PhysicsShadowWorld(IPhysicsEngine physicsEngine, IWorld world)
        {
            _gameSystemWorld = world;

            _physicsEngine = physicsEngine;
            _collisionSystem = new CollisionSystemPersistentSAP
            {
                EnableSpeculativeContacts = true
            };

            _physicsWorld = new JitterWorld(_collisionSystem);
            _physicsWorld.ContactSettings.MaterialCoefficientMixing =
                ContactSettings.MaterialCoefficientMixingType.TakeMinimum;

            _physicsWorld.Gravity = new JVector(0, 0.1f, 0);

            _rigidBodyMappings = new List<KeyValuePair<RigidBody, IHasMatrix>>();
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
                var rot = hasMatrix.GetFinalMatrix().Rotation;
                var pos = hasMatrix.GetFinalMatrix().Translation;
                originalRotation[rigidBody.GetHashCode()] = rot;
                originalPosition[rigidBody.GetHashCode()] = pos;
                rigidBody.Orientation = Matrix.CreateFromQuaternion(rot).ToJitterMatrix();
                rigidBody.Position = pos.ToJitterVector();
            }

            _physicsWorld.Step((float)gameContext.GameTime.ElapsedGameTime.TotalSeconds, true);

            foreach (var kv in _rigidBodyMappings)
            {
                var rigidBody = kv.Key;
                var hasMatrix = kv.Value;

                // Calculate the changes that the physics system made in world space.
                var oldWorldRot = originalRotation[rigidBody.GetHashCode()];
                var oldWorldPos = originalPosition[rigidBody.GetHashCode()];
                var newWorldRot = rigidBody.Orientation.ToXNAMatrix().Rotation;
                var newWorldPos = rigidBody.Position.ToXNAVector();

                // Calculate the inverse matrix for this object.
                var inverse = Matrix.Invert(hasMatrix.GetFinalMatrix());

                // Determine the localised differences in position and rotation.
                var localRot = Quaternion.Inverse(oldWorldRot) * newWorldRot;
                var localPos = Vector3.Transform(newWorldPos, inverse) - Vector3.Transform(oldWorldPos, inverse);
                
                // Reverse the old translation and rotation components of the matrix.
                var newMatrix = hasMatrix.LocalMatrix*
                                Matrix.CreateTranslation(-hasMatrix.LocalMatrix.Translation) *
                                Matrix.CreateFromQuaternion(localRot)*
                                Matrix.CreateTranslation(hasMatrix.LocalMatrix.Translation + localPos);
                hasMatrix.LocalMatrix = newMatrix;
            }
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.IsCurrentRenderPass<IPhysicsDebugRenderPass>())
            {
                var drawer = new PhysicsDebugDraw(renderContext.GraphicsDevice);

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

                        kv.Key.DebugDraw(drawer);
                    }
                }

                renderContext.World = world;
            }
        }

        public void Dispose()
        {
            
        }

        public void RegisterRigidBodyForHasMatrix(RigidBody rigidBody, IHasMatrix hasMatrix)
        {
            _rigidBodyMappings.Add(new KeyValuePair<RigidBody, IHasMatrix>(rigidBody, hasMatrix));
            _physicsWorld.AddBody(rigidBody);
        }
    }
}