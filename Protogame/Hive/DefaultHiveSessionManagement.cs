using System.Collections.Generic;
using System.Threading.Tasks;
using HiveMP.TemporarySession.Api;
using HiveMP.TemporarySession.Model;

namespace Protogame
{
    /// <summary>
    /// Default implementation of <see cref="IHiveSessionManagement"/>.
    /// </summary>
    /// <module>Hive</module>
    /// <internal>true</internal>
    /// <interface_ref>Protogame.IHiveSessionManagement</interface_ref>
    public class DefaultHiveSessionManagement : IHiveSessionManagement
    {
        private readonly IHivePublicAuthentication _hivePublicAuthentication;
        private readonly List<TempSessionWithSecrets> _tempSessions;

        private ITemporarySessionApi _temporarySessionApi;

        public DefaultHiveSessionManagement(IHivePublicAuthentication hivePublicAuthentication)
        {
            _hivePublicAuthentication = hivePublicAuthentication;
            _tempSessions = new List<TempSessionWithSecrets>();
        }

        private void Init()
        {
            if (_temporarySessionApi == null)
            {
                _temporarySessionApi = new TemporarySessionApi();
                _temporarySessionApi.Configuration.ApiKey["api_key"] = _hivePublicAuthentication.PublicApiKey;
            }
        }

        public async Task<TempSessionWithSecrets> CreateTemporarySession()
        {
            Init();

            // Ensure this does not run as part of a coroutine - force it to
            // a background thread with Task.Run.
            return await Task.Run(async () =>
            {
                var session = await _temporarySessionApi.SessionPutAsync();
                _tempSessions.Add(session);
                return session;
            });
        }

        public IReadOnlyCollection<TempSessionWithSecrets> ActiveTemporarySessions => _tempSessions.AsReadOnly();

        public async Task RefreshTemporarySessions()
        {
            Init();

            await Task.Yield();
        }
    }
}