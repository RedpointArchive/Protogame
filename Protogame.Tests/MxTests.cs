namespace Protogame.Tests
{
    using System.Net;
    using System.Text;
    using System.Threading;
    using Xunit;

    /// <summary>
    /// Tests relating to the multiplayer connection classes.
    /// </summary>
    public class MxTests
    {
        /// <summary>
        /// Test that the dispatcher can connect and update without throwing an exception.
        /// </summary>
        [Fact]
        public void TestConnectionWithUpdate()
        {
            var dispatcher1 = new MxDispatcher(9001, 9002);
            var dispatcher2 = new MxDispatcher(9003, 9004);

            try
            {
                dispatcher2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9001, 9002));

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
        [Fact]
        public void TestConnectionWithoutUpdate()
        {
            var dispatcher1 = new MxDispatcher(9005, 9006);
            var dispatcher2 = new MxDispatcher(9007, 9008);

            try
            {
                dispatcher2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9005, 9006));
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
        [Fact]
        public void TestConstruction()
        {
            var dispatcher1 = new MxDispatcher(9009, 9010);
            var dispatcher2 = new MxDispatcher(9011, 9012);

            dispatcher1.Close();
            dispatcher2.Close();
        }

        /// <summary>
        /// Test that large amounts of data can be sent over the reliable connection
        /// without data becoming corrupt or lost.
        /// </summary>
        [Fact]
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

            var dispatcher1 = new MxDispatcher(9013, 9014);
            var dispatcher2 = new MxDispatcher(9015, 9016);

            try
            {
                string receivedText = null;

                dispatcher1.MessageReceived +=
                    (sender, args) => { receivedText = Encoding.ASCII.GetString(args.Payload); };

                dispatcher2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9013, 9014));

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                dispatcher2.Send(
                    new DualIPEndPoint(IPAddress.Loopback, 9013, 9014), 
                    Encoding.ASCII.GetBytes(Text), 
                    true);

                Assert.Null(receivedText);

                this.SimulateNetworkCycles(14, dispatcher1, dispatcher2);

                Assert.Equal(Text, receivedText);
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
        [Fact]
        public void TestHugeReliableData()
        {
            var text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
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
            while (text.Length < 256 * 1024)
            {
                text += text;
            }

            var dispatcher1 = new MxDispatcher(9017, 9018);
            var dispatcher2 = new MxDispatcher(9019, 9020);

            try
            {
                string receivedText = null;

                dispatcher1.MessageReceived +=
                    (sender, args) =>
                    {
                        receivedText = Encoding.ASCII.GetString(args.Payload);
                    };

                dispatcher2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9017, 9018));

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                dispatcher2.Send(
                    new DualIPEndPoint(IPAddress.Loopback, 9017, 9018),
                    Encoding.ASCII.GetBytes(text),
                    true);

                Assert.Null(receivedText);

                for (var i = 0; i < 256 * 1024 && receivedText == null; i++)
                {
                    this.SimulateNetworkCycles(2, dispatcher1, dispatcher2);
                }

                Assert.Equal(text, receivedText);
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
        [Fact]
        public void TestUnreliableData()
        {
            const string Text = @"hello";

            var dispatcher1 = new MxDispatcher(9021, 9022);
            var dispatcher2 = new MxDispatcher(9023, 9024);

            try
            {
                string receivedText = null;

                dispatcher1.MessageReceived +=
                    (sender, args) => { receivedText = Encoding.ASCII.GetString(args.Payload); };

                dispatcher2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9021, 9022));

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                dispatcher2.Send(new DualIPEndPoint(IPAddress.Loopback, 9021, 9022), Encoding.ASCII.GetBytes(Text));

                Assert.Null(receivedText);

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                Assert.Equal(Text, receivedText);
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
        [Fact]
        public void TestAcknowledgeEventData()
        {
            const string Text = @"hello";

            var dispatcher1 = new MxDispatcher(9023, 9024);
            var dispatcher2 = new MxDispatcher(9025, 9026);

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

                dispatcher2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9023, 9024));

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                dispatcher2.Send(new DualIPEndPoint(IPAddress.Loopback, 9023, 9024), Encoding.ASCII.GetBytes(Text));

                Assert.Null(receivedText);

                this.SimulateNetworkCycles(8, dispatcher1, dispatcher2);

                Assert.Equal(Text, receivedText);
                Assert.True(acknowledged);
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
        [Fact]
        public void TestLostIsNotTriggered()
        {
            const string Text = @"hello";

            var dispatcher1 = new MxDispatcher(9027, 9028);
            var dispatcher2 = new MxDispatcher(9029, 9030);

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

                dispatcher2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9027, 9028));

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);

                dispatcher2.Send(new DualIPEndPoint(IPAddress.Loopback, 9027, 9028), bytes);

                Assert.Null(receivedText);

                this.SimulateNetworkCycles(8, dispatcher1, dispatcher2);

                Assert.Equal(Text, receivedText);
                Assert.False(lost);
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
        [Fact]
        public void TestSequenceNumbers()
        {
            Assert.Equal(2, MxUtility.GetSequenceNumberDifference(3, 1));
            Assert.Equal(1, MxUtility.GetSequenceNumberDifference(2, 1));
            Assert.Equal(1, MxUtility.GetSequenceNumberDifference(1, 0));
            Assert.Equal(1, MxUtility.GetSequenceNumberDifference(0, uint.MaxValue));

            Assert.Equal(-1, MxUtility.GetSequenceNumberDifference(1, 2));
            Assert.Equal(-2, MxUtility.GetSequenceNumberDifference(1, 3));
            Assert.Equal(-1, MxUtility.GetSequenceNumberDifference(0, 1));
            Assert.Equal(-1, MxUtility.GetSequenceNumberDifference(uint.MaxValue, 0));

            Assert.Equal(-int.MaxValue, MxUtility.GetSequenceNumberDifference(0, uint.MaxValue / 2));
            Assert.Equal(int.MinValue, MxUtility.GetSequenceNumberDifference(0, (uint.MaxValue / 2) + 1));
            Assert.Equal(int.MaxValue, MxUtility.GetSequenceNumberDifference(0, (uint.MaxValue / 2) + 2));
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