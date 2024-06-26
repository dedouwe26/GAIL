using GAIL.Serializing;

namespace GAIL.Storage;



/// <summary>
/// Represents a field in a storage file.
/// </summary>
public abstract class Field : ChildNode, ISerializable {
    /// <summary>
    /// Creates a new field, with a key.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    public Field(string key) : base(key) { }
    /// <summary>
    /// Creates a new field, with a key and parent.
    /// </summary>
    /// <param name="key">The hey of this field.</param>
    /// <param name="parent">The parent of this field (must be a container or a storage node).</param>
    /// <exception cref="ArgumentException">Parent is not a container or a storage node.</exception>
    public Field(string key, IParentNode parent) : base(key, parent) { }

    /// <inheritdoc/>
    public abstract uint? FixedSize { get; }

    /// <inheritdoc/>
    public abstract void Parse(byte[] data);

    /// <inheritdoc/>
    public abstract byte[] Serialize();
    /// <inheritdoc/>
    /// <exception cref="ArgumentException">Parent is not a container or a storage node.</exception>
    public override void SetParent(IParentNode parent) {
        if (parent is not Container || parent is not Storage) {
            throw new ArgumentException("Parent is not a container or a storage node.", nameof(parent));
        }
        base.SetParent(parent);
    }
}