using System;
using System.Collections.Generic;

namespace Protogame
{
    public interface INetworkFrame
    {
        int ID { get; }

        int MessagesSentTotal { get; }

        int MessagesReceivedTotal { get; }

        int BytesSentTotal { get; }

        int BytesReceivedTotal { get; }

        Dictionary<Type, int> MessagesSentByMessageType { get; }

        Dictionary<Type, int> MessagesReceivedByMessageType { get; }

        Dictionary<Type, int> BytesSentByMessageType { get; }

        Dictionary<Type, int> BytesReceivedByMessageType { get; }

        List<string> SynchronisationLog { get; }
    }
}