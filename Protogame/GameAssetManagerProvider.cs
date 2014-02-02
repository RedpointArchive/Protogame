using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Protogame;
using System;
using Ninject;

namespace Protogame
{
    public class GameAssetManagerProvider : IAssetManagerProvider
    {
        private LocalAssetManager m_AssetManager;

        public GameAssetManagerProvider(
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

