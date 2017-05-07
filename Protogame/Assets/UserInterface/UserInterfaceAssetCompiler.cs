using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Protogame
{
    public class UserInterfaceAssetCompiler : IAssetCompiler
    {
        public string[] Extensions => new[] { "ui2" };

        public async Task CompileAsync(IAssetFsFile assetFile, IAssetDependencies assetDependencies, TargetPlatform platform, IWritableSerializedAsset output)
        {
            using (var reader = new StreamReader(await assetFile.GetContentStream().ConfigureAwait(false)))
            {
                var content = await reader.ReadToEndAsync().ConfigureAwait(false);
                
                // Validate that the XML is valid so we don't throw exceptions at runtime.
                var document = new XmlDocument();
                document.LoadXml(content);

                output.SetLoader<IAssetLoader<UserInterfaceAsset>>();
                output.SetString("UserInterfaceFormat", "XmlVersion2");
                output.SetString("UserInterfaceData", content);
            }
        }
    }
}