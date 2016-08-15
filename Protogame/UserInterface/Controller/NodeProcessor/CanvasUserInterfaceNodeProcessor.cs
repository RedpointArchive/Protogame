using System;
using System.Xml;

namespace Protogame
{
    public class CanvasUserInterfaceNodeProcessor : IUserInterfaceNodeProcessor
    {
        public IContainer Process(XmlNode node, Action<UserInterfaceBehaviourEvent, object> eventCallback, out Action<XmlNode, IContainer> processChild)
        {
            var canvas = new Canvas();
            processChild = (childNode, childContainer) =>
            {
                canvas.SetChild(childContainer);
            };
            return canvas;
        }
    }
}
