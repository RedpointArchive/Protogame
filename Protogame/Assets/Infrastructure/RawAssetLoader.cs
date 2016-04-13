namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The raw asset loader.
    /// </summary>
    public class RawAssetLoader : IRawAssetLoader
    {
        /// <summary>
        /// The m_ path.
        /// </summary>
        private readonly string m_Path;

        /// <summary>
        /// The m_ source path.
        /// </summary>
        private readonly string m_SourcePath;

        /// <summary>
        /// The m_ strategies.
        /// </summary>
        private readonly ILoadStrategy[] m_Strategies;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawAssetLoader"/> class.
        /// </summary>
        /// <param name="strategies">
        /// The strategies.
        /// </param>
        public RawAssetLoader(ILoadStrategy[] strategies)
        {
            var directory = new DirectoryInfo(Environment.CurrentDirectory).FullName;
            var contentDirectory = Path.Combine(directory, "Content");
            if (Directory.Exists(contentDirectory))
            {
                Directory.CreateDirectory(directory);
                directory = contentDirectory;
            }

            this.m_Path = directory;
            this.m_SourcePath = null;

            // Check for the existance of a .source file in the directory; if so, we
            // also search that path.
            if (File.Exists(Path.Combine(this.m_Path, ".source")))
            {
                using (var reader = new StreamReader(Path.Combine(this.m_Path, ".source")))
                {
                    this.m_SourcePath = reader.ReadLine();
                    
                    // Don't scan twice if the source path is the same as the normal path.
                    if (string.Equals(this.m_SourcePath, this.m_Path, StringComparison.OrdinalIgnoreCase))
                    {
                        this.m_SourcePath = null;
                    }
                }
            }

            this.m_Strategies = strategies;
        }

        /// <summary>
        /// Loads all potential raw asset candidates for the given asset name.  It is up to the
        /// caller to determine which representation will be used.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The raw asset candidates.</returns>
        public IEnumerable<IRawAsset> LoadRawAssetCandidates(string name)
        {
            return this.LoadRawAssetCandidatesWithModificationDates(name).Select(x => x.Key);
        }

        /// <summary>
        /// Loads all potential raw asset candidates for the given asset name, including the last
        /// modification dates of the candidates.  It is up to the caller to determine which
        /// representation will be used.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The raw asset candidates.</returns>
        public IEnumerable<KeyValuePair<IRawAsset, DateTime?>> LoadRawAssetCandidatesWithModificationDates(string name)
        {
            foreach (var strategy in this.m_Strategies)
            {
                IRawAsset result;

                if (strategy.ScanSourcePath && this.m_SourcePath != null)
                {
                    DateTime? lastModified = null;
                    result = strategy.AttemptLoad(this.m_SourcePath, name, ref lastModified);
                    if (result != null)
                    {
                        yield return new KeyValuePair<IRawAsset, DateTime?>(result, lastModified);
                    }
                }

                {
                    DateTime? lastModified = null;
                    result = strategy.AttemptLoad(this.m_Path, name, ref lastModified);
                    if (result != null)
                    {
                        yield return new KeyValuePair<IRawAsset, DateTime?>(result, lastModified);
                    }
                }
            }
        }

        public IEnumerable<string> GetPotentialPathsForRawAsset(string name)
        {
            foreach (var strategy in this.m_Strategies)
            {
                IEnumerable<string> result;

                if (strategy.ScanSourcePath && this.m_SourcePath != null)
                {
                    result = strategy.GetPotentialPaths(this.m_SourcePath, name);
                    foreach (var r in result)
                    {
                        yield return r;
                    }
                }

                {
                    result = strategy.GetPotentialPaths(this.m_Path, name);
                    foreach (var r in result)
                    {
                        yield return r;
                    }
                }
            }
        }

        /// <summary>
        /// The rescan assets.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="prefixes">
        /// The prefixes.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<string> RescanAssets(string path, string prefixes = "")
        {
            var directoryInfo = new DirectoryInfo(path + "/" + prefixes.Replace('.', '/'));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                // Skip any files with an - symbol in the name; this usually indicates
                // a "subfile" that the load strategy will pick up (for example when there's
                // multiple source FBX files for an animated model).
                if (file.Name.Contains("-"))
                {
                    continue;
                }

                foreach (var extension in this.GetExtensions())
                {
                    if (file.Extension != extension)
                    {
                        continue;
                    }

                    var name = file.Name.Substring(0, file.Name.Length - extension.Length);
                    var asset = (prefixes.Trim('.') + "." + name).Trim('.');

                    // If the asset is now prefixed with a platform (e.g. "Linux.") then we
                    // should remove it as this is just an indication that it's probably a
                    // compiled asset.
                    asset = this.StripPlatformPrefix(asset);

                    yield return asset;
                }
            }

            foreach (var directory in directoryInfo.GetDirectories())
            {
                foreach (var asset in this.RescanAssets(path, prefixes + directory.Name + "."))
                {
                    yield return asset;
                }
            }
        }

        /// <summary>
        /// Return all available asset names.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> of asset names.
        /// </returns>
        public IEnumerable<string> ScanRawAssets()
        {
            return this.RescanAssets(this.m_Path);
        }

        /// <summary>
        /// The get extensions.
        /// </summary>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private string[] GetExtensions()
        {
            return this.m_Strategies.SelectMany(x => x.AssetExtensions).Select(x => "." + x).ToArray();
        }

        /// <summary>
        /// The strip platform prefix.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string StripPlatformPrefix(string name)
        {
            foreach (var value in Enum.GetNames(typeof(TargetPlatform)))
            {
                if (name.StartsWith(value + "."))
                {
                    return name.Substring(value.Length + 1);
                }
            }

            return name;
        }
    }
}