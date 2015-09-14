using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Targets;
using Ninject.Syntax;

namespace Protogame
{
    public class DefaultEntityFactory : IEntityFactory
    {
        private readonly IResolutionRoot _resolutionRoot;

        private List<Type> _entitiesToCreate;

        public DefaultEntityFactory(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
            _entitiesToCreate = new List<Type>();
        }

        public object HierarchyRoot { get; set; }

        public void PlanForEntityCreation<T>() where T : IEntity
        {
            _entitiesToCreate.Add(typeof(T));
        }

        public IEntity[] CreateEntities()
        {
            var results = new IEntity[_entitiesToCreate.Count];
            var parameterState = new ComponentHierarchyParameter();
            var pending = results.Length;
            var oldPending = 0;
            while (pending > 0 && oldPending != pending)
            {
                oldPending = pending;

                for (var i = 0; i < results.Length; i++)
                {
                    try
                    {
                        if (results[i] == null)
                        {
                            results[i] = (IEntity) _resolutionRoot.Get(_entitiesToCreate[i], parameterState);
                            pending--;
                        }
                    }
                    catch (NeedsComponentInHierarchyToContinueException)
                    {
                        // Try again next iteration to see if we succeed.
                    }
                }
            }

            if (pending > 0)
            {
                // TODO Show more information about the error.
                throw new Exception("One or more entities could not have their components resolved!");
            }

            return results;
        }

        private class ComponentHierarchyParameter : IComponentHierarchyParameter
        {
            // This is a very inefficient way to store this data.
            private Dictionary<object[], List<object>> _componentsAtPaths;

            public ComponentHierarchyParameter()
            {
                _componentsAtPaths = new Dictionary<object[], List<object>>();
            }

            public bool Equals(IParameter other)
            {
                return Equals(other, this);
            }

            public string Name { get { return "ComponentHierarchy"; } }
            public bool ShouldInherit {
                get { return true; }
            }
            public object GetValue(IContext context, ITarget target)
            {
                return null;
            }

            public void AddComponentAtPath(object[] path, object component)
            {
                if (!_componentsAtPaths.ContainsKey(path))
                {
                    _componentsAtPaths[path] = new List<object>();
                }
                _componentsAtPaths[path].Add(component);
            }

            public object[] GetComponentsUnderPath(object[] path, ComponentHierarchyPlannerDescendantMode includeDescendants)
            {
                var results = new List<object>();
                foreach (var kv in _componentsAtPaths)
                {
                    var search = false;
                    switch (includeDescendants)
                    {
                        case ComponentHierarchyPlannerDescendantMode.Full:
                            search = kv.Key.Length >= path.Length;
                            break;
                        case ComponentHierarchyPlannerDescendantMode.Immediate:
                            search = kv.Key.Length == path.Length || kv.Key.Length == path.Length + 1;
                            break;
                        case ComponentHierarchyPlannerDescendantMode.None:
                            search = kv.Key.Length == path.Length;
                            break;
                    }

                    if (search)
                    {
                        var matches = true;
                        for (var i = 0; i < path.Length; i++)
                        {
                            if (kv.Key[i] != path[i])
                            {
                                matches = false;
                                break;
                            }
                        }

                        if (matches)
                        {
                            results.AddRange(kv.Value);
                        }
                    }
                }
                return results.ToArray();
            }
        }
    }
}
