using System;
using System.IO;
using System.Xml;

namespace Protogame
{
    public class TextBoxUserInterfaceNodeProcessor : IUserInterfaceNodeProcessor
    {
        public IContainer Process(XmlNode node, Action<UserInterfaceBehaviourEvent, object> eventCallback, out Action<XmlNode, IContainer> processChild)
        {
            var textBox = new TextBox();
            textBox.Text = node?.Attributes?["text"]?.Value ?? string.Empty;
            textBox.Hint = node?.Attributes?["hint"]?.Value;
            textBox.TextChanged += (sender, args) =>
            {
                eventCallback(UserInterfaceBehaviourEvent.TextChanged, textBox);
            };
            processChild = (xmlNode, container) =>
            {
                throw new InvalidDataException("The '" + xmlNode.LocalName + "' control can not have any children.");
            };
            return textBox;
        }
    }
}
