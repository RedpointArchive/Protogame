namespace Protogame
{
    public class RuntimeCompiledAssetFs : CompiledAssetFs
    {
        public RuntimeCompiledAssetFs(IAssetFs assetFs, IAssetCompiler[] compilers) : base(assetFs, compilers, TargetPlatformUtility.GetExecutingPlatform())
        {
        }
    }
}
