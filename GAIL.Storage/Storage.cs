using GAIL.Serializing.Streams;
using GAIL.Storage.Members;
using GAIL.Storage.Parser;
using GAIL.Serializing.Formatters;

namespace GAIL.Storage;

/// <summary>
/// Represents a file containing data.
/// </summary>
public sealed class Storage : ParentNode {
    /// <summary>
    /// This is the formatter used for this storage file (default: DefaultFormatter).
    /// </summary>
    public IFormatter Formatter { get; set; }

    /// <summary>
    /// Creates a new storage.
    /// </summary>
    /// <param name="formatter">The formatter to use for encoding and decoding (default: DefaultFormatter).</param>
    public Storage(IFormatter? formatter = null) { Formatter = formatter??new DefaultFormatter(); }

    /// <summary>
    /// Loads the storage from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from (closes the stream).</param>
    /// <returns>True if it succeeded.</returns>
    public bool Load(Stream stream) {
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

    /// <summary>
    /// Loads the storage from a file.
    /// </summary>
    /// <param name="filePath">The path of the file.</param>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    /// <exception cref="PathTooLongException"/>
    /// <returns>True if it succeeded.</returns>
    public bool Load(string filePath) {
        using FileStream fs = new(Path.GetFullPath(filePath), FileMode.Open, FileAccess.Read);
        return Load(fs);
    }

    /// <summary>
    /// Saves the storage to a stream.
    /// </summary>
    /// <param name="stream">The stream to save to (closes the stream).</param>
    /// <returns>True if it succeeded.</returns>
    public bool Save(Stream stream) {
        StorageSerializer serializer = new(stream, false);

        try {
            serializer.Serialize(Children, Formatter);
        } catch (InvalidOperationException) {
            return false;
        }
        serializer.Dispose();

        return true;
    }
    
    /// <summary>
    /// Loads the storage from a file.
    /// </summary>
    /// <param name="filePath">The path of the file (does not need to exist).</param>
    /// <returns>True if it succeeded.</returns>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    /// <exception cref="PathTooLongException"/>
    /// <exception cref="UnauthorizedAccessException"/>
    /// <exception cref="IOException"/>
    public bool Save(string filePath) {
        using FileStream fs = new(Path.GetFullPath(filePath), FileMode.Create, FileAccess.Write);
        return Save(fs);
    }
}