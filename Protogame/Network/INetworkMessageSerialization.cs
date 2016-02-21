namespace Protogame
{
    public interface INetworkMessageSerialization
    {
        byte[] Serialize(object message);

        object Deserialize(byte[] data);
    }
}