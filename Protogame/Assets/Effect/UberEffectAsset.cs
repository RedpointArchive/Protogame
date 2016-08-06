using Protoinject;
using System;
using System.Collections.Generic;
using System.IO;
using Cloo;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{    
    public class UberEffectAsset : MarshalByRefObject, IAsset
    {
        private readonly IKernel _kernel;
        
        private readonly IAssetContentManager _assetContentManager;
        
        public UberEffectAsset(
            IKernel kernel,
            IAssetContentManager assetContentManager, 
            string name, 
            string code, 
            PlatformData platformData, 
            bool sourcedFromRaw)
        {
            Name = name;
            Code = code;
            PlatformData = platformData;
            _kernel = kernel;
            _assetContentManager = assetContentManager;
            SourcedFromRaw = sourcedFromRaw;

            if (PlatformData != null)
            {
                try
                {
                    ReloadEffects();
                }
                catch (NoAssetContentManagerException)
                {
                }
            }
        }
        
        public string Code { get; set; }
        
        public bool CompiledOnly
        {
            get
            {
                return Code == null;
            }
        }
        
        public Dictionary<string, IEffect> Effects { get; private set; }
        
        public string Name { get; private set; }
        
        public PlatformData PlatformData { get; set; }
        
        public bool SourceOnly
        {
            get
            {
                return PlatformData == null;
            }
        }
        
        public bool SourcedFromRaw { get; private set; }
        
        public void ReloadEffects()
        {
            if (_assetContentManager == null)
            {
                throw new NoAssetContentManagerException();
            }

            if (_assetContentManager != null && PlatformData != null)
            {
                // FIXME: We shouldn't be casting IAssetContentManager like this!
                var assetContentManager = _assetContentManager as AssetContentManager;
                if (assetContentManager == null)
                {
                    throw new NoAssetContentManagerException();
                }

                var serviceProvider = assetContentManager.ServiceProvider;
                var graphicsDeviceProvider =
                    (IGraphicsDeviceService)serviceProvider.GetService(typeof(IGraphicsDeviceService));
                if (graphicsDeviceProvider != null && graphicsDeviceProvider.GraphicsDevice != null)
                {
                    var graphicsDevice = graphicsDeviceProvider.GraphicsDevice;

                    // The platform data for uber effects is a bunch of XNA effects
                    // concatenated together.  We have to parse the binary platform data, and load
                    // each effect into our Effects dictionary.
                    Effects = new Dictionary<string, IEffect>();
                    using (var memory = new MemoryStream(PlatformData.Data))
                    {
                        using (var reader = new BinaryReader(memory))
                        {
                            if (reader.ReadUInt32() != 1)
                            {
                                throw new InvalidOperationException("Unexpected version number for uber effect.");
                            }

                            var count = reader.ReadUInt32();
                            for (var i = 0; i < count; i++)
                            {
                                var name = reader.ReadString();
                                var defines = reader.ReadString();
                                var dataLength = reader.ReadInt32();
                                var data = reader.ReadBytes(dataLength);

                                // Use the new EffectWithSemantics class that allows for extensible semantics.
                                var availableSemantics = _kernel.GetAll<IEffectSemantic>();
                                Effects[name] = new ProtogameEffect(graphicsDevice, PlatformData.Data, Name, availableSemantics);
                            }
                        }
                    }
                }
            }
        }
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(UberEffectAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to UberEffectAsset.");
        }
    }
}