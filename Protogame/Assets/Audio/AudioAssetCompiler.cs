using System.Threading.Tasks;

namespace Protogame
{
    public class AudioAssetCompiler : IAssetCompiler
    {
        public string[] Extensions => new[] { "wav" };

        public async Task CompileAsync(IAssetFsFile assetFile, IAssetDependencies assetDependencies, TargetPlatform platform, ISerializedAsset output)
        {
            var content = await assetFile.GetContentStream().ConfigureAwait(false);

            // Currently we just copy the raw data.  According to the XNA documentation:
            /*
             * The Stream object must point to the head of a valid PCM wave file. Also, this wave file must be in the RIFF bitstream format.
             * The audio format has the following restrictions:
             * Must be a PCM wave file
             * Can only be mono or stereo
             * Must be 8 or 16 bit
             * Sample rate must be between 8,000 Hz and 48,000 Hz
             */
            output.SetLoader<IAssetLoader<AudioAsset>>();
            var bytes = new byte[content.Length];
            await content.ReadAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            output.SetByteArray("Data", bytes);
        }
    }
}
