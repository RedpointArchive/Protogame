using Protoinject;
using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Protogame
{
    public class EffectAsset : IAsset, INativeAsset
    {
        private readonly IKernel _kernel;
        private readonly IAssetContentManager _assetContentManager;
        private readonly IRawLaunchArguments _rawLaunchArguments;
        private readonly byte[] _effect;

        public EffectAsset(
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
        
        public IEffect Effect { get; private set; }
        
        public string Name { get; private set; }

        public void ReadyOnGameThread()
        {
            if (_assetContentManager == null)
            {
                throw new NoAssetContentManagerException();
            }

            if (_assetContentManager != null && _effect != null)
            {
                // We don't load XNB-based effects because MonoGame doesn't have a reader
                // that supports them, and the XNB format mangles the shader bytecode if we
                // try and parse it out manually.  We also can't just define our own effect reader
                // as that doesn't seem to get picked up by the MonoGame content manager.

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

                    var availableSemantics = _kernel.GetAll<IEffectSemantic>();

                    using (var stream = new MemoryStream(_effect))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            if (reader.ReadUInt32() == (uint) 0x12345678)
                            {
                                // This is the new effect format that supports storing both debug
                                // and release builds of the shader code.
                                switch (reader.ReadUInt32())
                                {
                                    case 1:
                                        var debugLength = reader.ReadInt32();
                                        var debugCode = reader.ReadBytes(debugLength);
                                        var releaseLength = reader.ReadInt32();
                                        var releaseCode = reader.ReadBytes(releaseLength);
                                        if (_rawLaunchArguments.Arguments.Contains("--debug-shaders"))
                                        {
                                            Effect = new ProtogameEffect(graphicsDevice, debugCode,
                                                Name, availableSemantics);
                                        }
                                        else
                                        {
                                            Effect = new ProtogameEffect(graphicsDevice, releaseCode,
                                                Name, availableSemantics);
                                        }
                                        break;
                                    default:
                                        throw new NotSupportedException("Unknown version of effect binary data.");
                                }
                            }
                            else
                            {
                                // This is a legacy shader with no versioning.
                                Effect = new ProtogameEffect(graphicsDevice, _effect, Name, availableSemantics);
                            }
                        }
                    }
                }
            }
        }
    }
}