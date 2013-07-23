using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Protogame;
using System;

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
            this.m_AssetManager = new LocalAssetManager(
                rawLoader,
                rawSaver,
                loaders,
                savers);
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

