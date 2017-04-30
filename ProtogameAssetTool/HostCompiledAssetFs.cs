using Protogame;
using System;

namespace ProtogameAssetTool
{
    public class HostCompiledAssetFs : CompiledAssetFs
    {
        public HostCompiledAssetFs(IAssetFs assetFs, IAssetCompiler[] compilers, TargetPlatform targetPlatform) : base(assetFs, compilers, targetPlatform)
        {
        }

        protected override void OnCompileStart(IAssetFsFile assetFile, TargetPlatform targetPlatform)
        {
            Console.WriteLine("Compiling " + assetFile.Name + " for " + targetPlatform.ToString() + "...");
        }

        protected override void OnCompilerMissing(IAssetFsFile assetFile, TargetPlatform targetPlatform)
        {
            Console.WriteLine("No compiler available for " + assetFile.Name);
        }

        protected override void OnCompileFinish(IAssetFsFile assetFile, IAssetFsFile compiledAssetFile, TargetPlatform targetPlatform)
        {
            Console.WriteLine("Compiled " + assetFile.Name + " for " + targetPlatform.ToString());
        }
    }
}
