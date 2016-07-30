using System.Collections.Generic;
using System.Threading.Tasks;
using HiveMP.Lobby.Model;
using HiveMP.TemporarySession.Model;

namespace Protogame
{
    /// <summary>
    /// Allows you to interact with Hive game lobby services, creating game lobbies and
    /// managing them.
    /// </summary>
    /// <module>Hive</module>
    public interface IHiveLobbyManagement
    {
        /// <summary>
        /// Creates a new game lobby owned by the user session.  Note that this does NOT
        /// automatically join you to the lobby, you need to call <see cref="JoinLobby"/>
        /// after this.
        /// </summary>
        /// <param name="userSession">The user session that will own this lobby.</param>
        /// <returns>The new lobby information.</returns>
        Task<LobbyInfo> CreateLobby(TempSessionWithSecrets userSession);

        /// <summary>
        /// Gets a list of all available lobbies.
        /// </summary>
        /// <param name="userSession">The user session (used for authentication).</param>
        /// <returns>All of the available game lobbies.</returns>
        Task<IEnumerable<LobbyInfo>> GetLobbies(TempSessionWithSecrets userSession);

        /// <summary>
        /// Joins the specified lobby.
        /// </summary>
        /// <param name="userSession">The user session that will join the lobby.</param>
        /// <param name="lobby">The lobby to join.</param>
        /// <returns>Information about the connected user session.</returns>
        Task<ConnectedSession> JoinLobby(TempSessionWithSecrets userSession, LobbyInfo lobby);

        /// <summary>
        /// Leaves the specified lobby.
        /// </summary>
        /// <param name="userSession">The user session that will leave the lobby.</param>
        /// <param name="lobby">The lobby to join.</param>
        Task LeaveLobby(TempSessionWithSecrets userSession, LobbyInfo lobby);

        /// <summary>
        /// Deletes the specified lobby and kicks all players from it.
        /// </summary>
        /// <param name="userSession">The user session that owns the lobby.</param>
        /// <param name="lobby">The lobby to join.</param>
        Task DeleteLobby(TempSessionWithSecrets userSession, LobbyInfo lobby);

        /// <summary>
        /// Kicks the specified connected session from the lobby.
        /// </summary>
        /// <param name="userSession">The user session that owns the lobby.</param>
        /// <param name="lobbySession">The connected session to kick.</param>
        Task KickSessionFromLobby(TempSessionWithSecrets userSession, ConnectedSession lobbySession);

        /// <summary>
        /// Gets all of the connected sessions in the game lobby.  You do not have to be joined to
        /// a game lobby to query the connected sessions.
        /// </summary>
        /// <param name="userSession">The user session (used for authentication).</param>
        /// <param name="lobby">The lobby to retrieve connected sessions for.</param>
        Task<IEnumerable<ConnectedSession>> GetSessionsInLobby(TempSessionWithSecrets userSession, LobbyInfo lobby);
    }
}
