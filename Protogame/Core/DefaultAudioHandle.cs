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
        
        public DefaultAudioHandle(IAssetReference<AudioAsset> asset)
        {
            if (!asset.IsReady)
            {
                _instance = null;
            }
            else
            {
                _instance = asset.Asset.Audio.CreateInstance();
            }
        }
        
        public void Loop()
        {
            if (_instance == null)
            {
                return;
            }

            _instance.IsLooped = true;
            if (_instance.State != SoundState.Playing)
            {
                _instance.Play();
            }
        }
        
        public void Pause()
        {
            _instance?.Pause();
        }
        
        public void Play()
        {
            _instance?.Play();
        }
        
        public void Stop(bool immediate)
        {
            _instance?.Stop(immediate);
        }

        public float Volume
        {
            get
            {
                if (_instance == null)
                {
                    return 0;
                }

                return _instance.Volume;
            }
            set
            {
                if (_instance == null)
                {
                    return;
                }

                _instance.Volume = value;
            }
        }

        public bool IsPlaying => _instance?.State == SoundState.Playing;
    }
}