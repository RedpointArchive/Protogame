﻿using System.Threading.Tasks;

namespace Protogame
{
    public class AudioAssetLoader : IAssetLoader<AudioAsset>
    {
        public async Task<IAsset> Load(string name, IReadableSerializedAsset input, IAssetManager assetManager)
        {
            return new AudioAsset(name, input.GetByteArray("Data"));
        }
    }
}
