using System.Net.Sockets;
using HiveMP.TemporarySession.Model;

namespace Protogame
{
    public class BackgroundNATPunchthrough
    {
        public TempSessionWithSecrets UserSession { get; }
        public UdpClient UdpClient { get; }

        public bool ShouldStop { get; set; }

        public BackgroundNATPunchthrough(TempSessionWithSecrets userSession, UdpClient udpClient)
        {
            UserSession = userSession;
            UdpClient = udpClient;
        }
    }
}