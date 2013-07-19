using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Protogame;

namespace ProtogameAssetManager
{
    public class LocalAssetManagerProvider : IAssetManagerProvider
    {
        private LocalAssetManager m_AssetManager;

        public LocalAssetManagerProvider(
            IRawAssetLoader rawLoader,
            IRawAssetSaver rawSaver,
            IEnumerable<IAssetLoader> loaders,
            IEnumerable<IAssetSaver> savers)
        {
            var file = new FileInfo(Assembly.GetExecutingAssembly().Location);
            this.m_AssetManager = new LocalAssetManager(
                rawLoader,
                rawSaver,
                loaders,
                savers,
                Path.Combine(file.Directory.FullName, "Content"));
        }

        public bool IsReady
        {
            get
            {
                return true;
            }
        }

        public IAssetManager GetAssetManager(bool permitCreate)
        {
            return this.m_AssetManager;
        }
    }
}

