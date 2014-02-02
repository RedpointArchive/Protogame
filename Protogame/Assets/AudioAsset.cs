namespace Protogame
{
    using System;
    using System.IO;
    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// The audio asset.
    /// </summary>
    public class AudioAsset : MarshalByRefObject, IAsset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioAsset"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="rawData">
        /// The raw data.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="sourcedFromRaw">
        /// The sourced from raw.
        /// </param>
        public AudioAsset(string name, byte[] rawData, PlatformData data, bool sourcedFromRaw)
        {
            this.Name = name;
            this.RawData = rawData;
            this.PlatformData = data;
            this.SourcedFromRaw = sourcedFromRaw;

            if (this.PlatformData != null)
            {
                this.ReloadAudio();
            }
        }

        /// <summary>
        /// Gets the audio.
        /// </summary>
        /// <value>
        /// The audio.
        /// </value>
        public SoundEffect Audio { get; private set; }

        /// <summary>
        /// Gets a value indicating whether compiled only.
        /// </summary>
        /// <value>
        /// The compiled only.
        /// </value>
        public bool CompiledOnly
        {
            get
            {
                return this.RawData == null;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the platform data.
        /// </summary>
        /// <value>
        /// The platform data.
        /// </value>
        public PlatformData PlatformData { get; set; }

        /// <summary>
        /// Gets or sets the raw data.
        /// </summary>
        /// <value>
        /// The raw data.
        /// </value>
        public byte[] RawData { get; set; }

        /// <summary>
        /// Gets a value indicating whether source only.
        /// </summary>
        /// <value>
        /// The source only.
        /// </value>
        public bool SourceOnly
        {
            get
            {
                return this.PlatformData == null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether sourced from raw.
        /// </summary>
        /// <value>
        /// The sourced from raw.
        /// </value>
        public bool SourcedFromRaw { get; private set; }

        /// <summary>
        /// The reload audio.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void ReloadAudio()
        {
            if (this.PlatformData != null)
            {
                using (var stream = new MemoryStream(this.PlatformData.Data))
                {
                    this.Audio = SoundEffect.FromStream(stream);
                    if (this.Audio == null)
                    {
                        throw new InvalidOperationException("Unable to load effect from stream.");
                    }
                }
            }
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
            if (typeof(T).IsAssignableFrom(typeof(AudioAsset)))
            {
                return this as T;
            }

            throw new InvalidOperationException("Asset already resolved to AudioAsset.");
        }
    }
}