using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtogameAssetTool
{
    using System.IO;
    using Protogame;

    class DefaultAssetOutOfDateCalculator : IAssetOutOfDateCalculator
    {
        private readonly ILoadStrategy[] m_LoadStrategies;

        private readonly IRawAssetLoader m_RawAssetLoader;

        public DefaultAssetOutOfDateCalculator(ILoadStrategy[] loadStrategies, IRawAssetLoader rawAssetLoader)
        {
            this.m_LoadStrategies = loadStrategies;
            this.m_RawAssetLoader = rawAssetLoader;
        }

        public string[] GetAssetsForRecompilation(string outputRootPath)
        {
            var results = new List<string>();

            foreach (var assetName in this.m_RawAssetLoader.ScanRawAssets())
            {
                var candidates = this.m_RawAssetLoader.LoadRawAssetCandidatesWithModificationDates(assetName).ToList();

                var source = candidates.Where(x => !(x.Key is CompiledAsset)).ToList();

                var compiledTargetPaths = new[]
                {
                    new FileInfo(
                        Path.Combine(outputRootPath, assetName.Replace('.', Path.DirectorySeparatorChar) + ".bin")),
                    new FileInfo(
                        Path.Combine(outputRootPath, assetName.Replace('.', Path.DirectorySeparatorChar) + ".asset"))
                };

                var compiled = (from file in compiledTargetPaths
                                where file.Exists
                                select new KeyValuePair<string, DateTime>(file.FullName, file.LastWriteTime)).ToList();

                if (source.Count == 1 && compiled.Count == 0)
                {
                    results.Add(assetName);
                }
                else if (source.Count == 1 && compiled.Count == 1)
                {
                    if (source.First().Value.Value > compiled.First().Value)
                    {
                        // Source version is later than the compiled version.
                        results.Add(assetName);
                    }
                }
                else
                {
                    Console.WriteLine("Skipping " + assetName + "; it has " + source.Count + " source files and " + compiled.Count + " compiled files:");
                    foreach (var src in source)
                    {
                        Console.WriteLine(" - (source) ");
                        foreach (var prop in src.Key.Properties)
                        {
                            try
                            {
                                Console.WriteLine("   - " + prop.Key + " = " + prop.Value);
                            }
                            catch { }
                        }
                    }
                    foreach (var cmp in compiled)
                    {
                        Console.WriteLine(" - (compiled) " + cmp.Key);
                    }
                }
            }

            return results.ToArray();
        }
    }
}
