using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Protogame
{
    public class AudioAsset : IAsset, INativeAsset
    {
        public AudioAsset(string name, byte[] rawData)
        {
            Name = name;
            RawData = rawData;
        }
        
        public SoundEffect Audio { get; private set; }

        public string Name { get; private set; }
        
        public byte[] RawData { get; set; }
        
        public void ReadyOnGameThread()
        {
            if (RawData != null)
            {
                using (var stream = new MemoryStream(RawData))
                {
                    Audio = SoundEffect.FromStream(stream);
                    if (Audio == null)
                    {
                        throw new InvalidOperationException("Unable to load effect from stream.");
                    }
                }
            }
        }
    }
}