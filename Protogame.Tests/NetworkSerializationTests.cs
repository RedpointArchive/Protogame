using System;
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
            kernel.Bind<INetworkMessage>().To<TestNetworkMessage>();

            var serializer = kernel.Get<INetworkMessageSerialization>();
            var message = new TestNetworkMessage
            {
                TestInteger = 5,
                TestString = "hello world"
            };
            serializer.Serialize(message);
        }

        public void TestNetworkMessageCanBeSerializedAndDeserialized()
        {
            var kernel = new StandardKernel();
            kernel.Load<ProtogameNetworkModule>();
            kernel.Bind<INetworkMessage>().To<TestNetworkMessage>();

            var serializer = kernel.Get<INetworkMessageSerialization>();
            var message = new TestNetworkMessage
            {
                TestInteger = 5,
                TestString = "hello world"
            };
            var data = serializer.Serialize(message);
            Type messageType;
            var deserialized = serializer.Deserialize(data, out messageType);

            _assert.Same(messageType, typeof(TestNetworkMessage));
            _assert.NotNull(deserialized);
            _assert.IsType<TestNetworkMessage>(deserialized);

            var content = (TestNetworkMessage) deserialized;
            _assert.Equal(5, content.TestInteger);
            _assert.Equal("hello world", content.TestString);
        }
    }
}