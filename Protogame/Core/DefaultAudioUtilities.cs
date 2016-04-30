namespace Protogame
{
    /// <summary>
    /// The default audio utilities.
    /// </summary>
    public class DefaultAudioUtilities : IAudioUtilities
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
        public IAudioHandle GetHandle(AudioAsset asset)
        {
            return new DefaultAudioHandle(asset);
        }

        /// <summary>
        /// The loop.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <returns>
        /// The <see cref="IAudioHandle"/>.
        /// </returns>
        public IAudioHandle Loop(AudioAsset asset)
        {
            var handle = this.GetHandle(asset);
            handle.Loop();
            return handle;
        }

        /// <summary>
        /// The play.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <returns>
        /// The <see cref="IAudioHandle"/>.
        /// </returns>
        public IAudioHandle Play(AudioAsset asset)
        {
            var handle = this.GetHandle(asset);
            handle.Play();
            return handle;
        }
    }
}