#if FALSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Protogame;
using System.Threading.Tasks;


namespace ProtogameAssetTool
{
    public class DefaultAssetCleanup : IAssetCleanup
    {
        private readonly IAssetFs _assetFs;

        public DefaultAssetCleanup(IAssetFs assetFs)
        {
            this._assetFs = assetFs;
        }

        public async Task Cleanup(string outputRootPath)
        {
            var assetNames = (await _assetFs.List()).Select(x => x.Name);

            var compiledAssetNames = this.FindAllCompiledAssets(outputRootPath);

            foreach (var missingSource in compiledAssetNames.Where(x => !assetNames.Contains(x)))
            {
                Console.WriteLine("Deleting compiled asset " + missingSource + " which no longer has a source file");

                var compiledTargetPaths = new[]
                {
                    new FileInfo(
                        Path.Combine(outputRootPath, missingSource.Replace('.', Path.DirectorySeparatorChar) + ".bin"))
                };

                foreach (var file in compiledTargetPaths)
                {
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
            }
        }
        
        private IEnumerable<string> FindAllCompiledAssets(string outputRootPath, string prefix = "")
        {
            var dirInfo = new DirectoryInfo(outputRootPath);
            
            foreach (var file in dirInfo.GetFiles("*.bin"))
            {
                yield return prefix + file.Name.Substring(0, file.Name.Length - file.Extension.Length);
            }

            foreach (var dir in dirInfo.GetDirectories())
            {
                foreach (var file in this.FindAllCompiledAssets(dir.FullName, prefix + dir.Name + "."))
                {
                    yield return file;
                }
            }
        }
    }
}

#endif