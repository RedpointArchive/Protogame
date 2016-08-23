using Prototest.Library.Version1;

namespace Protogame.Tests
{
    using System.Net;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Tests relating to the multiplayer connection classes.
    /// </summary>
    public class MxTests
    {
        private readonly IAssert _assert;

        public MxTests(IAssert assert)
        {
            _assert = assert;
        }

        /// <summary>
        /// Test that the dispatcher can connect and update without throwing an exception.
        /// </summary>
        public void TestConnectionWithUpdate()
        {
            var dispatcher1 = new MxDispatcher(9001);
            var dispatcher2 = new MxDispatcher(9003);

            try
            {
                dispatcher2.Connect(new IPEndPoint(IPAddress.Loopback, 9001));

                this.SimulateNetworkCycles(8, dispatcher1, dispatcher2);
            }
            finally
            {
                dispatcher1.Close();
                dispatcher2.Close();
            }
        }

        /// <summary>
        /// Test that the dispatcher can connect without throwing an exception.
        /// </summary>
        public void TestConnectionWithoutUpdate()
        {
            var dispatcher1 = new MxDispatcher(9005);
            var dispatcher2 = new MxDispatcher(9007);

            try
            {
                dispatcher2.Connect(new IPEndPoint(IPAddress.Loopback, 9005));
            }
            finally
            {
                dispatcher1.Close();
                dispatcher2.Close();
            }
        }

        /// <summary>
        /// Test that the dispatcher can be constructed without throwing an exception.
        /// </summary>
        public void TestConstruction()
        {
            var dispatcher1 = new MxDispatcher(9009);
            var dispatcher2 = new MxDispatcher(9011);

            dispatcher1.Close();
            dispatcher2.Close();
        }

        /// <summary>
        /// Test that large amounts of data can be sent over the reliable connection
        /// without data becoming corrupt or lost.
        /// </summary>
        public void TestLargeReliableData()
        {
            const string Text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
Sed id enim et est aliquet mollis. Suspendisse tempus libero in mauris 
iaculis interdum. Ut quis purus in libero euismod suscipit a non est. 
Vestibulum nec scelerisque tellus. Nullam porttitor, metus vitae placerat 
dignissim, nibh ante vehicula felis, vestibulum commodo dui urna sed enim. 
Fusce id neque non neque pellentesque tincidunt. In vehicula lacus vitae nibh 
iaculis scelerisque. Ut libero felis, aliquet nec fringilla sit amet, dignissim
sed quam. Ut a pulvinar quam. Proin mollis dictum ante vel elementum. Sed 
elementum neque libero, ac hendrerit ante semper at. Donec pretium hendrerit 
nisl, non dapibus urna. Phasellus et suscipit nibh, sed tempus magna. Aliquam 
porttitor malesuada ligula, a posuere enim pellentesque quis.

Proin id neque varius, porta eros eget, pellentesque massa. Suspendisse viverra
ligula at lorem dignissim fringilla. Proin viverra nunc neque, nec dignissim 
velit viverra vitae. Vestibulum fringilla eget nunc id cursus cras amet.";

            var dispatcher1 = new MxDispatcher(9013);
            var dispatcher2 = new MxDispatcher(9015);

            try
            {
                string receivedText = null;

                dispatcher1.MessageReceived +=
                    (sender, args) => { receivedText = Encoding.ASCII.GetString(args.Payload); };

                var client = dispatcher2.Connect(new IPEndPoint(IPAddress.Loopback, 9013));
                var clientGroup = dispatcher2.PlaceInGroup(client, "Group2");

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                dispatcher2.Send(
                    clientGroup, 
                    Encoding.ASCII.GetBytes(Text), 
                    true);

                _assert.Null(receivedText);

                this.SimulateNetworkCycles(14, dispatcher1, dispatcher2);

                _assert.Equal(Text, receivedText);
            }
            finally
            {
                dispatcher1.Close();
                dispatcher2.Close();
            }
        }

        /// <summary>
        /// Test that large amounts of data can be sent over the reliable connection
        /// without data becoming corrupt or lost.
        /// </summary>
        public void TestHugeReliableData()
        {
            const string Content = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
Sed id enim et est aliquet mollis. Suspendisse tempus libero in mauris 
iaculis interdum. Ut quis purus in libero euismod suscipit a non est. 
Vestibulum nec scelerisque tellus. Nullam porttitor, metus vitae placerat 
dignissim, nibh ante vehicula felis, vestibulum commodo dui urna sed enim. 
Fusce id neque non neque pellentesque tincidunt. In vehicula lacus vitae nibh 
iaculis scelerisque. Ut libero felis, aliquet nec fringilla sit amet, dignissim
sed quam. Ut a pulvinar quam. Proin mollis dictum ante vel elementum. Sed 
elementum neque libero, ac hendrerit ante semper at. Donec pretium hendrerit 
nisl, non dapibus urna. Phasellus et suscipit nibh, sed tempus magna. Aliquam 
porttitor malesuada ligula, a posuere enim pellentesque quis.

Proin id neque varius, porta eros eget, pellentesque massa. Suspendisse viverra
ligula at lorem dignissim fringilla. Proin viverra nunc neque, nec dignissim 
velit viverra vitae. Vestibulum fringilla eget nunc id cursus cras amet.
";
            var text = string.Empty;

            while (text.Length < 128016 - Content.Length)
            {
                text += Content;
            }

            var dispatcher1 = new MxDispatcher(9017);
            var dispatcher2 = new MxDispatcher(9019);

            try
            {
                string receivedText = null;

                dispatcher1.MessageReceived +=
                    (sender, args) =>
                    {
                        receivedText = Encoding.ASCII.GetString(args.Payload);
                    };

                var client = dispatcher2.Connect(new IPEndPoint(IPAddress.Loopback, 9017));
                var clientGroup = dispatcher2.PlaceInGroup(client, "Group2");

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                dispatcher2.Send(
                    clientGroup,
                    Encoding.ASCII.GetBytes(text),
                    true);

                _assert.Null(receivedText);

                for (var i = 0; i < 246 * 1024 && receivedText == null; i++)
                {
                    this.SimulateNetworkCycles(2, dispatcher1, dispatcher2);
                }

                _assert.Equal(text, receivedText);
            }
            finally
            {
                dispatcher1.Close();
                dispatcher2.Close();
            }
        }

        /// <summary>
        /// Test that a small piece of data can be sent over the unreliable protocol.
        /// </summary>
        public void TestUnreliableData()
        {
            const string Text = @"hello";

            var dispatcher1 = new MxDispatcher(9021);
            var dispatcher2 = new MxDispatcher(9023);

            try
            {
                string receivedText = null;

                dispatcher1.MessageReceived +=
                    (sender, args) => { receivedText = Encoding.ASCII.GetString(args.Payload); };

                var client = dispatcher2.Connect(new IPEndPoint(IPAddress.Loopback, 9021));
                var clientGroup = dispatcher2.PlaceInGroup(client, "Group2");

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                dispatcher2.Send(clientGroup, Encoding.ASCII.GetBytes(Text));

                _assert.Null(receivedText);

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                _assert.Equal(Text, receivedText);
            }
            finally
            {
                dispatcher1.Close();
                dispatcher2.Close();
            }
        }

        /// <summary>
        /// Test that a small piece of data can be sent over the unreliable protocol.
        /// </summary>
        public void TestAcknowledgeEventData()
        {
            const string Text = @"hello";

            var dispatcher1 = new MxDispatcher(9033);
            var dispatcher2 = new MxDispatcher(9035);

            try
            {
                string receivedText = null;
                bool acknowledged = false;

                dispatcher1.MessageReceived +=
                    (sender, args) =>
                    {
                        receivedText = Encoding.ASCII.GetString(args.Payload);
                    };

                dispatcher2.MessageAcknowledged += (sender, args) =>
                {
                    acknowledged = true; 
                };

                var client = dispatcher2.Connect(new IPEndPoint(IPAddress.Loopback, 9033));
                var clientGroup = dispatcher2.PlaceInGroup(client, "Group2");

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                dispatcher2.Send(clientGroup, Encoding.ASCII.GetBytes(Text));

                _assert.Null(receivedText);

                this.SimulateNetworkCycles(8, dispatcher1, dispatcher2);

                _assert.Equal(Text, receivedText);
                _assert.True(acknowledged);
            }
            finally
            {
                dispatcher1.Close();
                dispatcher2.Close();
            }
        }

        /// <summary>
        /// Test that a small piece of data can be sent over the unreliable protocol.
        /// </summary>
        public void TestLostIsNotTriggered()
        {
            const string Text = @"hello";

            var dispatcher1 = new MxDispatcher(9027);
            var dispatcher2 = new MxDispatcher(9029);

            try
            {
                string receivedText = null;
                bool lost = false;
                var bytes = Encoding.ASCII.GetBytes(Text);

                dispatcher1.MessageReceived +=
                    (sender, args) =>
                    {
                        receivedText = Encoding.ASCII.GetString(args.Payload);
                    };

                dispatcher2.MessageLost += (sender, args) =>
                {
                    if (args.Payload == bytes)
                    {
                        lost = true;
                    }
                };

                var client = dispatcher2.Connect(new IPEndPoint(IPAddress.Loopback, 9027));
                var clientGroup = dispatcher2.PlaceInGroup(client, "Group2");

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                dispatcher2.Send(clientGroup, bytes);

                _assert.Null(receivedText);

                this.SimulateNetworkCycles(8, dispatcher1, dispatcher2);

                _assert.Equal(Text, receivedText);
                _assert.False(lost);
            }
            finally
            {
                dispatcher1.Close();
                dispatcher2.Close();
            }
        }

        /// <summary>
        /// Verifies that the sequence number difference is calculated correctly
        /// for different sequence numbers.
        /// </summary>
        public void TestSequenceNumbers()
        {
            _assert.Equal(2, MxUtility.GetSequenceNumberDifference(3, 1));
            _assert.Equal(1, MxUtility.GetSequenceNumberDifference(2, 1));
            _assert.Equal(1, MxUtility.GetSequenceNumberDifference(1, 0));
            _assert.Equal(1, MxUtility.GetSequenceNumberDifference(0, uint.MaxValue));

            _assert.Equal(-1, MxUtility.GetSequenceNumberDifference(1, 2));
            _assert.Equal(-2, MxUtility.GetSequenceNumberDifference(1, 3));
            _assert.Equal(-1, MxUtility.GetSequenceNumberDifference(0, 1));
            _assert.Equal(-1, MxUtility.GetSequenceNumberDifference(uint.MaxValue, 0));

            _assert.Equal(-int.MaxValue, MxUtility.GetSequenceNumberDifference(0, uint.MaxValue / 2));
            _assert.Equal(int.MinValue, MxUtility.GetSequenceNumberDifference(0, (uint.MaxValue / 2) + 1));
            _assert.Equal(int.MaxValue, MxUtility.GetSequenceNumberDifference(0, (uint.MaxValue / 2) + 2));
        }

        /// <summary>
        /// Simulate a series of network cycles on the specified dispatchers.
        /// </summary>
        /// <param name="cycles">
        /// The number of cycles to run.
        /// </param>
        /// <param name="dispatcher1">
        /// The first dispatcher.
        /// </param>
        /// <param name="dispatcher2">
        /// The second dispatcher.
        /// </param>
        private void SimulateNetworkCycles(int cycles, MxDispatcher dispatcher1, MxDispatcher dispatcher2)
        {
            for (var i = 0; i < cycles; i++)
            {
                dispatcher1.Update();
                dispatcher2.Update();
                Thread.Sleep(1000 / 30);
            }
        }
    }
}