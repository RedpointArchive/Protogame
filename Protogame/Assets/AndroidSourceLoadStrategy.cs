#if PLATFORM_ANDROID || PLATFORM_OUYA
namespace Protogame
{
    using System;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// The Android source load strategy.
    /// </summary>
    public class AndroidSourceLoadStrategy : ILoadStrategy
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
        public object AttemptLoad(string path, string name, ref DateTime? lastModified)
        {
            try
            {
                var stream = global::Android.App.Application.Context.Assets.Open(
                    TargetPlatformUtility.GetExecutingPlatform().ToString() + Path.DirectorySeparatorChar +
                    name.Replace('.', Path.DirectorySeparatorChar) + ".asset");
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return JsonConvert.DeserializeObject<dynamic>(reader.ReadToEnd());
                }
            }
            catch (Java.IO.FileNotFoundException)
            {
                try
                {
                    var stream = global::Android.App.Application.Context.Assets.Open(
                        name.Replace('.', Path.DirectorySeparatorChar) + ".asset");
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return JsonConvert.DeserializeObject<dynamic>(reader.ReadToEnd());
                    }
                }
                catch (Java.IO.FileNotFoundException)
                {
                    return null;
                }
            }
        }
    }
}
#endif