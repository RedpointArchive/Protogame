namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// The local source load strategy.
    /// </summary>
    public class LocalSourceLoadStrategy : ILoadStrategy
    {
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
                return new[] { "asset" };
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
            var file1 = new FileInfo(Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".asset"));
            var attempt1 = this.AttemptLoadOfFile(file1);
            if (attempt1 != null)
            {
                lastModified = file1.LastWriteTime;
                return attempt1;
            }

            var file2 = new FileInfo(Path.Combine(
                path,
                TargetPlatformUtility.GetExecutingPlatform().ToString(),
                (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".asset"));
            var attempt2 = this.AttemptLoadOfFile(file2);
            if (attempt2 != null)
            {
                lastModified = file2.LastWriteTime;
                return attempt2;
            }

            return null;
        }

        private DictionaryBasedRawAsset AttemptLoadOfFile(FileInfo file)
        {
            if (file.Exists)
            {
                using (var reader = new StreamReader(file.FullName, Encoding.UTF8))
                {
                    return
                        new DictionaryBasedRawAsset(
                            JsonConvert.DeserializeObject<Dictionary<string, object>>(reader.ReadToEnd()));
                }
            }

            return null;
        }

        public System.Collections.Generic.IEnumerable<string> GetPotentialPaths(string path, string name, bool noTranslate = false)
        {
            yield return Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".asset");
        }
    }
}