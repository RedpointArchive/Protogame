using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Protogame
{
    public class AudioAsset : MarshalByRefObject, IAsset
    {
        public string Name { get; private set; }
        public SoundEffect Audio { get; private set; }
        public byte[] Data { get; set; }
        public string SourcePath { get; set; }

        public AudioAsset(
            string name,
            string sourcePath,
            byte[] data)
        {
            this.Name = name;
            this.SourcePath = sourcePath;
            this.Data = data;
            
            this.ReloadAudio();
        }
        
        public void ReloadAudio()
        {
            if (this.Data != null)
            {
                using (var stream = new MemoryStream(this.Data))
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

