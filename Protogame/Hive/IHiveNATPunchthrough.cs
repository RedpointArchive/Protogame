using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HiveMP.NATPunchthrough.Model;
using HiveMP.TemporarySession.Model;

namespace Protogame
{
    /// <summary>
    /// Allows you to perform NAT punchthrough using Hive.
    /// </summary>
    /// <module>Hive</module>
    public interface IHiveNATPunchthrough
    {
        /// <summary>
        /// Performs NAT punchthrough for the given user session with traffic from a specific port.
        /// </summary>
        /// <param name="userSession">The authenticated user session.</param>
        /// <param name="udpPort">The UDP port that traffic will be sent from in the game.</param>
        /// <param name="timeout">The timeout before giving up on NAT punchthrough (in milliseconds).  If this value is <c>0</c>, then no timeout applies.</param>
        /// <returns>The asynchronous task that indicates completion.</returns>
        /// <exception cref="InvalidOperationException">The server provided an invalid NAT negotiation response.</exception>
        /// <exception cref="TimeoutException">The NAT punchthrough failed to complete before the timeout occurred.</exception>
        Task PerformNATPunchthrough(TempSessionWithSecrets userSession, int udpPort, int timeout);

        /// <summary>
        /// Performs NAT punchthrough for the given user session with traffic from a dynamically allocated port.
        /// </summary>
        /// <param name="userSession">The authenticated user session.</param>
        /// <param name="timeout">The timeout before giving up on NAT punchthrough (in milliseconds).  If this value is <c>0</c>, then no timeout applies.</param>
        /// <returns>The dynamically allocated port to send future traffic from.</returns>
        /// <exception cref="InvalidOperationException">The server provided an invalid NAT negotiation response.</exception>
        /// <exception cref="TimeoutException">The NAT punchthrough failed to complete before the timeout occurred.</exception>
        Task<int> PerformNATPunchthrough(TempSessionWithSecrets userSession, int timeout);

        /// <summary>
        /// Looks up registered NAT endpoints for the given target session.  You can use this
        /// method to find out what the external IP address and port is for another player.
        /// </summary>
        /// <param name="userSession">The authenticated user session.</param>
        /// <param name="targetSession">The target session to lookup endpoints registered via NAT punchthrough.</param>
        /// <returns>A list of registered endpoints.</returns>
        Task<List<NATEndpoint>> LookupEndpoints(TempSessionWithSecrets userSession, string targetSession);
    }
}