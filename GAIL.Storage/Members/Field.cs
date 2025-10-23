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

	/// <summary>
	/// Turns this class into bytes.
	/// </summary>
	/// <param name="serializer">The serializer to write to.</param>
	/// <param name="hasKey">Whether the key should be written.</param>
	/// <param name="formatter">The formatter to use.</param>
	public void Serialize(Serializer serializer, bool hasKey, IFormatter? formatter = null);
	/// <summary>
	/// Creates this class from bytes.
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	/// <param name="hasKey">Whether the key should be read.</param>
	/// <param name="formatter">The formatter to use.</param>
	public void Parse(Parser parser, bool hasKey, IFormatter? formatter = null);
}
/// <summary>
/// Implements some common methods of IField.
/// </summary>
/// <inheritdoc/>
public abstract class Field(string key, IParentNode parent) : ChildNode(key, parent), IField {
	/// <inheritdoc/>
	public abstract MemberType Type { get; }
	/// <inheritdoc/>
	public abstract void Parse(Parser parser, bool hasKey, IFormatter? formatter = null);
	/// <inheritdoc/>
	public abstract void Serialize(Serializer serializer, bool hasKey, IFormatter? formatter = null);

	void ISerializable.Parse(Parser parser, IFormatter? formatter) {
		Parse(parser, true, formatter);
	}
	void ISerializable.Serialize(Serializer serializer, IFormatter? formatter) {
		Serialize(serializer, true, formatter);
	}
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
	public virtual MemberType Type => MemberType.Custom;

	/// <inheritdoc/>
	public void Parse(Parser parser, IFormatter? formatter = null) {
        Parse(parser, true, formatter);
    }

    /// <inheritdoc/>
    public void Parse(Parser parser, bool hasKey, IFormatter? formatter = null) {
        if (formatter != null) {
            parser.Decode(p => Parse(p, hasKey, null), formatter);
        } else {
            if (hasKey) key = parser.ReadString();
            serializable.Parse(parser, null);
        }
    }

    /// <inheritdoc/>
    public void Serialize(Serializer serializer, IFormatter? formatter = null) {
        Serialize(serializer, true, formatter);
    }

    /// <inheritdoc/>
    public void Serialize(Serializer serializer, bool hasKey, IFormatter? formatter = null) {
        if (formatter != null) {
            serializer.Encode(s => Serialize(s, hasKey, null), formatter);
        } else {
            if (hasKey) serializer.WriteString(key);
            serializable.Serialize(serializer, null);
        }
    }
}
/// <summary>
/// Represents a basic field implementation using a serializable with a value.
/// </summary>
public class SerializableField<T> : SerializableField, IField<T> {
    /// <summary>
    /// The underlying serializable.
    /// </summary>
    public ISerializable<T> Serializable { get {
        return (ISerializable<T>)serializable;
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