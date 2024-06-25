namespace GAIL.Storage;

/// <summary>
/// Represents a file containing data.
/// </summary>
public sealed class Storage : ParentNode {
    /// <summary>
    /// Creates a new storage.
    /// </summary>
    public Storage() { }

    /// <summary>
    /// Loads the storage from a stream.
    /// </summary>
    /// <param name="stream">The binary stream to read from (doesn't close the stream).</param>
    /// <returns>True if it succeeded.</returns>
    public bool Load(BinaryReader stream) {
        return false;
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
        using BinaryReader bs = new(fs);
        return Load(bs);
    }

    /// <summary>
    /// Saves the storage to a stream.
    /// </summary>
    /// <param name="stream">The binary stream to save to (doesn't close the stream).</param>
    /// <returns>True if it succeeded.</returns>
    public bool Save(BinaryWriter stream) {
        return false;
    }
    
    /// <summary>
    /// Loads the storage from a file.
    /// </summary>
    /// <param name="filePath">The path of the file (does not need to exist).</param>
    /// <returns>True if it succeeded.</returns>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    /// <exception cref="PathTooLongException"/>
    /// <exception cref="IOException"/>
    public bool Save(string filePath) {
        using FileStream fs = new(Path.GetFullPath(filePath), FileMode.Create, FileAccess.Write);
        using BinaryWriter bs = new(fs);
        return Save(bs);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="node"/> is not a container or a field.</exception>
    public override void AddChild(IChildNode node) {
        if (node is not Container || node is not Field) {
            throw new ArgumentException("Node is not a container or field.", nameof(node));
        }
        base.AddChild(node);
    }
    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="node"/> is not a container or a field.</exception>
    public override bool RemoveChild(IChildNode node) {
        if (node is not Container || node is not Field) {
            throw new ArgumentException("Node is not a container or field.", nameof(node));
        }
        return base.RemoveChild(node);
    }
}