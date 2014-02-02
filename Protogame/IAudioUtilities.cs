namespace Protogame
{
    /// <summary>
    /// The AudioUtilities interface.
    /// </summary>
    public interface IAudioUtilities
    {
        /// <summary>
        /// The get handle.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <returns>
        /// The <see cref="IAudioHandle"/>.
        /// </returns>
        IAudioHandle GetHandle(AudioAsset asset);

        /// <summary>
        /// The loop.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <returns>
        /// The <see cref="IAudioHandle"/>.
        /// </returns>
        IAudioHandle Loop(AudioAsset asset);

        /// <summary>
        /// The play.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <returns>
        /// The <see cref="IAudioHandle"/>.
        /// </returns>
        IAudioHandle Play(AudioAsset asset);
    }
}