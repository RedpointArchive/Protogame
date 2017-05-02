using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Protogame
{
    public class AudioAsset : IAsset, INativeAsset, IDisposable
    {
        private byte[] _data;

        public AudioAsset(string name, byte[] rawData)
        {
            Name = name;
            _data = rawData;
        }
        
        public SoundEffect Audio { get; private set; }

        public string Name { get; private set; }

        public void Dispose()
        {
            Audio?.Dispose();
        }

        public void ReadyOnGameThread()
        {
            if (_data != null)
            {
                using (var stream = new MemoryStream(_data))
                {
                    Audio = SoundEffect.FromStream(stream);
                    if (Audio == null)
                    {
                        throw new InvalidOperationException("Unable to load effect from stream.");
                    }
                }

                // Free our copy of the raw data as it is now loaded into MonoGame.
                _data = null;
            }
        }
    }
}