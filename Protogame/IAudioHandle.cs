namespace Protogame
{
    /// <summary>
    /// The AudioHandle interface.
    /// </summary>
    public interface IAudioHandle
    {
        /// <summary>
        /// The loop.
        /// </summary>
        void Loop();

        /// <summary>
        /// The pause.
        /// </summary>
        void Pause();

        /// <summary>
        /// The play.
        /// </summary>
        void Play();

        /// <summary>
        /// The stop.
        /// </summary>
        void Stop();
    }
}