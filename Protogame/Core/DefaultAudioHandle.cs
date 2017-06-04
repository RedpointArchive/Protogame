// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Microsoft.Xna.Framework.Audio;
using System;

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
        private SoundEffectInstance _instance;
        private WeakReference<AudioAsset> _lastResolvedAsset;
        private readonly IAssetReference<AudioAsset> _asset;

        public DefaultAudioHandle(IAssetReference<AudioAsset> asset)
        {
            _asset = asset;
            _lastResolvedAsset = new WeakReference<AudioAsset>(null);
        }

        private void UpdateInstance()
        {
            var shouldReload = false;
            AudioAsset lastRef = null;
            if (_lastResolvedAsset != null)
            {
                if (!_lastResolvedAsset.TryGetTarget(out lastRef))
                {
                    // The reference expired, which means the old asset was disposed
                    // and we need to reload.
                    shouldReload = true;
                }
                else
                {
                    if (_asset.IsReady)
                    {
                        if (lastRef != _asset.Asset)
                        {
                            // The asset is pointing at a different resolved instance now,
                            // so reload the instance.
                            shouldReload = true;
                        }
                    }
                }
            }
            if (_instance == null && _asset.IsReady)
            {
                // Not a real reload, just a load.
                shouldReload = true;
            }

            if (shouldReload)
            {
                if (_instance != null)
                {
                    _instance.Dispose();
                }

                if (_asset.IsReady)
                {
                    _instance = _asset.Asset.Audio.CreateInstance();
                    _lastResolvedAsset.SetTarget(_asset.Asset);
                }
                else
                {
                    _instance = null;
                }
            }
        }

        public void Loop()
        {
            UpdateInstance();

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
            UpdateInstance();

            _instance?.Pause();
        }

        public void Play()
        {
            UpdateInstance();

            _instance?.Play();
        }

        public void Stop(bool immediate)
        {
            _instance?.Stop(immediate);

            UpdateInstance();

            _instance?.Stop(immediate);
        }

        public float Volume
        {
            get
            {
                UpdateInstance();

                if (_instance == null)
                {
                    return 0;
                }

                return _instance.Volume;
            }
            set
            {
                UpdateInstance();

                if (_instance == null)
                {
                    return;
                }

                _instance.Volume = value;
            }
        }

        public bool IsPlaying
        {
            get
            {
                UpdateInstance();

                return _instance?.State == SoundState.Playing;
            }
        }
    }
}