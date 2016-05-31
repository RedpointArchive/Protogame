using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class Physical3DCubeComponent : IUpdatableComponent
    {
        private readonly INode _node;
        private readonly IPhysicsEngine _physicsEngine;

        private BoxShape _boxShape;

        private RigidBody _rigidBody;

        private Vector3 _cachedScale;

        private bool _addedRigidBody;

        public Physical3DCubeComponent(INode node, IPhysicsEngine physicsEngine)
        {
            _node = node;
            _physicsEngine = physicsEngine;
            _cachedScale = new Vector3(1, 1, 1);
            _addedRigidBody = false;
            CreateShapeFromCachedScale();
        }

        public Vector3 Scale
        {
            get { return _cachedScale; }
            set
            {
                _cachedScale = value;
                CreateShapeFromCachedScale();
            }
        }

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

        private void CreateShapeFromCachedScale()
        {
            _boxShape = new BoxShape(_cachedScale.ToJitterVector());
            if (_rigidBody == null)
            {
                _rigidBody = new RigidBody(_boxShape);
            }
            else
            {
                _rigidBody.Shape = _boxShape;
            }
        }

        public void Update(ComponentizedEntity entity, IGameContext gameContext, IUpdateContext updateContext)
        {
            // Update the parent node's matrix based on the rigid body's state.
            var matrixComponent = _node.Parent?.UntypedValue as IHasMatrix;
            if (matrixComponent != null)
            {
                if (!_addedRigidBody)
                {
                    _physicsEngine.RegisterRigidBodyForHasMatrixInCurrentWorld(_rigidBody, matrixComponent);
                    _addedRigidBody = true;
                }
            }
        }
    }
}
