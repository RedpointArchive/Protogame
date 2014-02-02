namespace Protogame
{
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
        public object AttemptLoad(string path, string name)
        {
            var file = new FileInfo(Path.Combine(path, name.Replace('.', Path.DirectorySeparatorChar) + ".asset"));
            if (file.Exists)
            {
                using (var reader = new StreamReader(file.FullName, Encoding.UTF8))
                {
                    return JsonConvert.DeserializeObject<dynamic>(reader.ReadToEnd());
                }
            }

            return null;
        }
    }
}