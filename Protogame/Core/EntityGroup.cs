using System.Linq;
using Microsoft.Xna.Framework;
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
                editorQuery.MapMatrix(this, x => this.LocalMatrix);
            }
        }

        public Matrix LocalMatrix { get; set; }

        public Matrix GetFinalMatrix()
        {
            return this.GetDefaultFinalMatrixImplementation(_node);
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
