namespace Protogame
{
    /// <summary>
    /// The AudioUtilities interface.
    /// </summary>
    /// <module>Audio</module>
    public interface IAudioUtilities
    {
        /// <summary>
        /// Obtains an audio handle instance for an audio asset.  This instance
        /// can then be played, looped or stopped through the <see cref="IAudioHandle"/>
        /// interface.
        /// </summary>
        /// <param name="asset">
        /// The audio asset to obtain an instance handle for.
        /// </param>
        /// <returns>
        /// The <see cref="IAudioHandle"/> instance handle.
        /// </returns>
        IAudioHandle GetHandle(AudioAsset asset);

        /// <summary>
        /// Obtains an audio handle instance for an audio asset and starts playing
        /// it looped.  This instance can then be controlled through the <see cref="IAudioHandle"/>
        /// interface.
        /// </summary>
        /// <param name="asset">
        /// The audio asset to obtain an instance handle for.
        /// </param>
        /// <returns>
        /// The <see cref="IAudioHandle"/> instance handle.
        /// </returns>
        IAudioHandle Loop(AudioAsset asset);

        /// <summary>
        /// Obtains an audio handle instance for an audio asset and plays it once.
        /// This instance can then be controlled through the <see cref="IAudioHandle"/>
        /// interface.
        /// </summary>
        /// <param name="asset">
        /// The audio asset to obtain an instance handle for.
        /// </param>
        /// <returns>
        /// The <see cref="IAudioHandle"/> instance handle.
        /// </returns>
        IAudioHandle Play(AudioAsset asset);
    }
}