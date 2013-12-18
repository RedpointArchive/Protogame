using System;
using System.Linq;
using System.Text;
using Dx.Runtime;
using Newtonsoft.Json;

namespace Protogame
{
    [Distributed]
    public class NetworkAsset : IAsset
    {
        public string Name { get; private set; }
        private byte[] Data { get; set; }
        private NetworkAssetManager Manager { get; set; }
        
        [Local]
        private IAssetLoader[] m_AssetLoaders;

        [Local]
        private ITransparentAssetCompiler m_TransparentAssetCompiler;

        /// <summary>
        /// Raised when the network asset is dirtied.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler Dirtied;

        /// <summary>
        /// Whether the underlying network asset needs to be refreshed from
        /// the asset manager.
        /// </summary>
        public void Dirty()
        {
            if (this.Dirtied != null)
                this.Dirtied(null, new EventArgs());
            this.IsDirty = true;
        }

        /// <summary>
        /// Whether the asset has been dirtied.
        /// </summary>
        public bool IsDirty { get; private set; }

        internal NetworkAsset(
            IAssetLoader[] loaders,
            ITransparentAssetCompiler transparentAssetCompiler,
            object data,
            string name,
            NetworkAssetManager manager)
        {
            this.m_AssetLoaders = loaders;
            this.m_TransparentAssetCompiler = transparentAssetCompiler;
            this.Name = name;
            this.Manager = manager;
            this.IsDirty = false;
            if (data != null)
            {
                var raw = JsonConvert.SerializeObject(data);
                this.Data = Encoding.UTF8.GetBytes(raw);
            }
            else
                this.Data = null;
        }

        public bool SourceOnly
        {
            get
            {
                return false;
            }
        }

        public bool CompiledOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Injects the asset loader enumerable.  This is required since the network asset
        /// will be passed back to the client without that field set because it is
        /// local-only.
        /// </summary>
        /// <param name="loaders">The asset loaders this network asset should use.</param>
        [Local]
        public void InjectLoaders(IAssetLoader[] loaders)
        {
            this.m_AssetLoaders = loaders;
        }

        [Local]
        public T Resolve<T>() where T : class, IAsset
        {
            if (this.m_AssetLoaders == null)
                throw new InvalidOperationException(
                    "The asset loaders have not been set on this NetworkAsset.  This can occur if you retrieved " +
                    "a NetworkAsset via GetUnresolved and have not yet called InjectLoaders.  InjectLoaders is " +
                    "called for you if you use the Get<> method instead.");
        
            // We will use the registered asset loaders to resolve an
            // actual, local version of the asset.
            var data = this.GetAssetData();
            var loaders = this.m_AssetLoaders.ToArray();
            if (data != null)
            {
                var obj = (dynamic)JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(data));
                foreach (var loader in loaders)
                {
                    var result = false;
                    try
                    {
                        result = loader.CanHandle(obj);
                    }
                    catch (Exception)
                    {
                    }
                    if (result)
                    {
                        // We have to create a proxy around this class so
                        // that we can automatically discard the instance
                        // when we consider it to be dirty.
                        return this.FormProxyIfPossible<T>(
                            this.m_TransparentAssetCompiler.Handle(loader.Handle(this.Manager, this.Name, obj)));
                    }
                }
            }
            foreach (var loader in loaders)
            {
                var loadDefault = loader.GetDefault(this.Manager, this.Name);
                if (loadDefault is T)
                {
                    return this.FormProxyIfPossible<T>(loadDefault as T);
                }
            }
            throw new InvalidOperationException("Unable to resolve the network asset to a local asset.");
        }

        [Local]
        private T FormProxyIfPossible<T>(T obj) where T : class, IAsset
        {
            if (!typeof(MarshalByRefObject).IsAssignableFrom(obj.GetType()))
            {
                Console.WriteLine(
                    "WARNING: Asset type '" + obj.GetType().FullName + "' " +
                    "does not inherit from MarshalByRefObject; it will " +
                    "not automatically update in the game when changed " +
                    "from the asset manager.");
                return obj;
            }
            return new NetworkAssetProxy<T>(
                this.Manager,
                this,
                this.Name,
                obj).GetTransparentProxy() as T;
        }

        [ClientCallable]
        public byte[] GetAssetData()
        {
            return this.Data;
        }
    }
}
