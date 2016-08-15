using System;
using System.IO;
using System.Xml;

namespace Protogame
{
    public class ButtonUserInterfaceNodeProcessor : IUserInterfaceNodeProcessor
    {
        public IContainer Process(XmlNode node, Action<UserInterfaceBehaviourEvent, object> eventCallback, out Action<XmlNode, IContainer> processChild)
        {
            var button = new Button();
            button.Text = node?.Attributes?["text"]?.Value ?? string.Empty;
            button.Click += (sender, args) =>
            {
                eventCallback(UserInterfaceBehaviourEvent.Click, button);
            };
            processChild = (xmlNode, container) =>
            {
                throw new InvalidDataException("The '" + xmlNode.LocalName + "' control can not have any children.");
            };
            return button;
        }
    }
}
