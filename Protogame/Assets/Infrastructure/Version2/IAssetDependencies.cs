using System.Threading.Tasks;

namespace Protogame
{
    public interface IAssetDependencies
    {
        Task<IAssetFsFile> GetRequiredCompileTimeFileDependency(string name);

        Task<IAssetFsFile> GetOptionalCompileTimeFileDependency(string name);

        Task<IAssetFsFile[]> GetAvailableCompileTimeFiles();

        Task<IReadableSerializedAsset> GetRequiredCompileTimeCompiledDependency(string name);

        Task<IReadableSerializedAsset> GetOptionalCompileTimeCompiledDependency(string name);
    }
}
