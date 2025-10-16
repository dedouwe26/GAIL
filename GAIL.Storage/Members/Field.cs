using System.Diagnostics.Contracts;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;

namespace GAIL.Storage.Members;


/// <summary>
/// Represents a node in a storage file with data.
/// </summary>
public interface IField : IChildNode, ISerializable {
    /// <summary>
	/// The type of this field.
	/// </summary>
    [Pure]
    public MemberType Type { get; }
}
/// <summary>
/// Represents a node in a storage file with an actual value.
/// </summary>
public interface IField<T> : IField, IChildNode, ISerializable<T> { }

/// <summary>
/// Represents a basic field implementation using a serializable.
/// </summary>
public class SerializableField : Node, IField {
    /// <summary>
    /// The underlying serializable.
    /// </summary>
    public readonly ISerializable serializable;

    /// <summary>
    /// Creates a new serializable field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="baseSerializable">The underlying serializable.</param>
    public SerializableField(string key, ISerializable baseSerializable) : base(key) {
        serializable = baseSerializable;
    }
    /// <summary>
    /// Creates a new serializable field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="baseSerializable">The underlying serializable.</param>
    /// <param name="parent">The parent of this node.</param>
    public SerializableField(string key, ISerializable baseSerializable, IParentNode parent) : base(key, parent) {
        serializable = baseSerializable;
    }

    /// <inheritdoc/>
    public void Parse(Parser parser, IFormatter? formatter = null) {
        Parse(parser, readKey:true, formatter);
    }

    /// <inheritdoc/>
    public void Parse(Parser parser, bool readKey = true, IFormatter? formatter = null) {
        if (formatter != null) {
            parser.Decode((p) => {
                Parse(p, readKey, null);
            }, formatter);
        } else {
            if (readKey) key = parser.ReadString();
            serializable.Parse(parser, null);
        }
    }

    /// <inheritdoc/>
    public void Serialize(Serializer serializer, IFormatter? formatter = null) {
        Serialize(serializer, writeKey:true, formatter);
    }

    /// <inheritdoc/>
    public void Serialize(Serializer serializer, bool writeKey = true, IFormatter? formatter = null) {
        if (formatter != null) {
            serializer.Encode((s) => {
                Serialize(s, writeKey, null);
            }, formatter);
        } else {
            if (writeKey) serializer.WriteString(key);
            serializable.Serialize(serializer, null);
        }
    }
}
/// <summary>
/// Represents a basic field implementation using a serializable with a value.
/// </summary>
public class SerializableField<T> : SerializableField, IField<T> {
    private ISerializable<T>? castedSerializable;
    /// <summary>
    /// The underlying serializable.
    /// </summary>
    public ISerializable<T> Serializable { get {
        castedSerializable ??= (ISerializable<T>)serializable;
        return castedSerializable;
    } }
    /// <summary>
    /// Creates a new serializable field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="baseSerializable">The underlying serializable.</param>
    public SerializableField(string key, ISerializable<T> baseSerializable) : base(key, baseSerializable) { }
    /// <summary>
    /// Creates a new serializable field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="baseSerializable">The underlying serializable.</param>
    /// <param name="parent">The parent of this node.</param>
    public SerializableField(string key, ISerializable<T> baseSerializable, IParentNode parent) : base(key, baseSerializable, parent) { }

    /// <inheritdoc/>
    public T Value { get => Serializable.Value; set => Serializable.Value = value; }
}