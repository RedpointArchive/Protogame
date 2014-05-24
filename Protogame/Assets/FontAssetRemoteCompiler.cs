#if !PLATFORM_WINDOWS

namespace Protogame
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// The effect asset compiler.
    /// </summary>
    public class FontAssetRemoteCompiler : IAssetCompiler<FontAsset>
    {
        /// <summary>
        /// The compile.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        public void Compile(FontAsset asset, TargetPlatform platform)
        {
            var announceString = Encoding.ASCII.GetBytes("request compiler");

            var targetAddress = IPAddress.None;
            var client = new UdpClient(4321);
            client.Client.ReceiveTimeout = 500;

            try
            {
                client.Send(announceString, announceString.Length, new IPEndPoint(IPAddress.Broadcast, 4321));
            }
            catch (SocketException)
            {
                Console.WriteLine("WARNING: Unable to locate remote compiler for font compilation.");
                return;
            }

            var endpoint = new IPEndPoint(IPAddress.Loopback, 4321);
            try
            {
                var bytes = client.Receive(ref endpoint);

                while (Encoding.ASCII.GetString(bytes) == "request compiler")
                {
                    bytes = client.Receive(ref endpoint);
                }

                if (Encoding.ASCII.GetString(bytes) == "provide compiler")
                {
                    targetAddress = endpoint.Address;
                }
                else
                {
                    Console.WriteLine("WARNING: Received unexpected message from " + endpoint + " when locating remote effect compiler.");
                    return;
                }
            }
            catch
            {
                Console.WriteLine("WARNING: Unable to locate remote compiler for effect compilation.");
                return;
            }

            var content = string.Join(
                "\0",
                asset.FontName,
                asset.FontSize.ToString(),
                asset.Spacing.ToString(),
                asset.UseKerning.ToString());

            // Connect to the target on port 80.
            var webClient = new WebClient();
            try
            {
                var result = webClient.UploadData("http://" + targetAddress + ":8080/compilefont?platform=" + (int)platform, Encoding.ASCII.GetBytes(content));

                asset.PlatformData = new PlatformData { Platform = platform, Data = result };
            }
            catch (WebException ex)
            {
                using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    throw new InvalidOperationException(reader.ReadToEnd());
                }
            }

            try
            {
                asset.ReloadFont();
            }
            catch (NoAssetContentManagerException)
            {
            }
        }
    }
}

#endif