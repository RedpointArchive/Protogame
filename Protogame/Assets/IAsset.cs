namespace Protogame
{
    /// <summary>
    /// The Asset interface.
    /// </summary>
    public interface IAsset
    {
        /// <summary>
        /// Gets a value indicating whether compiled only.
        /// </summary>
        /// <value>
        /// The compiled only.
        /// </value>
        bool CompiledOnly { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether source only.
        /// </summary>
        /// <value>
        /// The source only.
        /// </value>
        bool SourceOnly { get; }

        /// <summary>
        /// The resolve.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Resolve<T>() where T : class, IAsset;
    }
}