namespace Protogame
{
    /// <summary>
    /// The LoadStrategy interface.
    /// </summary>
    public interface ILoadStrategy
    {
        /// <summary>
        /// Gets the asset extensions.
        /// </summary>
        /// <value>
        /// The asset extensions.
        /// </value>
        string[] AssetExtensions { get; }

        /// <summary>
        /// Gets a value indicating whether scan source path.
        /// </summary>
        /// <value>
        /// The scan source path.
        /// </value>
        bool ScanSourcePath { get; }

        /// <summary>
        /// The attempt load.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object AttemptLoad(string path, string name);
    }
}