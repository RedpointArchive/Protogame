namespace Protogame
{
    public interface IAssetCompiler<TAsset> where TAsset : IAsset
    {
        void Compile(TAsset asset, TargetPlatform platform);
    }
}
