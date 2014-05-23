namespace Protogame
{
    using System;
    using System.Linq;

    /// <summary>
    /// A load strategy that supports loading classes as runtime assets.
    /// </summary>
    public class AssemblyLoadStrategy : ILoadStrategy
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
                return new string[0];
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
        public IRawAsset AttemptLoad(string path, string name, ref DateTime? lastModified, bool noTranslate = false)
        {
            lastModified = new DateTime(1970, 1, 1, 0, 0, 0);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (typeof(IAsset).IsAssignableFrom(type))
                        {
                            var attribute =
                                type.GetCustomAttributes(false)
                                    .Cast<Attribute>()
                                    .OfType<AssetAttribute>()
                                    .FirstOrDefault();
                            if (attribute != null)
                            {
                                if (attribute.Name == name)
                                {
                                    return new TypeBasedRawAsset(name, type);
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return null;
        }
    }
}