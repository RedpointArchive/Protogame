namespace Protogame.Tests
{
    [NetworkMessage("testmessage")]
    public class TestNetworkMessage
    {
        [NetworkTag(1)]
        public int TestInteger { get; set; }

        [NetworkTag(2)]
        public string TestString { get; set; }
    }
}