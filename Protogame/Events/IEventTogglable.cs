namespace Protogame
{
    /// <summary>
    /// The EventTogglable interface.
    /// </summary>
    public interface IEventTogglable
    {
        /// <summary>
        /// The toggle.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        void Toggle(string id);
    }
}