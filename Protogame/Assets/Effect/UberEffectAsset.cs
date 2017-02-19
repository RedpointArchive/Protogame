using Protoinject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{    
    public class UberEffectAsset : MarshalByRefObject, IAsset, INativeAsset
    {
        private readonly IKernel _kernel;
        
        private readonly IAssetContentManager _assetContentManager;

        private readonly IRawLaunchArguments _rawLaunchArguments;

        public UberEffectAsset(
            IKernel kernel,
            IAssetContentManager assetContentManager, 
            IRawLaunchArguments rawLaunchArguments,
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
            _rawLaunchArguments = rawLaunchArguments;
            SourcedFromRaw = sourcedFromRaw;
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
        
        public void ReadyOnGameThread()
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
                            switch (reader.ReadUInt32())
                            {
                                case 1:
                                {
                                    var count = reader.ReadUInt32();
                                    for (var i = 0; i < count; i++)
                                    {
                                        var name = reader.ReadString();
                                        var defines = reader.ReadString();
                                        var dataLength = reader.ReadInt32();
                                        var data = reader.ReadBytes(dataLength);

                                        // Use the new EffectWithSemantics class that allows for extensible semantics.
                                        var availableSemantics = _kernel.GetAll<IEffectSemantic>();
                                        Effects[name] = new ProtogameEffect(graphicsDevice, data, Name + ":" + name,
                                            availableSemantics);
                                    }
                                    break;
                                }
                                case 2:
                                {

                                    var count = reader.ReadUInt32();
                                    for (var i = 0; i < count; i++)
                                    {
                                        var name = reader.ReadString();
                                        var defines = reader.ReadString();
                                        var debugDataLength = reader.ReadInt32();
                                        var debugData = reader.ReadBytes(debugDataLength);
                                        var releaseDataLength = reader.ReadInt32();
                                        var releaseData = reader.ReadBytes(releaseDataLength);

                                        // Use the new EffectWithSemantics class that allows for extensible semantics.
                                        var availableSemantics = _kernel.GetAll<IEffectSemantic>();
                                        Effects[name] = new ProtogameEffect(
                                            graphicsDevice,
                                            _rawLaunchArguments.Arguments.Contains("--debug-shaders")
                                                ? debugData
                                                : releaseData,
                                            Name + ":" + name,
                                            availableSemantics);
                                    }
                                    break;
                                }
                                default:
                                    throw new InvalidOperationException("Unexpected version number for uber effect.");
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