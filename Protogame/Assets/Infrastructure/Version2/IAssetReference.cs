using System;

namespace Protogame
{
    public interface IAssetReference<out T> where T : class, IAsset
    {
        T Asset { get; }

        AssetReferenceState State { get; }

        bool IsReady { get; }
        
        Exception LoadingException { get; }
    }
}