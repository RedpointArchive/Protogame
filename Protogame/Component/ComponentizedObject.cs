using System;
using System.Collections.Generic;
using System.Linq;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The base class for objects which support having components.
    /// <para>
    /// Unlike <see cref="ComponentizedEntity" />, this is a generic object that
    /// supports having components.  By default, it doesn't handle any components
    /// that are attached to it.
    /// </para>
    /// <para>
    /// You can support additional component types and callbacks through the use
    /// of <see cref="ComponentizedObject.RegisterCallable{T}"/>.
    /// </para>
    /// </summary>
    /// <module>Component</module>
    [InjectFieldsForBaseObjectInProtectedConstructor]
    public partial class ComponentizedObject : IContainsComponents, IQueryableComponent
    {
#pragma warning disable 169
        /// <summary>
        /// The node associated with this object.  This field is directly set by the kernel due to [InjectFieldsForBaseObjectInProtectedConstructor].
        /// </summary>
        private readonly INode _node;

        /// <summary>
        /// The dependency injection hierarchy.  This field is directly set by the kernel due to [InjectFieldsForBaseObjectInProtectedConstructor].
        /// </summary>
        private readonly IHierarchy _hierarchy;
#pragma warning restore 169

        /// <summary>
        /// When the componentized object has not been initialized by a dependency
        /// injection system, _node and _hierarchy will be null.  In this scenario,
        /// we use an private list to store components rather than the hierarchy
        /// system.
        /// </summary>
        private List<object> _nonHierarchyComponents = new List<object>();

        /// <summary>
        /// A list of interfaces which are either enabled via querying <see cref="IQueryableComponent"/>
        /// on children, or which children implement (where they don't also implement <see cref="IQueryableComponent"/>).
        /// </summary>
        private HashSet<Type> _enabledInterfaces = new HashSet<Type>();

        private bool _usesHierarchy;

        private object[] _componentCache;
        
        private List<IInternalComponentCallable> _knownCallablesForSync;

        protected ComponentizedObject()
        {
            FinalizeSetup();
        }

        public ComponentizedObject(IHierarchy hierarchy, INode node)
        {
            _hierarchy = hierarchy;
            _node = node;

            FinalizeSetup();
        }

        private void FinalizeSetup()
        {
            _knownCallablesForSync = new List<IInternalComponentCallable>();

            _usesHierarchy = _node != null && _hierarchy != null;

            if (_usesHierarchy)
            {
                _node.ChildrenChanged += (sender, args) => { UpdateCache(); OnComponentsChanged(); };
                _node.DescendantsChanged += (sender, args) => { UpdateEnabledInterfacesCache(); };
            }

            UpdateCache();
            UpdateEnabledInterfacesCache();
        }

        protected virtual void OnComponentsChanged()
        {
        }

        private void UpdateEnabledInterfacesCache()
        {
            _enabledInterfaces.Clear();

            for (var i = 0; i < _componentCache.Length; i++)
            {
                var queryableComponent = _componentCache[i] as IQueryableComponent;
                if (queryableComponent != null)
                {
                    _enabledInterfaces.UnionWith(queryableComponent.EnabledInterfaces);
                }
                else if (_componentCache[i] != null)
                {
                    _enabledInterfaces.UnionWith(_componentCache[i].GetType().GetInterfaces());
                }
            }

            AddAdditionalEnabledInterfaces(_enabledInterfaces);
        }

        protected virtual void AddAdditionalEnabledInterfaces(HashSet<Type> enabledInterfaces)
        {   
        }
            
        private void UpdateCache()
        {
            _componentCache = new object[0];

            if (_usesHierarchy)
            {
                if (_node != null && _node.Children != null)
                {
                    _componentCache = _node.Children.Select(x => x.UntypedValue).ToArray();
                }
            }
            else
            {
                if (_nonHierarchyComponents != null)
                {
                    _componentCache = _nonHierarchyComponents.ToArray();
                }
            }
            
            foreach (var callable in _knownCallablesForSync)
            {
                callable.SyncComponents(_componentCache);
            }
        }

        protected void RegisterComponent(object component)
        {
            if (_node == null || _hierarchy == null)
            {
                // No hierarchy, always use private list.
                _nonHierarchyComponents.Add(component);

                UpdateCache();
                UpdateEnabledInterfacesCache();
            }
            else
            {
                // Check if the component is underneath this hierarchy, if node, add it.
                if (_node.Children.All(x => x.UntypedValue != component))
                {
                    // It is not underneath the hierarchy; add it.
                    var childNode = _hierarchy.Lookup(component) ?? _hierarchy.CreateNodeForObject(component);

                    _hierarchy.AddChildNode(_node, childNode);
                }
            }
        }

        public object[] Components
        {
            get { return _componentCache; }
        }

        public bool AreComponentsStoredInHierarchy
        {
            get { return _usesHierarchy; }
        }

        private interface IInternalComponentCallable
        {
            void SyncComponents(object[] components);
        }

        public HashSet<Type> EnabledInterfaces
        {
            get { return _enabledInterfaces; }
        }
    }
}