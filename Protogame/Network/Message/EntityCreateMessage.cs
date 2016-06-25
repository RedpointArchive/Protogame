using ProtoBuf;

namespace Protogame
{
    [NetworkMessage("entity:create"), ProtoContract]
    public class EntityCreateMessage : INetworkMessage
    {
        [ProtoMember(1)]
        public int EntityID;

        [ProtoMember(2)]
        public string EntityType;

        [ProtoMember(3)]
        public NetworkTransform InitialTransform;

        [ProtoMember(4)]
        public int FrameTick;
    }
}
