using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetCompiler
    {
        string[] Extensions { get; }

        Task CompileAsync(IAssetFsFile assetFile, IAssetDependencies assetDependencies, TargetPlatform platform, IWritableSerializedAsset output);
    }
}
