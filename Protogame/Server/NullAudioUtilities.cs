using System;

namespace Protogame
{
    public class NullAudioUtilities : IAudioUtilities
    {
        public IAudioHandle GetHandle(IAssetReference<AudioAsset> asset)
        {
            throw new NotSupportedException();
        }

        public IAudioHandle Loop(IAssetReference<AudioAsset> asset)
        {
            throw new NotSupportedException();
        }

        public IAudioHandle Play(IAssetReference<AudioAsset> asset)
        {
            throw new NotSupportedException();
        }
    }
}