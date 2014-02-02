// This is a copy of the 7-zip LZMA
// compression library.
namespace Protogame.Compression
{
    using System;
    using System.IO;

    /// <summary>
    /// The InWindowStream interface.
    /// </summary>
    internal interface IInWindowStream
    {
        /// <summary>
        /// The get index byte.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        byte GetIndexByte(int index);

        /// <summary>
        /// The get match len.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="distance">
        /// The distance.
        /// </param>
        /// <param name="limit">
        /// The limit.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        uint GetMatchLen(int index, uint distance, uint limit);

        /// <summary>
        /// The get num available bytes.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        uint GetNumAvailableBytes();

        /// <summary>
        /// The init.
        /// </summary>
        void Init();

        /// <summary>
        /// The release stream.
        /// </summary>
        void ReleaseStream();

        /// <summary>
        /// The set stream.
        /// </summary>
        /// <param name="inStream">
        /// The in stream.
        /// </param>
        void SetStream(Stream inStream);
    }

    /// <summary>
    /// The MatchFinder interface.
    /// </summary>
    internal interface IMatchFinder : IInWindowStream
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="historySize">
        /// The history size.
        /// </param>
        /// <param name="keepAddBufferBefore">
        /// The keep add buffer before.
        /// </param>
        /// <param name="matchMaxLen">
        /// The match max len.
        /// </param>
        /// <param name="keepAddBufferAfter">
        /// The keep add buffer after.
        /// </param>
        void Create(uint historySize, uint keepAddBufferBefore, uint matchMaxLen, uint keepAddBufferAfter);

        /// <summary>
        /// The get matches.
        /// </summary>
        /// <param name="distances">
        /// The distances.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        uint GetMatches(uint[] distances);

        /// <summary>
        /// The skip.
        /// </summary>
        /// <param name="num">
        /// The num.
        /// </param>
        void Skip(uint num);
    }
}