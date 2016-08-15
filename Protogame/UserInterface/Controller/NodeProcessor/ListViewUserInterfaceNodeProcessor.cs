using System;
using System.IO;
using System.Xml;

namespace Protogame
{
    public class ListViewUserInterfaceNodeProcessor : IUserInterfaceNodeProcessor
    {
        public IContainer Process(XmlNode node, Action<UserInterfaceBehaviourEvent, object> eventCallback, out Action<XmlNode, IContainer> processChild)
        {
            var listView = new ListView();
            listView.SelectedItemChanged += (sender, args) =>
            {
                eventCallback(UserInterfaceBehaviourEvent.SelectedItemChanged, listView);
            };
            processChild = (xmlNode, container) =>
            {
                listView.AddChild(container);
            };
            return listView;
        }
    }
}
