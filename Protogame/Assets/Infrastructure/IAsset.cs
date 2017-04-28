namespace Protogame
{
    /// <summary>
    /// The interface for all assets.
    /// <para>
    /// When you implement a new asset type, you need to implement this interface.
    /// </para>
    /// </summary>
    /// <module>Assets</module>
    public interface IAsset
    {
        /// <summary>
        /// Gets the name of the asset.
        /// </summary>
        /// <value>
        /// The name of the asset.
        /// </value>
        string Name { get; }
    }
}