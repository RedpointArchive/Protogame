using Microsoft.Xna.Framework.Audio;

namespace Protogame
{
    public class DefaultAudioHandle : IAudioHandle
    {
        private SoundEffectInstance m_Instance;
        
        public DefaultAudioHandle(AudioAsset asset)
        {
            this.m_Instance = asset.Audio.CreateInstance();
        }
        
        public void Play()
        {
            this.m_Instance.Play();
        }

        public void Pause()
        {
            this.m_Instance.Pause();
        }

        public void Loop()
        {
            this.m_Instance.IsLooped = true;
            if (this.m_Instance.State == SoundState.Playing)
            {
                this.m_Instance.Play();
            }
        }

        public void Stop()
        {
            this.m_Instance.Stop();
        }
    }
}

