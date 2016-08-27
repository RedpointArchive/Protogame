// ReSharper disable CheckNamespace
#pragma warning disable 1591

namespace Protogame
{
    /// <summary>
    /// The default implementation of <see cref="IRawLaunchArguments"/>.
    /// </summary>
    /// <module>Core API</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IRawLaunchArguments</interface_ref>
    public class DefaultRawLaunchArguments : IRawLaunchArguments
    {
        public DefaultRawLaunchArguments(string[] args)
        {
            Arguments = args;
        }

        public string[] Arguments { get; }
    }
}