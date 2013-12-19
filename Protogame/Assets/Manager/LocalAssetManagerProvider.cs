using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Protogame;
using System;
using Ninject;

namespace Protogame
{
    public class LocalAssetManagerProvider : IAssetManagerProvider
    {
        private LocalAssetManager m_AssetManager;

        public LocalAssetManagerProvider(
            IKernel kernel,
            IRawAssetLoader rawLoader,
            IRawAssetSaver rawSaver,
            IAssetLoader[] loaders,
            IAssetSaver[] savers,
            ITransparentAssetCompiler transparentAssetCompiler)
        {
            this.m_AssetManager = new LocalAssetManager(
                kernel,
                rawLoader,
                rawSaver,
                loaders,
                savers,
                transparentAssetCompiler);
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

