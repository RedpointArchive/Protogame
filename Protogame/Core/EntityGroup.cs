using System;
using System.Collections.Generic;
using System.Linq;
using Protogame.ATFLevelEditor;
using Protoinject;

namespace Protogame
{
    public class EntityGroup : IContainsEntities, IEventListener<IGameContext>, IEventListener<INetworkEventContext>, IEventListener<IPhysicsEventContext>, IHasLights, IEntity, IServerEntity, INetworkIdentifiable, ISynchronisedObject, IPrerenderableEntity, IQueryableComponent
    {
        private readonly INode _node;

        private INode<IEntity>[] _entityCache = new INode<IEntity>[0];

        private INode<IServerEntity>[] _serverEntityCache = new INode<IServerEntity>[0];

        private readonly HashSet<Type> _enabledInterfaces = new HashSet<Type>();

        private bool _hasRenderableComponentDescendants;

        private bool _hasPrerenderableComponentDescendants;

        private bool _hasUpdatableComponentDescendants;

        private bool _hasServerUpdatableComponentDescendants;

        private bool _hasLightableComponentDescendants;

        public EntityGroup(INode node, IEditorQuery<EntityGroup> editorQuery)
        {
            _node = node;
            Transform = new DefaultTransform();

            // EditorGroup is used to represent game groups in the editor
            // and we need to map the transform to this object.
            if (editorQuery.Mode == EditorQueryMode.LoadingConfiguration)
            {
                editorQuery.MapTransform(this, x => this.Transform.Assign(x));

                _node.ChildrenChanged += ChildrenChanged;
                _node.DescendantsChanged += DescendantsChanged;
                ChildrenChanged(null, null);
                DescendantsChanged(null, null);
            }
        }

        private void DescendantsChanged(object sender, EventArgs e)
        {
            _enabledInterfaces.Clear();

            var children = _node.Children.Select(x => x.UntypedValue).ToArray();

            for (var i = 0; i < children.Length; i++)
            {
                var queryableComponent = children[i] as IQueryableComponent;
                if (queryableComponent != null)
                {
                    _enabledInterfaces.UnionWith(queryableComponent.EnabledInterfaces);
                }
                else if (children[i] != null)
                {
                    _enabledInterfaces.UnionWith(children[i].GetType().GetInterfaces());
                }
            }

            _hasRenderableComponentDescendants = _enabledInterfaces.Contains(typeof(IRenderableComponent));
            _hasPrerenderableComponentDescendants = _enabledInterfaces.Contains(typeof(IPrerenderableComponent));
            _hasUpdatableComponentDescendants = _enabledInterfaces.Contains(typeof(IUpdatableComponent));
            _hasServerUpdatableComponentDescendants = _enabledInterfaces.Contains(typeof(IServerUpdatableComponent));
            _hasLightableComponentDescendants = _enabledInterfaces.Contains(typeof(ILightableComponent));
        }

        private void ChildrenChanged(object sender, EventArgs e)
        {
            _entityCache = _node.Children.Where(x => typeof(IEntity).IsAssignableFrom(x.Type)).Cast<INode<IEntity>>().ToArray();
            _serverEntityCache = _node.Children.Where(x => typeof(IServerEntity).IsAssignableFrom(x.Type)).Cast<INode<IServerEntity>>().ToArray();
        }

        public ITransform Transform { get; }

        public IFinalTransform FinalTransform
        {
            get { return this.GetAttachedFinalTransformImplementation(_node); }
        }

        public void Update(IServerContext serverContext, IUpdateContext updateContext)
        {
            if (_hasServerUpdatableComponentDescendants)
            {
                for (var i = 0; i < _entityCache.Length; i++)
                {
                    _serverEntityCache[i].Value.Update(serverContext, updateContext);
                }
            }
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (_hasRenderableComponentDescendants)
            {
                for (var i = 0; i < _entityCache.Length; i++)
                {
                    _entityCache[i].Value.Render(gameContext, renderContext);
                }
            }
        }

        public void Prerender(IGameContext gameContext, IRenderContext renderContext)
        {
            if (_hasPrerenderableComponentDescendants)
            {
                foreach (var child in _node.Children.Select(x => x.UntypedValue).OfType<IPrerenderableEntity>())
                {
                    child.Prerender(gameContext, renderContext);
                }
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (_hasUpdatableComponentDescendants)
            {
                for (var i = 0; i < _entityCache.Length; i++)
                {
                    _entityCache[i].Value.Update(gameContext, updateContext);
                }
            }
        }

        public bool Handle(IGameContext context, IEventEngine<IGameContext> eventEngine, Event @event)
        {
            if (EnabledInterfaces.Contains(typeof(IEventListener<IGameContext>)))
            {
                foreach (var child in _node.Children.Select(x => x.UntypedValue).OfType<IEventListener<IGameContext>>())
                {
                    if (child.Handle(context, eventEngine, @event))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Handle(IPhysicsEventContext context, IEventEngine<IPhysicsEventContext> eventEngine, Event @event)
        {
            foreach (var child in _node.Children.Select(x => x.UntypedValue).OfType<IEventListener<IPhysicsEventContext>>())
            {
                if (child.Handle(context, eventEngine, @event))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Handle(INetworkEventContext context, IEventEngine<INetworkEventContext> eventEngine, Event @event)
        {
            foreach (var child in _node.Children.Select(x => x.UntypedValue).OfType<IEventListener<INetworkEventContext>>())
            {
                if (child.Handle(context, eventEngine, @event))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<ILight> GetLights()
        {
            if (_hasLightableComponentDescendants)
            {
                foreach (var child in _node.Children.Select(x => x.UntypedValue).OfType<IHasLights>())
                {
                    foreach (var light in child.GetLights())
                    {
                        yield return light;
                    }
                }
            }
        }

        public void ReceiveNetworkIDFromServer(IGameContext gameContext, IUpdateContext updateContext, int identifier, int initialFrameTick)
        {
            throw new InvalidOperationException(
                "Entity groups can not receive network IDs.  This indicates an error in the code.");
        }

        public void ReceivePredictedNetworkIDFromClient(IServerContext serverContext, IUpdateContext updateContext, MxClient client,
            int predictedIdentifier)
        {
            throw new InvalidOperationException(
                "Entity groups can not receive predicted network IDs.  This indicates an error in the code.");
        }

        public void DeclareSynchronisedProperties(ISynchronisationApi synchronisationApi)
        {
            throw new InvalidOperationException(
                "Entity groups can not declare synchronised properties.  Do not attach a network synchronisation component to an entity group.");
        }
        
        public HashSet<Type> EnabledInterfaces => _enabledInterfaces;
    }
}
