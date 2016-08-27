// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Protoinject;

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IAssetManagerProviderInitializer"/>.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IAssetManagerProviderInitializer</interface_ref>
    public class AssetManagerProviderInitializer : IAssetManagerProviderInitializer
    {
        private readonly IKernel _kernel;
        private readonly string[] _args;

        public AssetManagerProviderInitializer(IKernel kernel, string[] args)
        {
            _kernel = kernel;
            _args = args;
        }

        public void Initialize<T>() where T : IAssetManagerProvider
        {
            AssetManagerClient.AcceptArgumentsAndSetup<T>(_kernel, _args);
        }
    }
}