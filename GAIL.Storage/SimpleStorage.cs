using GAIL.Serializing.Formatters;
using GAIL.Storage.Streams;

namespace GAIL.Storage;

/// <summary>
/// Represents a file containing data.
/// </summary>
public sealed class SimpleStorage : BaseStorage {
    /// <inheritdoc/>
    public SimpleStorage(IFormatter? formatter = null) : base(formatter) { }

    /// <inheritdoc/>
    public override bool Load(Stream stream) {
        StorageParser parser;

        try {
            parser = new(stream, false);
        } catch (InvalidOperationException) {
            return false;
        }

        try {
            children = parser.Parse(Formatter);
        } catch (KeyNotFoundException) {
            return false;
        }
        parser.Dispose();

        return true;
    }

    /// <inheritdoc/>
    public override bool Save(Stream stream) {
        StorageSerializer serializer = new(stream, false);

        try {
            serializer.Serialize(Children, Formatter);
        } catch (InvalidOperationException) {
            return false;
        }
        serializer.Dispose();

        return true;
    }
}