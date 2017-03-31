byte[] uncompressedBytes;
byte[] compressedBytes;

using (var input = new MemoryStream(uncompressedBytes))
{
    using (var output = new MemoryStream())
    {
        LzmaHelper.Compress(input, output);
        var length = output.Position;
        compressedBytes = new byte[length];
        output.Seek(SeekOrigin.Begin, 0);
        output.Read(compressedBytes, 0, length);
    }
}