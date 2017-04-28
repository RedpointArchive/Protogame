using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Protogame
{
    public class BaseEffectAssetLoader
    {
        /// <summary>
        /// Resolves #include declarations in the file before the code is evaluated internally by MonoGame.
        /// </summary>
        /// <remarks>
        /// This resolves the #include declarations before the code is processed by MonoGame.  This is required
        /// because the code is stored as a temporary file before being processed, instead of being processed at
        /// the source (because once we get the code, we are not processing per-file but per-asset).
        /// </remarks>
        /// <param name="assetDependencies">The asset dependencies interface to get included files from.</param>
        /// <param name="dirName">The folder name that this asset resides in.</param>
        /// <param name="code">The code to process.</param>
        /// <returns>The resulting code.</returns>
        protected async Task<string> ResolveIncludes(IAssetDependencies assetDependencies, string dirName, string code)
        {
            var regex = new Regex("^#include <([^>]+)>\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            var matches = regex.Matches(code).OfType<Match>().ToList();

            foreach (var currentMatch in matches)
            {
                var relName = currentMatch.Groups[1].Captures[0].Value;
                var relComponents = relName.Split(new[] { '\\', '/' });
                var dirComponents = dirName.Split('.').ToList();
                for (var i = 0; i < relComponents.Length; i++)
                {
                    if (relComponents[i] == ".")
                    {
                        // Do nothing, same directory.
                    }
                    else if (relComponents[i] == "..")
                    {
                        // Parent directory.
                        if (dirComponents.Count > 0)
                        {
                            dirComponents.RemoveAt(dirComponents.Count - 1);
                        }
                    }
                    else
                    {
                        dirComponents.Add(relComponents[i]);
                    }
                }

                var includeFile = await assetDependencies.GetOptionalCompileTimeFileDependency(string.Join(".", dirComponents.ToArray())).ConfigureAwait(false);
                if (includeFile == null)
                {
                    throw new InvalidOperationException(
                        "Unable to include " + relName + "; resolved name " + dirComponents.ToArray() + " does not exist!");
                }

                // TODO: Calculate original filename from the PC that this asset was sourced from/
                var nameOnDisk = string.Join("\\", dirComponents.ToArray());

                using (var reader = new StreamReader(await includeFile.GetContentStream().ConfigureAwait(false)))
                {
                    var replace = reader.ReadToEnd();
                    replace = "#line 1 \"" + nameOnDisk + "\"\r\n" + replace;
                    replace = await this.ResolveIncludes(assetDependencies, Path.GetDirectoryName(includeFile.Name.Replace(".", "/")).Replace(Path.DirectorySeparatorChar, '.'), replace).ConfigureAwait(false);

                    code = code.Replace(currentMatch.Value, replace);
                }
            }

            return code;
        }
    }
}
