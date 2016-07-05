using System;
using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
    public class DefaultNetworkFrame : INetworkFrame
    {
        private readonly Dictionary<Type, int> _messagesSent;
        private readonly Dictionary<Type, int> _messagesReceived;
        private readonly Dictionary<Type, int> _bytesSent;
        private readonly Dictionary<Type, int> _bytesReceived;

        public DefaultNetworkFrame(int id)
        {
            ID = id;
            _messagesSent = new Dictionary<Type, int>();
            _messagesReceived = new Dictionary<Type, int>();
            _bytesSent = new Dictionary<Type, int>();
            _bytesReceived = new Dictionary<Type, int>();
            SynchronisationLog = new List<string>();
        }

        public void Calculate()
        {
            MessagesSentTotal = _messagesSent.Select(x => x.Value).DefaultIfEmpty(0).Sum();
            MessagesReceivedTotal = _messagesReceived.Select(x => x.Value).DefaultIfEmpty(0).Sum();
            BytesSentTotal = _bytesSent.Select(x => x.Value).DefaultIfEmpty(0).Sum();
            BytesReceivedTotal = _bytesReceived.Select(x => x.Value).DefaultIfEmpty(0).Sum();
        }

        public int ID { get; }
        public int MessagesSentTotal { get; private set; }
        public int MessagesReceivedTotal { get; private set; }
        public int BytesSentTotal { get; private set; }
        public int BytesReceivedTotal { get; private set; }
        public Dictionary<Type, int> MessagesSentByMessageType => _messagesSent;
        public Dictionary<Type, int> MessagesReceivedByMessageType => _messagesReceived;
        public Dictionary<Type, int> BytesSentByMessageType => _bytesSent;
        public Dictionary<Type, int> BytesReceivedByMessageType => _bytesReceived;
        public List<string> SynchronisationLog { get; }
    }
}
