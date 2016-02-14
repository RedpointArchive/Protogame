namespace Protogame
{
    /// <summary>
    /// An interface which represents an instance of an audio asset.
    /// </summary>
    /// <module>Audio</module>
    public interface IAudioHandle
    {
        /// <summary>
        /// Plays this audio instance in a loop.
        /// </summary>
        void Loop();

        /// <summary>
        /// Pauses playback of this audio instance.
        /// </summary>
        void Pause();

        /// <summary>
        /// Starts playing or resumes this audio instance once (not looped).
        /// </summary>
        void Play();

        /// <summary>
        /// Stops playback of this audio instance, resetting the playback
        /// position to the start of the audio instance.
        /// </summary>
        void Stop();
    }
}