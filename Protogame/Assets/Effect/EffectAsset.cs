using Protoinject;
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Protogame
{    
    public class EffectAsset : MarshalByRefObject, IAsset
    {
        private readonly IKernel _kernel;
        
        private readonly IAssetContentManager _assetContentManager;

        private readonly IRawLaunchArguments _rawLaunchArguments;

        public EffectAsset(
            IKernel kernel,
            IAssetContentManager assetContentManager,
            IRawLaunchArguments rawLaunchArguments,
            string name, 
            string code, 
            PlatformData platformData, 
            bool sourcedFromRaw)
        {
            this.Name = name;
            this.Code = code;
            this.PlatformData = platformData;
            _kernel = kernel;
            this._assetContentManager = assetContentManager;
            _rawLaunchArguments = rawLaunchArguments;
            this.SourcedFromRaw = sourcedFromRaw;

            if (this.PlatformData != null)
            {
                try
                {
                    this.ReloadEffect();
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
                return this.Code == null;
            }
        }
        
        public IEffect Effect { get; private set; }
        
        public string Name { get; private set; }
        
        public PlatformData PlatformData { get; set; }
        
        public bool SourceOnly
        {
            get
            {
                return this.PlatformData == null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this asset was sourced from a raw file (such as a PNG image).
        /// </summary>
        /// <value>
        /// The sourced from raw.
        /// </value>
        public bool SourcedFromRaw { get; private set; }
        
        public void ReloadEffect()
        {
            if (this._assetContentManager == null)
            {
                throw new NoAssetContentManagerException();
            }

            if (this._assetContentManager != null && this.PlatformData != null)
            {
                // We don't load XNB-based effects because MonoGame doesn't have a reader
                // that supports them, and the XNB format mangles the shader bytecode if we
                // try and parse it out manually.  We also can't just define our own effect reader
                // as that doesn't seem to get picked up by the MonoGame content manager.

                // FIXME: We shouldn't be casting IAssetContentManager like this!
                var assetContentManager = this._assetContentManager as AssetContentManager;
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

                    using (var stream = new MemoryStream(this.PlatformData.Data))
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
                                            this.Effect = new ProtogameEffect(graphicsDevice, debugCode,
                                                this.Name, availableSemantics);
                                        }
                                        else
                                        {
                                            this.Effect = new ProtogameEffect(graphicsDevice, releaseCode,
                                                this.Name, availableSemantics);
                                        }
                                        break;
                                    default:
                                        throw new NotSupportedException("Unknown version of effect binary data.");
                                }
                            }
                            else
                            {
                                // This is a legacy shader with no versioning.
                                this.Effect = new ProtogameEffect(graphicsDevice, this.PlatformData.Data, this.Name, availableSemantics);
                            }
                        }
                    }
                }
            }
        }
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(EffectAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to EffectAsset.");
        }
    }
}