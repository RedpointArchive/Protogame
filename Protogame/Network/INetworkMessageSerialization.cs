using System;

namespace Protogame
{
    public interface INetworkMessageSerialization
    {
        byte[] Serialize<T>(T message);

        byte[] Serialize(object message);

        object Deserialize(byte[] message, out Type type);

        object Deserialize(byte[] message);
    }
}