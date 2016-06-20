using System.Linq;
using Protogame.ATFLevelEditor;
using Protoinject;

namespace Protogame
{
    public class EntityGroup : IEntity, IContainsEntities
    {
        private readonly INode _node;

        public EntityGroup(INode node, IEditorQuery<EntityGroup> editorQuery)
        {
            _node = node;
            
            // EditorGroup is used to represent game groups in the editor
            // and we need to map the transform to this object.
            if (editorQuery.Mode == EditorQueryMode.LoadingConfiguration)
            {
                editorQuery.MapTransform(this, x => this.Transform = x);
            }
        }

        public ITransform Transform { get; set; }

        public IFinalTransform FinalTransform
        {
            get { return this.GetAttachedFinalTransformImplementation(_node); }
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            foreach (var child in _node.Children.Select(x => x.UntypedValue).OfType<IEntity>())
            {
                child.Render(gameContext, renderContext);
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            foreach (var child in _node.Children.Select(x => x.UntypedValue).OfType<IEntity>())
            {
                child.Update(gameContext, updateContext);
            }
        }
    }
}
