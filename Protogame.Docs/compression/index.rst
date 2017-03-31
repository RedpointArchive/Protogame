Compression Utilities
================================

The compression utilities inside Protogame provide helper methods which
compress byte arrays using the LZMA algorithm.

The compression utilities do not require loading a module; instead use
the ``LzmaHelper`` class directly.

LzmaHelper.Compress
-----------------------

This method takes an input (uncompressed) stream and an output stream.

To compress a byte array, use this method in conjunction with the `MemoryStream`
class available in .NET:

.. literalinclude:: snippet/compress.cs
    :language: csharp

LzmaHelper.Decompress
-----------------------

This method takes an input (compressed) stream and an output stream.

To compress a byte array, use this method in conjunction with the `MemoryStream`
class available in .NET:

.. literalinclude:: snippet/decompress.cs
    :language: csharp