// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Microsoft.Xna.Framework.Audio;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IAudioHandle"/>.
    /// </summary>
    /// <module>Audio</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IAudioHandle</interface_ref>
    public class DefaultAudioHandle : IAudioHandle
    {
        private readonly SoundEffectInstance _instance;
        
        public DefaultAudioHandle(AudioAsset asset)
        {
            _instance = asset.Audio.CreateInstance();
        }
        
        public void Loop()
        {
            _instance.IsLooped = true;
            if (_instance.State == SoundState.Playing)
            {
                _instance.Play();
            }
        }
        
        public void Pause()
        {
            _instance.Pause();
        }
        
        public void Play()
        {
            _instance.Play();
        }
        
        public void Stop()
        {
            _instance.Stop();
        }
    }
}