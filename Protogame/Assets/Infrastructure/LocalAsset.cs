namespace Protogame
{
    using System;

    /// <summary>
    /// The local asset.
    /// </summary>
    public class LocalAsset : IAsset
    {
        /// <summary>
        /// The m_ instance.
        /// </summary>
        private readonly IAsset m_Instance;

        /// <summary>
        /// The m_ cached proxy.
        /// </summary>
        private IAsset m_CachedProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalAsset"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="manager">
        /// The manager.
        /// </param>
        internal LocalAsset(string name, IAsset instance, LocalAssetManager manager)
        {
            this.Name = name;
            this.Manager = manager;
            this.IsDirty = false;
            this.m_Instance = instance;
        }

        /// <summary>
        /// The dirtied.
        /// </summary>
        public event EventHandler Dirtied;

        /// <summary>
        /// Gets a value indicating whether compiled only.
        /// </summary>
        /// <value>
        /// The compiled only.
        /// </value>
        public bool CompiledOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public IAsset Instance
        {
            get
            {
                return this.GetProxy(this.m_Instance);
            }
        }

        /// <summary>
        /// Whether the asset has been dirtied.
        /// </summary>
        /// <value>
        /// The is dirty.
        /// </value>
        public bool IsDirty { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether source only.
        /// </summary>
        /// <value>
        /// The source only.
        /// </value>
        public bool SourceOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        private LocalAssetManager Manager { get; set; }

        /// <summary>
        /// Whether the underlying network asset needs to be refreshed from
        /// the asset manager.
        /// </summary>
        public void Dirty()
        {
            if (this.Dirtied != null)
            {
                this.Dirtied(null, new EventArgs());
            }

            this.IsDirty = true;
        }

        /// <summary>
        /// The resolve.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public T Resolve<T>() where T : class, IAsset
        {
            if (this.m_Instance is T)
            {
                return this.GetProxy(this.m_Instance as T);
            }

            throw new InvalidOperationException("Local asset can not be resolved");
        }

        /// <summary>
        /// The form proxy if possible.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private T FormProxyIfPossible<T>(T obj) where T : class, IAsset
        {
            if (!typeof(MarshalByRefObject).IsAssignableFrom(obj.GetType()))
            {
                Console.WriteLine(
                    "WARNING: Asset type '" + obj.GetType().FullName + "' "
                    + "does not inherit from MarshalByRefObject; it will "
                    + "not automatically update in the game when changed " + "from the asset manager.");
                return obj;
            }

            return new LocalAssetProxy(this.Manager, this, this.Name, obj).GetTransparentProxy() as T;
        }

        /// <summary>
        /// The get proxy.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
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
    }
}