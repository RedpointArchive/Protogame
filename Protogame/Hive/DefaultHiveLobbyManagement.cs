using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HiveMP.Lobby.Api;
using HiveMP.Lobby.Model;
using HiveMP.TemporarySession.Model;

namespace Protogame
{
    /// <summary>
    /// Default implementation of <see cref="IHiveLobbyManagement"/>.
    /// </summary>
    /// <module>Hive</module>
    /// <internal>true</internal>
    /// <interface_ref>Protogame.IHiveLobbyManagement</interface_ref>
    public class DefaultHiveLobbyManagement : IHiveLobbyManagement
    {
        public async Task<LobbyInfo> CreateLobby(TempSessionWithSecrets userSession)
        {
            var lobbyApi = new LobbyApi();
            lobbyApi.Configuration.ApiKey["api_key"] = userSession.ApiKey;
            return await lobbyApi.LobbyPutAsync();
        }

        public async Task<IEnumerable<LobbyInfo>> GetLobbies(TempSessionWithSecrets userSession)
        {
            var lobbyApi = new LobbyApi();
            lobbyApi.Configuration.ApiKey["api_key"] = userSession.ApiKey;
            return await lobbyApi.LobbiesGetAsync();
        }

        public async Task<ConnectedSession> JoinLobby(TempSessionWithSecrets userSession, LobbyInfo lobby)
        {
            var lobbyApi = new LobbyApi();
            lobbyApi.Configuration.ApiKey["api_key"] = userSession.ApiKey;
            return await lobbyApi.SessionPutAsync(lobby.Id, userSession.Id);
        }

        public async Task LeaveLobby(TempSessionWithSecrets userSession, LobbyInfo lobby)
        {
            var lobbyApi = new LobbyApi();
            lobbyApi.Configuration.ApiKey["api_key"] = userSession.ApiKey;
            await lobbyApi.SessionDeleteAsync(lobby.Id, userSession.Id);
        }

        public async Task DeleteLobby(TempSessionWithSecrets userSession, LobbyInfo lobby)
        {
            var lobbyApi = new LobbyApi();
            lobbyApi.Configuration.ApiKey["api_key"] = userSession.ApiKey;
            await lobbyApi.LobbyDeleteAsync(lobby.Id);
        }

        public async Task KickSessionFromLobby(TempSessionWithSecrets userSession, ConnectedSession lobbySession)
        {
            var lobbyApi = new LobbyApi();
            lobbyApi.Configuration.ApiKey["api_key"] = userSession.ApiKey;
            await lobbyApi.SessionDeleteAsync(lobbySession.LobbyId, lobbySession.SessionId);
        }

        public async Task<IEnumerable<ConnectedSession>> GetSessionsInLobby(TempSessionWithSecrets userSession, LobbyInfo lobby)
        {
            var lobbyApi = new LobbyApi();
            lobbyApi.Configuration.ApiKey["api_key"] = userSession.ApiKey;
            return await lobbyApi.SessionsGetAsync(lobby.Id);
        }
    }
}
