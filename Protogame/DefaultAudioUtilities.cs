namespace Protogame
{
    public class DefaultAudioUtilities : IAudioUtilities
    {
        public IAudioHandle GetHandle(AudioAsset asset)
        {
            return new DefaultAudioHandle(asset);
        }
        
        public IAudioHandle Play(AudioAsset asset)
        {
            var handle = this.GetHandle(asset);
            handle.Play();
            return handle;
        }
        
        public IAudioHandle Loop(AudioAsset asset)
        {
            var handle = this.GetHandle(asset);
            handle.Loop();
            return handle;
        }
    }
}

