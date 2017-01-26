using System;

namespace Protogame
{
    /// <summary>
    /// Declares a configuration class to load when the game or server starts.  The type passed to this
    /// attribute must implement <see cref="IGameConfiguration"/> or <see cref="IServerConfiguration"/>.
    /// <para>
    /// If you don't declare any configuration attributes on any assembly that is loaded,
    /// Protogame will scan all assemblies for implementors of <see cref="IGameConfiguration"/>
    /// and <see cref="IServerConfiguration"/>.  Using reflection to scan all assemblies and all
    /// types is significantly slower than using the attribute, so specifying the attribute is
    /// highly recommended.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ConfigurationAttribute : Attribute
    {
        public Type GameConfigurationOrServerClass { get; }

        /// <summary>
        /// Declares a configuration class to load when the game or server starts.  The type passed to this
        /// attribute must implement <see cref="IGameConfiguration"/> or <see cref="IServerConfiguration"/>.
        /// <para>
        /// If you don't declare any configuration attributes on any assembly that is loaded,
        /// Protogame will scan all assemblies for implementors of <see cref="IGameConfiguration"/>
        /// and <see cref="IServerConfiguration"/>.  Using reflection to scan all assemblies and all
        /// types is significantly slower than using the attribute, so specifying the attribute is
        /// highly recommended.
        /// </para>
        /// </summary>
        /// <param name="gameConfigurationOrServerClass">The game configuration class to load on game or server startup.</param>
        public ConfigurationAttribute(Type gameConfigurationOrServerClass)
        {
            GameConfigurationOrServerClass = gameConfigurationOrServerClass;
        }
    }
}
