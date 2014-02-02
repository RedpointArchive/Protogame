using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Protogame;
using System;
using Ninject;

namespace Protogame
{
    /// <summary>
    /// This is the local asset manager for ProtogameAssetManager.  Do not use it in your game!
    /// </summary>
    public class LocalAssetManagerProvider : IAssetManagerProvider
    {
        private LocalAssetManager m_AssetManager;

        public LocalAssetManagerProvider(
            IKernel kernel,
            IProfiler profiler,
            IRawAssetLoader rawLoader,
            IRawAssetSaver rawSaver,
            IAssetLoader[] loaders,
            IAssetSaver[] savers,
            ITransparentAssetCompiler transparentAssetCompiler)
        {
            this.m_AssetManager = new LocalAssetManager(
                kernel,
                profiler,
                rawLoader,
                rawSaver,
                loaders,
                savers,
                transparentAssetCompiler);
            this.m_AssetManager.AllowSourceOnly = true;
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

