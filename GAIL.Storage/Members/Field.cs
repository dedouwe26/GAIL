using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Streams;

namespace GAIL.Storage.Members;

/// <summary>
/// Represents a node in a storage file with data.
/// </summary>
public interface IField : IChildNode, ISerializable {
	/// <summary>
	/// Creates a serializable info.
	/// </summary>
	/// <param name="instantiator">The instantiator of the serializable.</param>
	/// <param name="writeType">Represents the method used to write the type.</param>
	/// <returns>A new serializable info.</returns>
	public static Info CreateInfo(Func<IField> instantiator, Action<Serializer> writeType) {
		return new ((p, hasKey, f) => {
			IField inst = instantiator();
			inst.Parse(p, hasKey, f);
			return inst;
		}, writeType);
	}
	/// <summary>
	/// Represents how to instantiate a field.
	/// </summary>
	public new class Info {
		/// <summary>
		/// Instantiates this class.
		/// </summary>
		/// <param name="fieldCreator">The instance creator of the field.</param>
		/// <param name="writeType">Represents the method used to write the type.</param>
		public Info(Func<Parser, bool, IFormatter?, IField> fieldCreator, Action<Serializer> writeType) {
			FieldCreator = fieldCreator;
			WriteType = writeType;
		}
		/// <summary>
		/// Represents the method used to write the type.
		/// </summary>
		public Action<Serializer> WriteType;
		/// <summary>
		/// The instance creator of the field.
		/// </summary>
		public Func<Parser, bool, IFormatter?, IField> FieldCreator { get; init; }
		/// <summary>
		/// Converts this field info to a serializable info.
		/// </summary>
		/// <param name="hasKey">Whether it has a key.</param>
		/// <returns></returns>
		public ISerializable.Info Convert(bool hasKey) {
			return new ISerializable.Info((p, f) => FieldCreator(p, hasKey, f));
		}
	}

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
	/// <summary>
	/// Writes the fully qualified type to the serializer.
	/// </summary>
	/// <param name="serializer">The serializer to write to.</param>
	public void WriteType(Serializer serializer);
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

	/// <inheritdoc/>
	public virtual void WriteType(Serializer serializer) {
		serializer.WriteType(Type);
	}

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
public interface IField<T> : IField, ISerializable<T> { }

/// <summary>
/// Represents a basic field implementation using a serializable.
/// </summary>
public abstract class WrapperField : ChildNode, IField {
	private ISerializable? serializable;
	/// <summary>
	/// The underlying serializable.
	/// </summary>
	public ISerializable Serializable { get => serializable ?? throw new InvalidOperationException("The serializable has no value"); }
	private readonly ISerializable.Info? baseInfo;
	private ISerializable.Info BaseInfo { get => baseInfo ?? throw new InvalidOperationException("There is no base info defined"); }

	/// <summary>
	/// Creates a parser-ready serializable field.
	/// </summary>
	/// <param name="baseInfo">The info of the base serializable.</param>
	protected WrapperField(ISerializable.Info baseInfo) : base("") {
		this.baseInfo = baseInfo;
	}
	/// <summary>
	/// Creates a serializer-ready serializable field.
	/// </summary>
	/// <param name="key">The key of this field.</param>
	/// <param name="baseSerializable">The underlying serializable.</param>
	public WrapperField(string key, ISerializable baseSerializable) : base(key) {
		serializable = baseSerializable;
	}
	/// <summary>
	/// Creates a serializer- and node-ready serializable field.
	/// </summary>
	/// <param name="key">The key of this field.</param>
	/// <param name="baseSerializable">The underlying serializable.</param>
	/// <param name="parent">The parent of this node.</param>
	public WrapperField(string key, ISerializable baseSerializable, IParentNode parent) : base(key, parent) {
		serializable = baseSerializable;
	}


	void ISerializable.Parse(Parser parser, IFormatter? formatter) {
		Parse(parser, true, formatter);
	}
	void ISerializable.Serialize(Serializer serializer, IFormatter? formatter) {
		Serialize(serializer, true, formatter);
	}

	/// <inheritdoc/>
	public virtual void Parse(Parser parser, bool hasKey, IFormatter? formatter = null) {
		if (formatter != null) {
			parser.Decode(p => Parse(p, hasKey, null), formatter);
		} else {
			if (hasKey) Key = parser.ReadString();
			serializable = parser.ReadSerializable(BaseInfo);
		}
	}

	/// <inheritdoc/>
	public virtual void Serialize(Serializer serializer, bool hasKey, IFormatter? formatter = null) {
		if (formatter != null) {
			serializer.Encode(s => Serialize(s, hasKey, null), formatter);
		} else {
			if (hasKey) serializer.WriteString(Key);
			Serializable.Serialize(serializer, null);
		}
	}

	/// <inheritdoc/>
	public abstract void WriteType(Serializer serializer);
}
/// <summary>
/// Represents a basic field implementation using a serializable with a value.
/// </summary>
public abstract class WrapperField<T> : WrapperField, IField<T> {
	/// <summary>
	/// The underlying serializable.
	/// </summary>
	public new ISerializable<T> Serializable => (ISerializable<T>)base.Serializable;
	/// <summary>
	/// Creates a parser-ready serializable field.
	/// </summary>
	/// <param name="baseInfo">The info of the base serializable.</param>
	protected WrapperField(ISerializable.Info baseInfo) : base(baseInfo) { }
	/// <summary>
	/// Creates a new serializable field.
	/// </summary>
	/// <param name="key">The key of this field.</param>
	/// <param name="baseSerializable">The underlying serializable.</param>
	public WrapperField(string key, ISerializable<T> baseSerializable) : base(key, baseSerializable) { }
	/// <summary>
	/// Creates a new serializable field.
	/// </summary>
	/// <param name="key">The key of this field.</param>
	/// <param name="baseSerializable">The underlying serializable.</param>
	/// <param name="parent">The parent of this node.</param>
	public WrapperField(string key, ISerializable<T> baseSerializable, IParentNode parent) : base(key, baseSerializable, parent) { }

	/// <inheritdoc/>
	public T Value { get => Serializable.Value; set => Serializable.Value = value; }
}