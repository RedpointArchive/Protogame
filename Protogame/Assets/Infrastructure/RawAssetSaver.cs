namespace Protogame
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// The raw asset saver.
    /// </summary>
    public class RawAssetSaver : IRawAssetSaver
    {
        /// <summary>
        /// The m_ path.
        /// </summary>
        private readonly string m_Path;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawAssetSaver"/> class.
        /// </summary>
        public RawAssetSaver()
        {
            var directory = new DirectoryInfo(Environment.CurrentDirectory).FullName;
            var contentDirectory = Path.Combine(directory, "Content");
            if (Directory.Exists(contentDirectory))
            {
                Directory.CreateDirectory(directory);
                directory = contentDirectory;
            }

            this.m_Path = directory;

            // Check for the existance of a .source file in the directory; if so, we
            // set our path to that directory instead.
            if (File.Exists(Path.Combine(this.m_Path, ".source")))
            {
                using (var reader = new StreamReader(Path.Combine(this.m_Path, ".source")))
                {
                    this.m_Path = reader.ReadLine();
                }
            }
        }

        /// <summary>
        /// The save raw asset.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <exception cref="AssetNotFoundException">
        /// </exception>
        public void SaveRawAsset(string name, IRawAsset data)
        {
            try
            {
                var file =
                    new FileInfo(Path.Combine(this.m_Path, name.Replace('.', Path.DirectorySeparatorChar) + ".asset"));
                this.CreateDirectories(file.Directory);
                using (var writer = new StreamWriter(file.FullName, false, Encoding.UTF8))
                {
                    writer.Write(JsonConvert.SerializeObject(data.Properties.ToDictionary(k => k.Key, v => v.Value)));
                }
            }
            catch (Exception ex)
            {
                throw new AssetNotFoundException(name, ex);
            }
        }

        /// <summary>
        /// The create directories.
        /// </summary>
        /// <param name="directory">
        /// The directory.
        /// </param>
        private void CreateDirectories(DirectoryInfo directory)
        {
            if (directory.Exists)
            {
                return;
            }

            this.CreateDirectories(directory.Parent);
            directory.Create();
        }
    }
}