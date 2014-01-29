using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Protogame
{
    public class AudioAsset : MarshalByRefObject, IAsset
    {
        public string Name { get; private set; }
        public SoundEffect Audio { get; private set; }
        public PlatformData PlatformData { get; set; }
        public byte[] RawData { get; set; }

        public AudioAsset(
            string name,
            byte[] rawData,
            PlatformData data,
            bool sourcedFromRaw)
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

        public bool SourceOnly
        {
            get
            {
                return this.PlatformData == null;
            }
        }

        public bool CompiledOnly
        {
            get
            {
                return this.RawData == null;
            }
        }

        public bool SourcedFromRaw
        {
            get;
            private set;
        }

        public void ReloadAudio()
        {
            if (this.PlatformData != null)
            {
                using (var stream = new MemoryStream(this.PlatformData.Data))
                {
                    this.Audio = SoundEffect.FromStream(stream);
                    if (this.Audio == null)
                        throw new InvalidOperationException("Unable to load effect from stream.");
                }
            }
        }
        
        public T Resolve<T>() where T : class, IAsset
        {
            if (typeof(T).IsAssignableFrom(typeof(AudioAsset)))
                return this as T;
            throw new InvalidOperationException("Asset already resolved to AudioAsset.");
        }
    }
}

