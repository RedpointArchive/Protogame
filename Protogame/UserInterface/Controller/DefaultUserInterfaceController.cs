using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Protoinject;

namespace Protogame
{
    public class DefaultUserInterfaceController : IUserInterfaceController
    {
        private readonly IKernel _kernel;
        private readonly IHierarchy _hierarchy;
        private readonly INode _node;
        private readonly UserInterfaceAsset _userInterfaceAsset;

        private readonly
            Dictionary<string, Dictionary<UserInterfaceBehaviourEvent, Action<object, IUserInterfaceController, IGameContext, IUpdateContext>>>
            _registeredBehaviours;

        private readonly Dictionary<object, string[]> _containerBehaviours;

        private IGameContext _currentGameContext;
        private IUpdateContext _currentUpdateContext;
        private bool _loadedUserInterface;
        private CanvasEntity _canvasEntity;
        private Dictionary<string, XmlNode> _availableFragments;
        private HashSet<IContainer> _firedCreatedEventsForContainers;

        public DefaultUserInterfaceController(IKernel kernel, IHierarchy hierarchy, INode node, UserInterfaceAsset userInterfaceAsset)
        {
            _kernel = kernel;
            _hierarchy = hierarchy;
            _node = node;
            _userInterfaceAsset = userInterfaceAsset;
            _registeredBehaviours = new Dictionary<string, Dictionary<UserInterfaceBehaviourEvent, Action<object, IUserInterfaceController, IGameContext, IUpdateContext>>>();
            _containerBehaviours = new Dictionary<object, string[]>();
            _availableFragments = new Dictionary<string, XmlNode>();
            _firedCreatedEventsForContainers = new HashSet<IContainer>();
        }
        
        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            _currentGameContext = gameContext;
            _currentUpdateContext = updateContext;

            if (!_loadedUserInterface)
            {
                LoadUserInterface(gameContext);
                _loadedUserInterface = true;
            }

            foreach (var kv in _containerBehaviours)
            {
                var sender = (IContainer)kv.Key;

                if (_firedCreatedEventsForContainers.Contains(sender))
                {
                    continue;
                }

                foreach (var behaviour in kv.Value)
                {
                    if (!_registeredBehaviours.ContainsKey(behaviour))
                    {
                        continue;
                    }

                    if (!_registeredBehaviours[behaviour].ContainsKey(UserInterfaceBehaviourEvent.Create))
                    {
                        continue;
                    }

                    _registeredBehaviours[behaviour][UserInterfaceBehaviourEvent.Create](sender, this,
                        _currentGameContext, _currentUpdateContext);
                }

                _firedCreatedEventsForContainers.Add(sender);
            }

            foreach (var kv in _containerBehaviours)
            {
                var sender = kv.Key;

                foreach (var behaviour in kv.Value)
                {
                    if (!_registeredBehaviours.ContainsKey(behaviour))
                    {
                        continue;
                    }

                    if (!_registeredBehaviours[behaviour].ContainsKey(UserInterfaceBehaviourEvent.GameUpdate))
                    {
                        continue;
                    }

                    _registeredBehaviours[behaviour][UserInterfaceBehaviourEvent.GameUpdate](sender, this, _currentGameContext, _currentUpdateContext);
                }
            }
        }

        private void LoadUserInterface(IGameContext gameContext)
        {
            var document = new XmlDocument();
            document.LoadXml(_userInterfaceAsset.UserInterfaceData);

            var childNodes = document.DocumentElement?.ChildNodes;
            if (childNodes == null)
            {
                return;
            }

            foreach (var element in childNodes.OfType<XmlElement>())
            {
                if (element.Name == "canvas")
                {
                    if (_canvasEntity == null)
                    {
                        var root = ProcessElement(element);
                        if (root is Canvas)
                        {
                            _canvasEntity = _kernel.Get<CanvasEntity>(_hierarchy.Lookup(gameContext.World));
                            _canvasEntity.Canvas = (Canvas) root;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("User interface file declares more than one canvas.");
                    }
                }
                else if (element.Name == "fragment")
                {
                    _availableFragments[element.GetAttribute("name")] = element;
                }
            }
        }

        public IContainer LoadUserFragment(string name)
        {
            var c = ((CanvasFragment)ProcessElement(_availableFragments[name])).Children[0];
            c.Parent = null;
            return c;
        }

        private IContainer ProcessElement(XmlNode xmlNode)
        {
            var processor = _kernel.TryGet<IUserInterfaceNodeProcessor>(_node, xmlNode.LocalName);
            if (processor == null)
            {
                return null;
            }

            Action<XmlNode, IContainer> processChild;
            var container = processor.Process(xmlNode, HandleEventFromContainer, out processChild);
            if (container == null)
            {
                return null;
            }

            _containerBehaviours[container] = (xmlNode?.Attributes?["behaviours"]?.Value ?? string.Empty).Split(',');
            
            foreach (var child in xmlNode.ChildNodes.OfType<XmlNode>())
            {
                var childContainer = ProcessElement(child);
                if (childContainer != null)
                {
                    processChild(child, childContainer);
                }
            }

            return container;
        }

        private void HandleEventFromContainer(UserInterfaceBehaviourEvent ev, object sender)
        {
            if (!_containerBehaviours.ContainsKey(sender))
            {
                return;
            }

            var behaviours = _containerBehaviours[sender];
            foreach (var behaviour in behaviours)
            {
                if (!_registeredBehaviours.ContainsKey(behaviour))
                {
                    continue;
                }

                if (!_registeredBehaviours[behaviour].ContainsKey(ev))
                {
                    continue;
                }

                _registeredBehaviours[behaviour][ev](sender, this, _currentGameContext, _currentUpdateContext);
            }
        }

        public void RegisterBehaviour<TContainerType>(string name, UserInterfaceBehaviourEvent @event, Action<TContainerType, IUserInterfaceController, IGameContext, IUpdateContext> callback)
        {
            if (!_registeredBehaviours.ContainsKey(name))
            {
                _registeredBehaviours[name] = new Dictionary<UserInterfaceBehaviourEvent, Action<object, IUserInterfaceController, IGameContext, IUpdateContext>>();
            }

            _registeredBehaviours[name][@event] = (o, t, g, u) => { callback((TContainerType) o, t, g, u); };
        }

        public void Dispose()
        {
            if (_loadedUserInterface)
            {
                var node = _hierarchy.Lookup(_canvasEntity);
                _hierarchy.RemoveNode(node);
            }
        }
    }
}
