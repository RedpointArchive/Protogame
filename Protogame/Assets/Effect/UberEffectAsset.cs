using Protoinject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{    
    public class UberEffectAsset : IAsset, INativeAsset, IDisposable
    {
        private readonly IKernel _kernel;
        private readonly IAssetContentManager _assetContentManager;
        private readonly IRawLaunchArguments _rawLaunchArguments;
        private byte[] _effect;

        public UberEffectAsset(
            IKernel kernel,
            IAssetContentManager assetContentManager, 
            IRawLaunchArguments rawLaunchArguments,
            string name,
            byte[] effect)
        {
            _kernel = kernel;
            _assetContentManager = assetContentManager;
            _rawLaunchArguments = rawLaunchArguments;
            Name = name;
            _effect = effect;
        }
        
        public Dictionary<string, IEffect> Effects { get; private set; }
        
        public string Name { get; private set; }

        public void Dispose()
        {
            foreach (var effect in Effects.Values)
            {
                effect?.NativeEffect?.Dispose();
            }
        }

        public void ReadyOnGameThread()
        {
            if (_assetContentManager == null)
            {
                throw new NoAssetContentManagerException();
            }

            if (_assetContentManager != null && _effect != null)
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
                    using (var memory = new MemoryStream(_effect))
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

                // Free the resource from main memory since it is now loaded into the GPU.  If the
                // resource is ever removed from the GPU (i.e. UnloadContent occurs followed by
                // LoadContent), then the asset will need to be reloaded through the asset management
                // system.
                _effect = null;
            }
        }
    }
}