using System;
using System.Xml;

namespace Protogame
{
    public class CanvasFragmentUserInterfaceNodeProcessor : IUserInterfaceNodeProcessor
    {
        public IContainer Process(XmlNode node, Action<UserInterfaceBehaviourEvent, object> eventCallback, out Action<XmlNode, IContainer> processChild)
        {
            var canvas = new CanvasFragment();
            processChild = (childNode, childContainer) =>
            {
                canvas.SetChild(childContainer);
            };
            return canvas;
        }
    }
}
