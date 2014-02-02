namespace Protogame
{
    /// <summary>
    /// The audio asset compiler.
    /// </summary>
    public class AudioAssetCompiler : IAssetCompiler<AudioAsset>
    {
        /// <summary>
        /// The compile.
        /// </summary>
        /// <param name="asset">
        /// The asset.
        /// </param>
        /// <param name="platform">
        /// The platform.
        /// </param>
        public void Compile(AudioAsset asset, TargetPlatform platform)
        {
            if (asset.RawData == null)
            {
                return;
            }

            // Currently we just copy the raw data.  According to the XNA documentation:
            /*
             * The Stream object must point to the head of a valid PCM wave file. Also, this wave file must be in the RIFF bitstream format.
             * The audio format has the following restrictions:
             * Must be a PCM wave file
             * Can only be mono or stereo
             * Must be 8 or 16 bit
             * Sample rate must be between 8,000 Hz and 48,000 Hz
             */
            asset.PlatformData = new PlatformData { Platform = platform, Data = asset.RawData };

            try
            {
                asset.ReloadAudio();
            }
            catch (NoAssetContentManagerException)
            {
            }
        }
    }
}