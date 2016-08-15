using System;
using System.IO;
using System.Xml;

namespace Protogame
{
    public class LabelUserInterfaceNodeProcessor : IUserInterfaceNodeProcessor
    {
        public IContainer Process(XmlNode node, Action<UserInterfaceBehaviourEvent, object> eventCallback, out Action<XmlNode, IContainer> processChild)
        {
            var label = new Label();
            label.Text = node?.Attributes?["text"]?.Value ?? string.Empty;
            processChild = (xmlNode, container) =>
            {
                throw new InvalidDataException("The '" + xmlNode.LocalName + "' control can not have any children.");
            };
            return label;
        }
    }
}
