namespace Protogame
{
    public interface IAudioUtilities
    {
        IAudioHandle GetHandle(AudioAsset asset);
        IAudioHandle Play(AudioAsset asset);
        IAudioHandle Loop(AudioAsset asset);
    }
}

