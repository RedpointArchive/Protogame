namespace Protogame
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The raw effect load strategy.
    /// </summary>
    public class RawEffectLoadStrategy : ILoadStrategy
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
                return new[] { "fx", "usl" };
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
            foreach (var ext in AssetExtensions)
            {
                var file =
                    new FileInfo(Path.Combine(path,
                        (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + "." + ext));
                if (file.Exists)
                {
                    lastModified = file.LastWriteTime;
                    using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(fileStream))
                        {
                            var code = reader.ReadToEnd();

                            if (file.Directory != null)
                            {
                                code = this.ResolveIncludes(file.Directory, code);
                            }

                            return
                                new AnonymousObjectBasedRawAsset(
                                    new
                                    {
                                        Loader = ext == "fx" ? typeof (EffectAssetLoader).FullName : typeof(UnifiedShaderAssetLoader).FullName,
                                        PlatformData = (PlatformData) null,
                                        Code = code,
                                        SourcedFromRaw = true
                                    });
                        }
                    }
                }
            }

            return null;
        }

        public System.Collections.Generic.IEnumerable<string> GetPotentialPaths(string path, string name, bool noTranslate = false)
        {
            yield return Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".fx");
            yield return Path.Combine(path, (noTranslate ? name : name.Replace('.', Path.DirectorySeparatorChar)) + ".usl");
        }

        /// <summary>
        /// Resolves #include declarations in the file before the code is evaluated internally by MonoGame.
        /// </summary>
        /// <remarks>
        /// This resolves the #include declarations before the code is processed by MonoGame.  This is required
        /// because the code is stored as a temporary file before being processed, instead of being processed at
        /// the source (because once we get the code, we are not processing per-file but per-asset).
        /// </remarks>
        /// <param name="directory">The current directory.</param>
        /// <param name="code">The code to process.</param>
        /// <returns>The resulting code.</returns>
        private string ResolveIncludes(DirectoryInfo directory, string code)
        {
            var regex = new Regex("^#include <([^>]+)>\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            var matches = regex.Matches(code).OfType<Match>().ToList();

            foreach (var currentMatch in matches)
            {
                var relName = currentMatch.Groups[1].Captures[0].Value;
                var fullName = Path.Combine(directory.FullName, relName);
                if (!File.Exists(fullName))
                {
                    throw new InvalidOperationException(
                        "Unable to include " + relName + "; resolved name " + fullName + " does not exist!");
                }

                using (var reader = new StreamReader(fullName))
                {
                    var replace = reader.ReadToEnd();
                    replace = this.ResolveIncludes(new FileInfo(fullName).Directory, replace);

                    code = code.Replace(currentMatch.Value, replace);
                }
            }

            return code;
        }
    }
}