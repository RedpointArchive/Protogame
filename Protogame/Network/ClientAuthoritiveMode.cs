namespace Protogame
{
    /// <summary>
    /// The authoritive mode when dealing with client data.
    /// </summary>
    public enum ClientAuthoritiveMode
    {
        /// <summary>
        /// Do nothing with client data.
        /// </summary>
        None,

        /// <summary>
        /// Trust all data sent by the client.  This is the simplest mode, but leaves
        /// your game vunerable to cheating.
        /// </summary>
        TrustClient,

        /// <summary>
        /// Replay inputs sent by the client.  Rather than clients indicating position
        /// of entities, they send the inputs players have made each frame, which
        /// are then replayed on the server.
        /// </summary>
        ReplayInputs,
    }
}
