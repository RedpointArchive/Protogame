// ReSharper disable CheckNamespace

using Jitter.Collision.Shapes;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// This entity contains and controls the results of a batching operation.
    /// </summary>
    /// <module>Batching</module>
    public class BatchedControlEntity : ComponentizedEntity
    {
        private readonly IHierarchy _hierarchy;
        private readonly INode _node;
        private readonly IBatchedControlFactory _batchedControlFactory;

        public BatchedControlEntity(IHierarchy hierarchy, INode node, IBatchedControlFactory batchedControlFactory)
        {
            _hierarchy = hierarchy;
            _node = node;
            _batchedControlFactory = batchedControlFactory;

            Transform.LocalPosition = Vector3.Zero;
            Transform.LocalRotation = Quaternion.Identity;
            Transform.LocalScale = Vector3.One;
        }

        public INode Node => _node;

        public void AttachBatchedPhysics(CompoundShape shape)
        {
            var component = _batchedControlFactory.CreatePhysicalBatchedStaticCompoundComponent(shape);
            _hierarchy.MoveNode(_node, _hierarchy.Lookup(component));

            component.Static = true;
            component.StaticAndImmovable = true;
            component.Target = PhysicsTarget.Component;
            component.Transform.LocalPosition = -shape.Shift.ToXNAVector();
        }
    }
}
