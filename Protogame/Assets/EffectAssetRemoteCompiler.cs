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
    public class EffectAssetRemoteCompiler : IAssetCompiler<EffectAsset>
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
        public void Compile(EffectAsset asset, TargetPlatform platform)
        {
            var announceString = Encoding.ASCII.GetBytes("request compiler");

            var targetAddress = IPAddress.None;
            var client = new UdpClient(4321);
            client.Client.ReceiveTimeout = 500;

            client.Send(announceString, announceString.Length, new IPEndPoint(IPAddress.Broadcast, 4321));

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

            // Connect to the target on port 80.
            var webClient = new WebClient();
            try
            {
                var result = webClient.UploadData("http://" + targetAddress + ":8080/compileeffect?platform=" + (int)platform, Encoding.ASCII.GetBytes(asset.Code));

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
                asset.ReloadEffect();
            }
            catch (NoAssetContentManagerException)
            {
            }
        }
    }
}

#endif