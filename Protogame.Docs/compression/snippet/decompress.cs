byte[] uncompressedBytes;
byte[] compressedBytes;

using (var input = new MemoryStream(compressedBytes))
{
    using (var output = new MemoryStream())
    {
        LzmaHelper.Decompress(input, output);
        var length = output.Position;
        uncompressedBytes = new byte[length];
        output.Seek(SeekOrigin.Begin, 0);
        output.Read(uncompressedBytes, 0, length);
    }
}