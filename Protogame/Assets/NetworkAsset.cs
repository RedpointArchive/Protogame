//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using Process4.Attributes;
using System.Web.Script.Serialization;
using System;
using Ninject;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Protogame
{
    [Distributed]
    public class NetworkAsset : IAsset
    {
        public string Name { get; private set; }
        private byte[] Data { get; set; }
        private NetworkAssetManager Manager { get; set; }
        private IEnumerable<IAssetLoader> m_AssetLoaders;

        /// <summary>
        /// Whether the underlying network asset needs to be refreshed from
        /// the asset manager.
        /// </summary>
        public bool Dirty
        {
            get;
            set;
        }

        internal NetworkAsset(IEnumerable<IAssetLoader> loaders, object data, string name, NetworkAssetManager manager)
        {
            this.m_AssetLoaders = loaders;
            this.Name = name;
            this.Manager = manager;
            this.Dirty = false;
            if (data != null)
            {
                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new[] { new DynamicJsonUnconverter() });
                var raw = serializer.Serialize(data);
                this.Data = Encoding.UTF8.GetBytes(raw);
            }
            else
                this.Data = null;
        }

        [Local]
        public T Resolve<T>() where T : class, IAsset
        {
            // We will use the registered asset loaders to resolve an
            // actual, local version of the asset.
            var data = this.GetAssetData();
            var loaders = this.m_AssetLoaders.ToArray();
            if (data != null)
            {
                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                var obj = (dynamic)serializer.Deserialize<object>(Encoding.UTF8.GetString(data));
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
                            loader.Handle(this.Name, obj));
                    }
                }
            }
            foreach (var loader in loaders)
            {
                var loadDefault = loader.GetDefault(this.Name);
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
