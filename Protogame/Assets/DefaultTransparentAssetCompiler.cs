namespace Protogame
{
    using Ninject;

    /// <summary>
    /// The default transparent asset compiler.
    /// </summary>
    public class DefaultTransparentAssetCompiler : ITransparentAssetCompiler
    {
        /// <summary>
        /// The m_ kernel.
        /// </summary>
        private readonly IKernel m_Kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTransparentAssetCompiler"/> class.
        /// </summary>
        /// <param name="kernel">
        /// The kernel.
        /// </param>
        public DefaultTransparentAssetCompiler(IKernel kernel)
        {
            this.m_Kernel = kernel;
        }

        /// <summary>
        /// The AssetCompilerProxyInterface interface.
        /// </summary>
        private interface IAssetCompilerProxyInterface
        {
            /// <summary>
            /// The compile.
            /// </summary>
            /// <param name="asset">
            /// The asset.
            /// </param>
            /// <param name="platform">
            /// The platform.
            /// </param>
            void Compile(IAsset asset, TargetPlatform platform);
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="force">
        /// The force.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        public void Handle(IAsset asset, bool force = false)
        {
            this.HandlePlatform(asset, TargetPlatformUtility.GetExecutingPlatform(), force);
        }

        /// <summary>
        /// The handle platform.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        /// <param name="force">
        /// The force.
        /// </param>
        /// <returns>
        /// The <see cref="IAsset"/>.
        /// </returns>
        public void HandlePlatform(IAsset asset, TargetPlatform platform, bool force = false)
        {
            if (asset.SourceOnly || force)
            {
                var compilerType = typeof(IAssetCompiler<>).MakeGenericType(asset.GetType());
                var compiler = this.m_Kernel.TryGet(compilerType);
                if (compiler == null)
                {
                    // The caller will throw AssetNotCompiledException if all of the candidates
                    // for loading only have source information.
                    return;
                }

                var proxyType = typeof(AssetCompilerProxy<>).MakeGenericType(asset.GetType());
                var proxy =
                    (IAssetCompilerProxyInterface)
                    proxyType.GetConstructor(new[] { compilerType }).Invoke(new[] { compiler });
                proxy.Compile(asset, platform);
            }
        }

        /// <summary>
        /// The asset compiler proxy.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        private class AssetCompilerProxy<T> : IAssetCompilerProxyInterface
            where T : IAsset
        {
            /// <summary>
            /// The m_ asset compiler.
            /// </summary>
            private readonly IAssetCompiler<T> m_AssetCompiler;

            /// <summary>
            /// Initializes a new instance of the <see cref="AssetCompilerProxy{T}"/> class.
            /// </summary>
            /// <param name="compiler">
            /// The compiler.
            /// </param>
            public AssetCompilerProxy(IAssetCompiler<T> compiler)
            {
                this.m_AssetCompiler = compiler;
            }

            /// <summary>
            /// The compile.
            /// </summary>
            /// <param name="asset">
            /// The asset.
            /// </param>
            /// <param name="platform">
            /// The platform.
            /// </param>
            public void Compile(IAsset asset, TargetPlatform platform)
            {
                this.m_AssetCompiler.Compile((T)asset, platform);
            }
        }
    }
}