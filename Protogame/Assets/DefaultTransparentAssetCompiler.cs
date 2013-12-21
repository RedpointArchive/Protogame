using System;
using Ninject;

namespace Protogame
{
    public class DefaultTransparentAssetCompiler : ITransparentAssetCompiler
    {
        private IKernel m_Kernel;

        public DefaultTransparentAssetCompiler(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

        public IAsset Handle(IAsset asset, bool force = false)
        {
            return this.HandlePlatform(asset, TargetPlatformUtility.GetExecutingPlatform(), force);
        }

        public IAsset HandlePlatform(IAsset asset, TargetPlatform platform, bool force = false)
        {
            if (asset.SourceOnly || force)
            {
                var compilerType = typeof(IAssetCompiler<>).MakeGenericType(asset.GetType());
                var compiler = this.m_Kernel.TryGet(compilerType);
                if (compiler == null)
                {
                    // The caller will throw AssetNotCompiledException if all of the candidates
                    // for loading only have source information.
                    return asset;
                }
                var proxyType = typeof(AssetCompilerProxy<>).MakeGenericType(asset.GetType());
                var proxy = (IAssetCompilerProxyInterface)
                    proxyType.GetConstructor(new Type[] { compilerType }).Invoke(new object[] { compiler });
                proxy.Compile(asset, platform);
            }

            return asset;
        }

        private interface IAssetCompilerProxyInterface
        {
            void Compile(IAsset asset, TargetPlatform platform);
        }

        private class AssetCompilerProxy<T> : IAssetCompilerProxyInterface where T : IAsset
        {
            private IAssetCompiler<T> m_AssetCompiler;

            public AssetCompilerProxy(IAssetCompiler<T> compiler)
            {
                this.m_AssetCompiler = compiler;
            }

            public void Compile(IAsset asset, TargetPlatform platform)
            {
                this.m_AssetCompiler.Compile((T)asset, platform);
            }
        }
    }
}
