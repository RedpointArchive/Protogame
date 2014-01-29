using System.Linq;

namespace Protogame
{
    public class AudioAssetSaver : IAssetSaver
    {
        public bool CanHandle(IAsset asset)
        {
            return asset is AudioAsset;
        }

        public dynamic Handle(IAsset asset, AssetTarget target)
        {
            var audioAsset = asset as AudioAsset;

            if (audioAsset.SourcedFromRaw && target != AssetTarget.CompiledFile)
            {
                // We were sourced from a raw WAV; we don't want to save
                // an ".asset" file back to disk.
                return null;
            }

            if (target == AssetTarget.CompiledFile)
            {
                return new CompiledAsset
                {
                    Loader = typeof(AudioAssetLoader).FullName,
                    PlatformData = audioAsset.PlatformData
                };
            }

            return new
            {
                Loader = typeof(AudioAssetLoader).FullName,
                PlatformData = target == AssetTarget.SourceFile ? null : audioAsset.PlatformData,
                RawData = audioAsset.RawData == null ? null : audioAsset.RawData.Select(x => (int)x).ToList()
            };
        }
    }
}

