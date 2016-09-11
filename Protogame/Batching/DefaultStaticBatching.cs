// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IStaticBatching"/>.
    /// </summary>
    /// <module>Batching</module>
    public class DefaultStaticBatching : IStaticBatching
    {
        private readonly IHierarchy _hierarchy;
        private readonly IBatchedControlFactory _batchedControlFactory;
        private readonly IConsoleHandle _consoleHandle;

        public DefaultStaticBatching(IHierarchy hierarchy, IBatchedControlFactory batchedControlFactory, IConsoleHandle consoleHandle)
        {
            _hierarchy = hierarchy;
            _batchedControlFactory = batchedControlFactory;
            _consoleHandle = consoleHandle;
        }

        public INode Batch(IGameContext gameContext, INode node, IBatchingOptions options)
        {
            BatchedControlEntity entity = null;

            if (options.BatchPhysics)
            {
                entity = BatchPhysics(gameContext, node, entity);
            }

            if (entity != null)
            {
                _consoleHandle.LogDebug("Attached batched control entity to parent of batch request.");

                _hierarchy.AddChildNode(node, entity.Node);
            }

            return entity?.Node;
        }

        private BatchedControlEntity BatchPhysics(IGameContext gameContext, INode node, BatchedControlEntity entity)
        {
            var physicsComponentsToProcess = new List<Tuple<INode, PhysicalBaseRigidBodyComponent>>();

            FindPhysicsComponentsUnderNode(node, physicsComponentsToProcess);

            var transformedShapes = new List<CompoundShape.TransformedShape>();

            foreach (var pair in physicsComponentsToProcess)
            {
                foreach (var body in pair.Item2.RigidBodies)
                {
                    transformedShapes.Add(new CompoundShape.TransformedShape(
                        body.Shape,
                        JMatrix.CreateFromQuaternion(pair.Item2.FinalTransform.AbsoluteRotation.ToJitterQuaternion()),
                        pair.Item2.FinalTransform.AbsolutePosition.ToJitterVector()));
                }
            }

            if (transformedShapes.Count == 0)
            {
                return entity;
            }

            var compoundShape = new CompoundShape(transformedShapes);

            if (entity == null)
            {
                entity = CreateBatchedControlEntity();
            }

            foreach (var pair in physicsComponentsToProcess)
            {
                pair.Item2.SetBatchedEntity(entity);
            }

            entity.AttachBatchedPhysics(compoundShape);

            _hierarchy.MoveNode(node, _hierarchy.Lookup(entity));

            _consoleHandle.LogInfo("Batching physics objected combined " + transformedShapes.Count + " shapes.");

            return entity;
        }

        private BatchedControlEntity CreateBatchedControlEntity()
        {
            return _batchedControlFactory.CreateBatchedControlEntity();
        }

        private static void FindPhysicsComponentsUnderNode(INode node, List<Tuple<INode, PhysicalBaseRigidBodyComponent>> physicsComponentsToProcess)
        {
            foreach (var child in node.Children)
            {
                if (typeof(PhysicalBaseRigidBodyComponent).IsAssignableFrom(child.Type))
                {
                    var component = (PhysicalBaseRigidBodyComponent) child.UntypedValue;
                    if (component.StaticAndImmovable)
                    {
                        physicsComponentsToProcess.Add(new Tuple<INode, PhysicalBaseRigidBodyComponent>(child,
                            component));
                    }
                }

                FindPhysicsComponentsUnderNode(child, physicsComponentsToProcess);
            }
        }
    }
}
