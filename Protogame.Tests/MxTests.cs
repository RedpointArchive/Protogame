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
            var dispatcher1 = new MxDispatcher(9098, 9099);
            var dispatcher2 = new MxDispatcher(9100, 9101);

            try
            {
                dispatcher2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9098, 9099));

                this.SimulateNetworkCycles(4, dispatcher1, dispatcher2);
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
            var dispatcher1 = new MxDispatcher(9094, 9095);
            var dispatcher2 = new MxDispatcher(9096, 9097);

            try
            {
                dispatcher2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9094, 9095));
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
            var dispatcher1 = new MxDispatcher(9090, 9091);
            var dispatcher2 = new MxDispatcher(9092, 9093);

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

            var dispatcher1 = new MxDispatcher(9102, 9103);
            var dispatcher2 = new MxDispatcher(9104, 9105);

            try
            {
                string receivedText = null;

                dispatcher1.MessageReceived +=
                    (sender, args) => { receivedText = Encoding.ASCII.GetString(args.Payload); };

                dispatcher2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9102, 9103));

                this.SimulateNetworkCycles(2, dispatcher1, dispatcher2);

                dispatcher2.Send(
                    new DualIPEndPoint(IPAddress.Loopback, 9102, 9103), 
                    Encoding.ASCII.GetBytes(Text), 
                    true);

                Assert.Null(receivedText);

                this.SimulateNetworkCycles(7, dispatcher1, dispatcher2);

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
        public void TestUnreliableData()
        {
            const string Text = @"hello";

            var dispatcher1 = new MxDispatcher(9106, 9107);
            var dispatcher2 = new MxDispatcher(9108, 9109);

            try
            {
                string receivedText = null;

                dispatcher1.MessageReceived +=
                    (sender, args) => { receivedText = Encoding.ASCII.GetString(args.Payload); };

                dispatcher2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9106, 9107));

                this.SimulateNetworkCycles(2, dispatcher1, dispatcher2);

                dispatcher2.Send(new DualIPEndPoint(IPAddress.Loopback, 9106, 9107), Encoding.ASCII.GetBytes(Text));

                Assert.Null(receivedText);

                this.SimulateNetworkCycles(2, dispatcher1, dispatcher2);

                Assert.Equal(Text, receivedText);
            }
            finally
            {
                dispatcher1.Close();
                dispatcher2.Close();
            }
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