using System;

namespace Protogame
{
    public class NullAudioUtilities : IAudioUtilities
    {
        public IAudioHandle GetHandle(AudioAsset asset)
        {
            throw new NotSupportedException();
        }

        public IAudioHandle Loop(AudioAsset asset)
        {
            throw new NotSupportedException();
        }

        public IAudioHandle Play(AudioAsset asset)
        {
            throw new NotSupportedException();
        }
    }
}