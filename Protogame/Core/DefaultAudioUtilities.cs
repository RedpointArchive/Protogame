// ReSharper disable CheckNamespace
#pragma warning disable 1591

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IAudioUtilities"/>.
    /// </summary>
    /// <module>Audio</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IAudioUtilities</interface_ref>
    public class DefaultAudioUtilities : IAudioUtilities
    {
        public IAudioHandle GetHandle(AudioAsset asset)
        {
            return new DefaultAudioHandle(asset);
        }
        
        public IAudioHandle Loop(AudioAsset asset)
        {
            var handle = GetHandle(asset);
            handle.Loop();
            return handle;
        }
        
        public IAudioHandle Play(AudioAsset asset)
        {
            var handle = GetHandle(asset);
            handle.Play();
            return handle;
        }
    }
}