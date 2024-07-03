using GAIL.Serializing;
using GAIL.Storage.Parser;

namespace GAIL.Storage.Members;

/// <summary>
/// Contains information about fields.
/// </summary>
/// <param name="FixedSize">The fixed size of the field.</param>
/// <param name="Creator">Can create a field from the raw bytes and from a key.</param>
public record struct FieldInfo(uint? FixedSize, Func<byte[], string, Field> Creator);

/// <summary>
/// Represents a field in a storage file.
/// </summary>
public abstract class Field : Member, IMember, ISerializable {
    /// <summary>
    /// Creates a new field info.
    /// </summary>
    /// <param name="creator">The field creator that creates a field from a key.</param>
    /// <returns>A new field info.</returns>
    public static FieldInfo CreateFieldInfo(Func<string, Field> creator) {
        return new(creator(string.Empty).FixedSize, (byte[] raw, string key) => {
            Field field = creator(key);
            field.Parse(raw);
            return field;
        });
    }
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
}