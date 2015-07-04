using System.Collections.ObjectModel;
using System.Linq;

namespace Protogame
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An asset which represents a set of configuration options.
    /// </summary>
    public class ConfigurationAsset : MarshalByRefObject, IAsset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationAsset"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the configuration asset.
        /// </param>
        /// <param name="settings">
        /// A dictionary of setting groups, to a dictionary of key-value settings.
        /// </param>
        public ConfigurationAsset(string name, Dictionary<string, Dictionary<string, object>> settings)
        {
            this.Name = name;
            this.Settings = settings;
        }

        /// <summary>
        /// Gets a value indicating whether the asset is in compiled form only.
        /// </summary>
        /// <value>
        /// Whether the asset is in compiled form only.
        /// </value>
        public bool CompiledOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the name of the asset.
        /// </summary>
        /// <value>
        /// The name of the asset.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the asset is in source form only.
        /// </summary>
        /// <value>
        /// Whether the asset is in source form only.
        /// </value>
        public bool SourceOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the settings dictionary.
        /// </summary>
        /// <value>The settings.</value>
        public Dictionary<string, Dictionary<string, object>> Settings { get; set; }

        /// <summary>
        /// Gets a setting from the configuration asset.
        /// </summary>
        /// <returns>The setting value.</returns>
        /// <param name="group">The setting group.</param>
        /// <param name="key">The setting key in the group.</param>
        /// <param name="default">The default value.</param>
        public T GetSetting<T>(string @group, string key, T @default)
        {
            if (!this.Settings.ContainsKey(@group))
            {
                return @default;
            }

            if (!this.Settings[@group].ContainsKey(key))
            {
                return @default;
            }

            var rawValue = this.Settings[@group][key];

            if (typeof(T) == typeof(int) && rawValue is long)
            {
                return (T)(object)(int)((long)rawValue);
            }

            if (typeof(T) == typeof(short) && rawValue is long)
            {
                return (T)(object)(short)((long)rawValue);
            }

            if (typeof(T) == typeof(float) && rawValue is long)
            {
                return (T)(object)(float)((long)rawValue);
            }

            if (typeof(T) == typeof(double) && rawValue is long)
            {
                return (T)(object)(double)((long)rawValue);
            }

            if (typeof(T) == typeof(int) && rawValue is double)
            {
                return (T)(object)(int)((double)rawValue);
            }

            if (typeof(T) == typeof(short) && rawValue is double)
            {
                return (T)(object)(short)((double)rawValue);
            }

            if (typeof(T) == typeof(float) && rawValue is double)
            {
                return (T)(object)(float)((double)rawValue);
            }

            return (T)rawValue;
        }

        /// <summary>
        /// Gets all of the settings groups.
        /// </summary>
        /// <returns>The setting groups.</returns>
        public ReadOnlyCollection<string> GetSettingGroups()
        {
            return this.Settings.Keys.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets all of the settings keys in a group.
        /// </summary>
        /// <returns>The setting keys.</returns>
        public ReadOnlyCollection<string> GetSettingGroups(string @group)
        {
            if (!this.Settings.ContainsKey(@group))
            {
                return new List<string>().AsReadOnly();
            }

            return this.Settings[@group].Keys.ToList().AsReadOnly();
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
            if (typeof(T).IsAssignableFrom(typeof(ConfigurationAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to ConfigurationAsset.");
        }
    }
}