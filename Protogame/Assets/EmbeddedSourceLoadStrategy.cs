namespace Protogame
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// The embedded source load strategy.
    /// </summary>
    public class EmbeddedSourceLoadStrategy : ILoadStrategy
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
                return false;
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
            var embedded = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                            where !assembly.IsDynamic
                            from resource in assembly.GetManifestResourceNames()
                            where resource == assembly.GetName().Name + "." + name + ".asset"
                            select assembly.GetManifestResourceStream(resource)).ToList();
            if (embedded.Any())
            {
                using (var reader = new StreamReader(embedded.First(), Encoding.UTF8))
                {
                    return JsonConvert.DeserializeObject<dynamic>(reader.ReadToEnd());
                }
            }

            return null;
        }
    }
}