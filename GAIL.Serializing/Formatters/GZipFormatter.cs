using System.IO.Compression;

namespace GAIL.Serializing.Formatters;

/// <summary>
/// Can encode / decode using GZip compression
/// </summary>
public class GZipFormatter : IFormatter {
    /// <inheritdoc/>
    public byte[] Decode(byte[] encoded) {
        using MemoryStream encodedStream = new(encoded);
        using MemoryStream decodedStream = new();
        using (GZipStream stream = new(encodedStream, CompressionMode.Decompress)) {
            stream.CopyTo(decodedStream);
        }
        return decodedStream.ToArray();
    }

    /// <inheritdoc/>
    public byte[] Encode(byte[] original) {
        using MemoryStream originalStream = new(original);
        using MemoryStream encodedStream = new();
        using (GZipStream stream = new(encodedStream, CompressionMode.Compress)) {
            originalStream.CopyTo(stream);
        }
        return encodedStream.ToArray();
    }
}