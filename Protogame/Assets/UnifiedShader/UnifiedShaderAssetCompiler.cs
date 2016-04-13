namespace Protogame
{
    public class UnifiedShaderAssetCompiler : IAssetCompiler<UnifiedShaderAsset>
    {
        public void Compile(UnifiedShaderAsset asset, TargetPlatform platform)
        {
            var info = new UnifiedShaderParser().Parse(asset.Code);

            // TODO
        }
    }
}
