namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The raw model load strategy.
    /// </summary>
    public class RawModelLoadStrategy : ILoadStrategy
    {
        private readonly Dictionary<string, bool> _supportedAnimationFormats = new Dictionary<string, bool>
            {
                { "fbx", true },
                { "x", false },
                { "dae", true },
            };

        /// <summary>
        /// Gets the asset extensions.
        /// </summary>
        /// <value>
        /// The asset extensions.
        /// </value>
        public string[] AssetExtensions
        {
            get
            {
                return _supportedAnimationFormats.Keys.ToArray();
            }
        }

        /// <summary>
        /// Gets a value indicating whether scan source path.
        /// </summary>
        /// <value>
        /// The scan source path.
        /// </value>
        public bool ScanSourcePath
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The attempt load.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public IRawAsset AttemptLoad(string path, string name, ref DateTime? lastModified, bool noTranslate = false)
        {
            FileInfo file = null;

            foreach (var kv in _supportedAnimationFormats)
            {
                file =
                    new FileInfo(Path.Combine(path,
                        (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + "." + kv.Key));

                if (file.Exists)
                {
                    lastModified = file.LastWriteTime;

                    var otherAnimations = new Dictionary<string, byte[]>();
                    if (kv.Value)
                    {
                        // If there are name-anim.fbx files, read those in as additional animations.
                        var directory = file.Directory;
                        if (directory != null)
                        {
                            var lastComponent = name.Split('.').Last();
                            var otherAnimationFiles = directory.GetFiles(lastComponent + "-*." + kv.Key);
                            otherAnimations =
                                otherAnimationFiles.ToDictionary(
                                    key => key.Name.Split('-').Last().Split('.').First(),
                                    value => this.ReadModelData(value.FullName));
                        }
                    }

                    var folderPath = Path.GetDirectoryName(
                        new FileInfo(Path.Combine(path,
                            (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".txt")).FullName);
                    var folderOptionsFile = new FileInfo(Path.Combine(folderPath, "_FolderOptions.txt"));
                    string[] importFolderOptions = null;
                    if (folderOptionsFile.Exists)
                    {
                        using (var reader = new StreamReader(folderOptionsFile.FullName))
                        {
                            importFolderOptions = reader.ReadToEnd()
                                .Trim()
                                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .Where(x => !x.StartsWith("#"))
                                .ToArray();
                        }
                    }

                    var optionsFile =
                        new FileInfo(Path.Combine(path,
                            (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".txt"));
                    string[] importOptions = null;
                    if (optionsFile.Exists)
                    {
                        using (var reader = new StreamReader(optionsFile.FullName))
                        {
                            importOptions = reader.ReadToEnd()
                                .Trim()
                                .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .Where(x => !x.StartsWith("#"))
                                .ToArray();
                        }
                    }

                    if (importOptions == null)
                    {
                        importOptions = importFolderOptions;
                    }

                    return
                        new AnonymousObjectBasedRawAsset(
                            new
                            {
                                Loader = typeof (ModelAssetLoader).FullName,
                                PlatformData = (PlatformData) null,
                                RawData = this.ReadModelData(file.FullName),
                                RawAdditionalAnimations = otherAnimations,
                                SourcedFromRaw = true,
                                Extension = kv.Key,
                                ImportOptions = importOptions
                            });
                }
            }

            return null;
        }

        public System.Collections.Generic.IEnumerable<string> GetPotentialPaths(string path, string name, bool noTranslate = false)
        {
            foreach (var ext in _supportedAnimationFormats.Keys)
            {
                yield return
                    Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + "." + ext);
            }
        }

        /// <summary>
        /// The read model data.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private byte[] ReadModelData(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var binary = new BinaryReader(fileStream))
                {
                    return binary.ReadBytes((int)fileStream.Length);
                }
            }
        }
    }
}