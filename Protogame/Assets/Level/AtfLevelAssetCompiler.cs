using System.IO;
using System.Threading.Tasks;

namespace Protogame
{
    public class AtfLevelAssetCompiler : IAssetCompiler
    {
        public string[] Extensions => new[] { "lvl" };

        public async Task CompileAsync(IAssetFsFile assetFile, IAssetDependencies assetDependencies, TargetPlatform platform, IWritableSerializedAsset output)
        {
            string level;
            using (var reader = new StreamReader(await assetFile.GetContentStream().ConfigureAwait(false)))
            {
                level = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            output.SetLoader<IAssetLoader<LevelAsset>>();
            output.SetString("LevelData", level);
            output.SetString("LevelDataFormat", "ATF");
        }
    }
}
