using GAIL.Serializing.Formatters;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Parser;

namespace GAIL.Storage;

/// <summary>
/// Represents a common layout for a storage file.
/// </summary>
public abstract class BaseStorage : ParentNode {
    /// <summary>
    /// This is the formatter used for this storage file.
    /// </summary>
    public IFormatter? Formatter { get; set; }

    /// <summary>
    /// Creates a new storage.
    /// </summary>
    /// <param name="formatter">The formatter to use for encoding and decoding .</param>
    public BaseStorage(IFormatter? formatter = null) { Formatter = formatter; }
    /// <summary>
    /// Loads the storage from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from (closes the stream).</param>
    /// <returns>True if it succeeded.</returns>
    public abstract bool Load(Stream stream);

    /// <summary>
    /// Loads the storage from a file.
    /// </summary>
    /// <param name="filePath">The path of the file.</param>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    /// <exception cref="PathTooLongException"/>
    /// <returns>True if it succeeded.</returns>
    public virtual bool Load(string filePath) {
        using FileStream fs = new(Path.GetFullPath(filePath), FileMode.Open, FileAccess.Read);
        return Load(fs);
    }

    /// <summary>
    /// Saves the storage to a stream.
    /// </summary>
    /// <param name="stream">The stream to save to (closes the stream).</param>
    /// <returns>True if it succeeded.</returns>
    public abstract bool Save(Stream stream);
    
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
    public virtual bool Save(string filePath) {
        using FileStream fs = new(Path.GetFullPath(filePath), FileMode.Create, FileAccess.Write);
        return Save(fs);
    }
}