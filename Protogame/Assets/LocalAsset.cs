using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Protogame
{
    public class LocalAsset : IAsset
    {
        public string Name { get; private set; }
        private LocalAssetManager Manager { get; set; }

        private IAsset m_Instance;

        private IAsset m_CachedProxy;

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

        internal LocalAsset(
            string name,
            IAsset instance,
            LocalAssetManager manager)
        {
            this.Name = name;
            this.Manager = manager;
            this.IsDirty = false;
            this.m_Instance = instance;
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

        public IAsset Instance
        {
            get
            {
                return this.GetProxy(this.m_Instance);
            }
        }

        public T Resolve<T>() where T : class, IAsset
        {
            if (this.m_Instance is T)
            {
                return this.GetProxy(this.m_Instance as T);
            }
            throw new InvalidOperationException(
                "Local asset can not be resolved");
        }

        private T GetProxy<T>(T obj) where T : class, IAsset
        {
            if (this.m_CachedProxy != null)
            {
                return this.m_CachedProxy as T;
            }

            var proxy = this.FormProxyIfPossible(obj);
            this.m_CachedProxy = proxy;
            return proxy;
        }

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
            return new LocalAssetProxy<T>(
                this.Manager,
                this,
                this.Name,
                obj).GetTransparentProxy() as T;
        }
    }
}
