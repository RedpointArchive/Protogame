using Protoinject;
using Prototest.Library.Version1;

namespace Protogame.Tests
{
    public class NetworkSerializationTests
    {
        private readonly IAssert _assert;

        public NetworkSerializationTests(IAssert assert)
        {
            _assert = assert;
        }

        public void TestNetworkMessageCanBeSerialized()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameNetworkModule>();
        }
    }
}