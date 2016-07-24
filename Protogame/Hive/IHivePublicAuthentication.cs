namespace Protogame
{
    /// <summary>
    /// Configures the public authentication settings for interaction with
    /// Hive multiplayer services.  You should set up this configuration
    /// at the start of your game.
    /// </summary>
    /// <module>Hive</module>
    public interface IHivePublicAuthentication
    {
        /// <summary>
        /// The public API key used to authenticate with Hive.  This API key is
        /// used when communicating with the temporary or user session services to
        /// perform initial authentication.
        /// </summary>
        string PublicApiKey { get; set; }
    }
}