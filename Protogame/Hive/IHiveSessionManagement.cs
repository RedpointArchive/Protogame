using System.Collections.Generic;
using System.Threading.Tasks;
using HiveMP.TemporarySession.Model;

namespace Protogame
{
    /// <summary>
    /// Allows you to interact with Hive session services, creating temporary or user sessions
    /// for your game.
    /// </summary>
    /// <module>Hive</module>
    public interface IHiveSessionManagement
    {
        /// <summary>
        /// Creates a temporary session.
        /// </summary>
        /// <returns>The temporary session information.</returns>
        Task<TempSessionWithSecrets> CreateTemporarySession();

        /// <summary>
        /// A list of active temporary sessions that have been created in this game.
        /// </summary>
        IReadOnlyCollection<TempSessionWithSecrets> ActiveTemporarySessions { get; }

        /// <summary>
        /// Refreshes the temporary sessions created by this game, if needed.
        /// </summary>
        Task RefreshTemporarySessions();
    }
}