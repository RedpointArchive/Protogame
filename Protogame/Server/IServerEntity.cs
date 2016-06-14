namespace Protogame
{
    /// <summary>
    /// The ServerEntity interface.
    /// </summary>
    public interface IServerEntity : IHasTransform
    {
        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="serverContext">
        /// The server context.
        /// </param>
        /// <param name="updateContext">
        /// The update context.
        /// </param>
        void Update(IServerContext serverContext, IUpdateContext updateContext);
    }
}

