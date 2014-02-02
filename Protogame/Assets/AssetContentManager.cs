namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// The asset content manager.
    /// </summary>
    public class AssetContentManager : ContentManager, IAssetContentManager
    {
        /// <summary>
        /// The m_ memory streams.
        /// </summary>
        private readonly Dictionary<string, Stream> m_MemoryStreams = new Dictionary<string, Stream>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetContentManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        public AssetContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public override T Load<T>(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new ArgumentNullException("assetName");
            }

            T result = default(T);

            // Check for a previously loaded asset first
            object asset = null;
            if (this.LoadedAssets.TryGetValue(assetName, out asset))
            {
                if (asset is T)
                {
                    return (T)asset;
                }
            }

            // Load the asset.
            result = this.ReadAsset<T>(assetName, x => { });

            this.LoadedAssets[assetName] = result;
            return result;
        }

        /// <summary>
        /// The purge.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        public void Purge(string assetName)
        {
            if (!this.LoadedAssets.ContainsKey(assetName))
            {
                return;
            }

            var asset = this.LoadedAssets[assetName];
            if (asset is IDisposable)
            {
                (asset as IDisposable).Dispose();
            }

            this.LoadedAssets.Remove(assetName);
        }

        /// <summary>
        /// The set stream.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        public void SetStream(string assetName, Stream stream)
        {
            this.m_MemoryStreams[assetName] = stream;
        }

        /// <summary>
        /// The unset stream.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        public void UnsetStream(string assetName)
        {
            this.m_MemoryStreams[assetName] = null;
        }

        /// <summary>
        /// The open stream.
        /// </summary>
        /// <param name="assetName">
        /// The asset name.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        protected override Stream OpenStream(string assetName)
        {
            return this.m_MemoryStreams[assetName];
        }
    }
}