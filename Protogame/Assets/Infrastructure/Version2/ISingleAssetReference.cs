using System;

namespace Protogame
{
    public interface ISingleAssetReference<out T> : IAssetReference<T> where T : class, IAsset
    {
        string Name { get; }

        void Update(IAsset asset, AssetReferenceState assetState);

        void Update(AssetReferenceState assetState);

        void Update(Exception exception);
    }
}
