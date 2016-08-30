using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Protogame.ATFLevelEditor;
using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The level reader for levels saved from an ATF level editor.
    /// </summary>
    /// <module>Level</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.ILevelReader</interface_ref>
    public class ATFLevelReader : ILevelReader
    {
        private readonly IKernel _kernel;

        private readonly IHierarchy _hierarchy;

        public ATFLevelReader(IKernel kernel, IHierarchy hierarchy)
        {
            _kernel = kernel;
            _hierarchy = hierarchy;
        }
        
        public IEnumerable<IEntity> Read(Stream stream, object context, Func<IPlan, object, bool> filter)
        {
            var node = _hierarchy.Lookup(context);

            var document = new XmlDocument();
            document.Load(stream);

            if (document.DocumentElement == null)
            {
                throw new InvalidOperationException("The level data doesn't contain a document element.");
            }

            // Find the <gameObjectFolder> node under the root element.  This is
            // the top of our hierarchy.
            var gameObjectFolder =
                document.DocumentElement.ChildNodes.OfType<XmlElement>()
                    .FirstOrDefault(x => x.LocalName == "gameObjectFolder");

            if (gameObjectFolder == null)
            {
                throw new InvalidOperationException("No top level game folder found in ATF level.");
            }

            // Construct the plans for the level.
            var plansList = new List<IPlan>();
            foreach (var element in gameObjectFolder.ChildNodes.OfType<XmlElement>())
            {
                ProcessElementToPlan(node, null, element, plansList, 0, filter);
            }

            // Validate the level configuration.
            var plans = plansList.ToArray();
            IEnumerable<IEntity> entities = null;
            bool isOkay = false;
            try
            {
                _kernel.ValidateAll(plans);

                // Resolve all the plans.
                entities = _kernel.ResolveAll(plans).OfType<IEntity>();
                isOkay = true;
                return entities;
            }
            finally
            {
                if (!isOkay)
                {
                    _kernel.DiscardAll(plans);

                    foreach (var plan in plans)
                    {
                        // Also detach the child nodes that were appended to the root object.
                        _hierarchy.RemoveChildNode(node, (INode)plan);
                    }
                }
            }
        }

        public IEnumerable<IEntity> Read(Stream stream, object context)
        {
            return Read(stream, context, null);
        }

        private void ProcessElementToPlan(IPlan parentPlan, IPlan rootPlan, XmlElement currentElement, List<IPlan> rootPlans, int depth, Func<IPlan, object, bool> filter)
        {
            switch (currentElement.LocalName)
            {
                case "gameObjectFolder":
                {
                    foreach (var childElement in currentElement.ChildNodes.OfType<XmlElement>())
                    {
                        ProcessElementToPlan(parentPlan, rootPlan, childElement, rootPlans, depth, filter);
                    }
                    break;
                }
                case "resource":
                {
                    // This is used directly by the parent game object.
                    break;
                }
                case "gameObject":
                {
                    Type targetType = null;

                    var xsiType = currentElement.GetAttribute("xsi:type");
                    if (xsiType == "gameObjectGroupType")
                    {
                        targetType = typeof (EntityGroup);
                    }
                    else if (xsiType == "locatorType")
                    {
                        targetType = typeof (LocatorEntity);
                    }
                    else if (xsiType == "DirLight")
                    {
                        targetType = typeof (DefaultDirectionalLightEntity);
                    }
                    else if (xsiType == "PointLight")
                    {
                        targetType = typeof (DefaultPointLightEntity);
                    }

                    var qualifiedName = currentElement.GetAttribute("qualifiedName");
                    if (!string.IsNullOrEmpty(qualifiedName))
                    {
                        targetType = Type.GetType(qualifiedName);
                    }

                    if (targetType == null)
                    {
                        break;
                    }

                    var queryType = typeof (IEditorQuery<>).MakeGenericType(targetType);
                        
                    var plan = _kernel.Plan(
                        targetType,
                        (INode) parentPlan,
                        null,
                        null,
                        (INode) rootPlan,
                        null,
                        null,
                        new Dictionary<Type, List<IMapping>>
                        {
                            {
                                queryType, new List<IMapping>
                                {
                                    new DefaultMapping(
                                        null,
                                        ctx => ResolveEditorQueryForElement(ctx, queryType, targetType, currentElement),
                                        false,
                                        null,
                                        null,
                                        true,
                                        true,
                                        null)
                                }
                            }
                        });

                    if (filter != null && !filter(plan, currentElement))
                    {
                        // Discard this plan; it has been filtered out.
                        _kernel.Discard(plan);
                    }
                    else
                    {
                        _hierarchy.AddChildNode(parentPlan, (INode) plan);

                        if (rootPlan == null)
                        {
                            rootPlan = (INode) plan;
                        }

                        if (depth == 0)
                        {
                            rootPlans.Add(plan);
                        }

                        foreach (var childElement in currentElement.ChildNodes.OfType<XmlElement>())
                        {
                            ProcessElementToPlan(plan, rootPlan, childElement, null, depth + 1, filter);
                        }

                        if (depth > 0)
                        {
                            parentPlan.PlannedCreatedNodes.InsertRange(0, plan.PlannedCreatedNodes);
                        }
                    }

                    break;
                }
            }
        }

        private object ResolveEditorQueryForElement(IContext context, Type queryType, Type targetType, XmlElement element)
        {
            var loadingType = typeof (LoadingEditorQuery<>).MakeGenericType(targetType);
            return context.Kernel.Get(loadingType, context.Parent, new NamedConstructorArgument("element", element));
        }
    }
}