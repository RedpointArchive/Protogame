namespace Protogame
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using ProtoBuf;

    /// <summary>
    /// Represents a compiled, raw asset.
    /// </summary>
    /// <remarks>
    /// This class is used to serialize and deserialize compiled assets on disk.  Instead
    /// of compiled assets being stored as JSON (as is the case with source assets), compiled
    /// assets are serialized with Protobuf and compressed using LZMA to reduce
    /// the disk space they consume.
    /// </remarks>
    [ProtoContract]
    public class CompiledAsset : IRawAsset
    {
        /// <summary>
        /// Gets or sets the type name for the loader which can load this asset.
        /// </summary>
        /// <value>
        /// The type name of the loader class to load this asset.
        /// </value>
        [ProtoMember(1)]
        public string Loader { get; set; }

        /// <summary>
        /// Gets or sets the platform-specific data associated with this compiled asset.
        /// </summary>
        /// <value>
        /// The platform-specific data associated with this compiled asset.
        /// </value>
        [ProtoMember(3)]
        public PlatformData PlatformData { get; set; }

        /// <summary>
        /// Indicates that the asset is compiled.
        /// </summary>
        public bool IsCompiled
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// A read-only copy of the properties associated with this raw asset.
        /// </summary>
        public ReadOnlyCollection<KeyValuePair<string, object>> Properties
        {
            get
            {
                return new ReadOnlyCollection<KeyValuePair<string, object>>(new[]
                {
                    new KeyValuePair<string, object>("Loader", this.Loader),
                    new KeyValuePair<string, object>("PlatformData", this.PlatformData), 
                });
            }
        }

        /// <summary>
        /// Retrieve a property value from a raw asset.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="defaultValue">The default value if the property isn't present.</param>
        /// <typeparam name="T">The type of data to retrieve.</typeparam>
        /// <returns>The data on the raw asset.</returns>
        public T GetProperty<T>(string name, T defaultValue = default(T))
        {
            switch (name)
            {
                case "Loader":
                    if (typeof(T) != typeof(string))
                    {
                        throw new InvalidCastException();
                    }

                    return (T)(object)this.Loader;
                case "PlatformData":
                    if (typeof(T) != typeof(PlatformData))
                    {
                        throw new InvalidCastException();
                    }

                    return (T)(object)this.PlatformData;
                default:
                    throw new MissingMemberException(this.GetType().FullName, name);
            }
        }
    }
}