// ReSharper disable CheckNamespace

using ProtoBuf;

namespace Protogame
{
    [NetworkMessage("entity:ack"), ProtoContract]
    public class EntityAcknowledgeMessage : INetworkMessage
    {
        [ProtoMember(1)] public int PredictionID;
    }
}
