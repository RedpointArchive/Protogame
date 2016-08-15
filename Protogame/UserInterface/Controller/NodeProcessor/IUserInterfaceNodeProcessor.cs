using System;
using System.Xml;

namespace Protogame
{
    public interface IUserInterfaceNodeProcessor
    {
        IContainer Process(XmlNode node, Action<UserInterfaceBehaviourEvent, object> eventCallback, out Action<XmlNode, IContainer> processChild);
    }
}
