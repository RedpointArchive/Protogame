using System;
using System.Xml;

namespace Protogame
{
    public class ContainerUserInterfaceNodeProcessor : IUserInterfaceNodeProcessor
    {
        public IContainer Process(XmlNode node, Action<UserInterfaceBehaviourEvent, object> eventCallback, out Action<XmlNode, IContainer> processChild)
        {
            IContainer container;
            switch (node?.Attributes?["type"]?.Value)
            {
                case "horizontal":
                    var horizontalContainer = new HorizontalContainer();
                    processChild = (childNode, childContainer) =>
                    {
                        horizontalContainer.AddChild(childContainer, childNode?.Attributes?["width"]?.Value ?? "*");
                    };
                    container = horizontalContainer;
                    break;
                case "vertical":
                    var verticalContainer = new VerticalContainer();
                    processChild = (childNode, childContainer) =>
                    {
                        verticalContainer.AddChild(childContainer, childNode?.Attributes?["height"]?.Value ?? "*");
                    };
                    container = verticalContainer;
                    break;
                case "single":
                    var singleContainer = new SingleContainer();
                    processChild = (childNode, childContainer) =>
                    {
                        singleContainer.SetChild(childContainer);
                    };
                    container = singleContainer;
                    break;
                case "scrollable":
                    var scrollableContainer = new ScrollableContainer();
                    processChild = (childNode, childContainer) =>
                    {
                        scrollableContainer.SetChild(childContainer);
                    };
                    container = scrollableContainer;
                    break;
                case "empty":
                    container = new EmptyContainer();
                    processChild = (childNode, childContainer) => { };
                    break;
                default:
                    processChild = (childNode, childContainer) => { };
                    return null;
            }

            return container;
        }
    }
}
