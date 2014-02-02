namespace Protogame
{
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// The default audio handle.
    /// </summary>
    public class DefaultAudioHandle : IAudioHandle
    {
        /// <summary>
        /// The m_ instance.
        /// </summary>
        private readonly SoundEffectInstance m_Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAudioHandle"/> class.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        public DefaultAudioHandle(AudioAsset asset)
        {
            this.m_Instance = asset.Audio.CreateInstance();
        }

        /// <summary>
        /// The loop.
        /// </summary>
        public void Loop()
        {
            this.m_Instance.IsLooped = true;
            if (this.m_Instance.State == SoundState.Playing)
            {
                this.m_Instance.Play();
            }
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public void Pause()
        {
            this.m_Instance.Pause();
        }

        /// <summary>
        /// The play.
        /// </summary>
        public void Play()
        {
            this.m_Instance.Play();
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.m_Instance.Stop();
        }
    }
}