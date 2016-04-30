using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Protoinject;

namespace Protogame
{
    public class EntityGroup : IEntity
    {
        private readonly INode _node;

        public EntityGroup(INode node)
        {
            _node = node;
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
