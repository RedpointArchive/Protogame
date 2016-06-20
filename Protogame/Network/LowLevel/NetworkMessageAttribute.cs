using System;
using ProtoBuf;

namespace Protogame
{
    /// <summary>
    /// Declares that the specified class is used as a network message. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class NetworkMessageAttribute : ProtoContractAttribute
    {
        public string MessageType { get; }

        public NetworkMessageAttribute(string type)
        {
            MessageType = type;
        }
    }
}