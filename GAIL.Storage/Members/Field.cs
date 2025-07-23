using GAIL.Serializing;
using GAIL.Storage.Hierarchy;

namespace GAIL.Storage.Members;


/// <summary>
/// Represents a node in a storage file with actual data.
/// </summary>
public interface IField : IChildNode, IRawSerializable {
    /// <summary>
    /// Contains information about fields.
    /// </summary>
    /// <param name="FixedSize">The fixed size of the field.</param>
    /// <param name="Creator">Can create a field from the raw bytes and from a key.</param>
    public new record Info (uint? FixedSize, Func<byte[], string, IField> Creator);
    /// <summary>
    /// Creates a new field info.
    /// </summary>
    /// <param name="creator">The field creator that creates a field from a key.</param>
    /// <returns>A new field info.</returns>
    public static Info CreateInfo(Func<string, IField> creator) {
        return new(creator("").FixedSize, (raw, key) => {
            IField field = creator(key);
            field.Parse(raw);
            return field;
        });
    }
}
/// <summary>
/// Represents a node in a storage file with an actual value.
/// </summary>
public interface IField<T> : IChildNode, IRawSerializable<T> { }

/// <summary>
/// Represents a basic field implementation using a serializable.
/// </summary>
public class SerializableField : Node, IField {
    /// <summary>
    /// The underlying serializable.
    /// </summary>
    public readonly IRawSerializable serializable;

    /// <summary>
    /// Creates a new serializable field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="baseSerializable">The underlying serializable.</param>
    public SerializableField(string key, IRawSerializable baseSerializable) : base(key) {
        serializable = baseSerializable;
    }
    /// <summary>
    /// Creates a new serializable field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="baseSerializable">The underlying serializable.</param>
    /// <param name="parent">The parent of this node.</param>
    public SerializableField(string key, IRawSerializable baseSerializable, IParentNode parent) : base(key, parent) {
        serializable = baseSerializable;
    }
    /// <inheritdoc/>
    public uint? FixedSize => serializable.FixedSize;

    /// <inheritdoc/>
    public void Parse(byte[] data) {
        serializable.Parse(data);
    }

    /// <inheritdoc/>
    public byte[] Serialize() {
        return serializable.Serialize();
    }
}
/// <summary>
/// Represents a basic field implementation using a serializable with a value.
/// </summary>
public class SerializableField<T> : Node, IField<T> {
    /// <summary>
    /// The underlying serializable.
    /// </summary>
    public readonly IRawSerializable<T> serializable;

    /// <summary>
    /// Creates a new serializable field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="baseSerializable">The underlying serializable.</param>
    public SerializableField(string key, IRawSerializable<T> baseSerializable) : base(key) {
        serializable = baseSerializable;
    }
    /// <summary>
    /// Creates a new serializable field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="baseSerializable">The underlying serializable.</param>
    /// <param name="parent">The parent of this node.</param>
    public SerializableField(string key, IRawSerializable<T> baseSerializable, IParentNode parent) : base(key, parent) {
        serializable = baseSerializable;
    }

    /// <inheritdoc/>
    public uint? FixedSize => serializable.FixedSize;

    /// <inheritdoc/>
    public T Value { get => serializable.Value; set => serializable.Value = value; }

    /// <inheritdoc/>
    public void Parse(byte[] data) {
        serializable.Parse(data);
    }

    /// <inheritdoc/>
    public byte[] Serialize() {
        return serializable.Serialize();
    }
}