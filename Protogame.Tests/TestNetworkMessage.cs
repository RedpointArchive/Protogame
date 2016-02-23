using ProtoBuf;

namespace Protogame.Tests
{
    [NetworkMessage("testmessage"), ProtoContract]
    public class TestNetworkMessage : INetworkMessage
    {
        [NetworkTag(1), ProtoMember(1)]
        public int TestInteger { get; set; }

        [NetworkTag(2), ProtoMember(2)]
        public string TestString { get; set; }
    }
}