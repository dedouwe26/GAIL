using System.Reflection.Metadata.Ecma335;
using GAIL.Serializing;
using GAIL.Serializing.Streams;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Streams;

namespace GAIL.Storage.Members;

/// <summary>
/// Represents a basicly implemented field.
/// </summary>
public class BasicField : WrapperField {
	/// <summary>
	/// Creates a field info for this field.
	/// </summary>
	/// <param name="type">The type of this field.</param>
	/// <param name="baseInfo">The base info on which this field is based.</param>
	/// <returns></returns>
	public static IField.Info CreateInfo(MemberType type, ISerializable.Info baseInfo) {
		return new((p, hasKey, f) => {
			BasicField s = new(type, baseInfo);
			s.Parse(p, hasKey, f);
			return s;
		}, (s) => WriteType(s, type));
	}
	
	/// <summary>
	/// Creates a parser-ready serializable field.
	/// </summary>
	/// <param name="type">The type of this field.</param>
	/// <param name="baseInfo">The info of the base serializable.</param>
	public BasicField(MemberType type, ISerializable.Info baseInfo) : base(baseInfo) {
		Type = type;
	}

	/// <summary>
	/// Creates a serializer-ready serializable field.
	/// </summary>
	/// <param name="type">The type of this field.</param>
	/// <param name="key">The key of this field.</param>
	/// <param name="baseSerializable">The underlying serializable.</param>
	public BasicField(string key, MemberType type, ISerializable baseSerializable) : base(key, baseSerializable) {
		Type = type;
	}

	/// <summary>
	/// Creates a serializer- and node-ready serializable field.
	/// </summary>
	/// <param name="type">The type of this field.</param>
	/// <param name="key">The key of this field.</param>
	/// <param name="baseSerializable">The underlying serializable.</param>
	/// <param name="parent">The parent of this node.</param>
	public BasicField(string key, IParentNode parent, MemberType type, ISerializable baseSerializable) : base(key, baseSerializable, parent) {
		Type = type;
	}

	/// <inheritdoc/>
	public MemberType Type { get; }
	private static void WriteType(Serializer s, MemberType type) {
		s.WriteType(type);
	}
	/// <inheritdoc/>
	public override void WriteType(Serializer serializer) {
		WriteType(serializer, Type);
	}
}

/// <summary>
/// Represents a basicly implemented field.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public class BasicField<T> : BasicField, IField<T> where T : notnull {
	/// <summary>
	/// Creates a field info for this field.
	/// </summary>
	/// <param name="type">The type of this field.</param>
	/// <param name="baseInfo">The base info on which this field is based.</param>
	/// <returns></returns>
	public static new IField.Info CreateInfo(MemberType type, ISerializable.Info baseInfo) {
		return new((p, hasKey, f) => {
			BasicField<T> s = new(type, baseInfo);
			s.Parse(p, hasKey, f);
			return s;
		}, (s) => WriteType(s, type));
	}
	private static readonly KeyValuePair<MemberType, Func<object, ISerializable>>[] creators = [
		new(MemberType.Int, (v) => new IntSerializable((int)v)),
		new(MemberType.String, (v) => new StringSerializable((string)v)),
		new(MemberType.Bool, (v) => new BoolSerializable((bool)v)),
		new(MemberType.Float, (v) => new FloatSerializable((float)v)),
		new(MemberType.Byte, (v) => new ByteSerializable((byte)v)),
		new(MemberType.Long, (v) => new LongSerializable((long)v)),
		new(MemberType.Double, (v) => new DoubleSerializable((double)v)),
		new(MemberType.Short, (v) => new ShortSerializable((short)v)),
		new(MemberType.Bytes, (v) => new BytesSerializable((byte[])v)),
		new(MemberType.SByte, (v) => new SByteSerializable((sbyte)v)),
		new(MemberType.UShort, (v) => new UShortSerializable((ushort)v)),
		new(MemberType.UInt, (v) => new UIntSerializable((uint)v)),
		new(MemberType.ULong, (v) => new ULongSerializable((ulong)v))
	];
	/// <summary>
	/// A utility to create a basic field.
	/// </summary>
	/// <param name="type">The type of this field.</param>
	/// <param name="value">The value of this field, depends on the type.</param>
	/// <returns>A new basic field.</returns>
	public static BasicField<T> Create(MemberType type, T value) {
		return new BasicField<T>("", type, (creators.First((p) => p.Key == type).Value(value) as ISerializable<T>)!);
	}
	/// <summary>
	/// A utility to create a basic field.
	/// </summary>
	/// <param name="key">The key of the basic field.</param>
	/// <param name="type">The type of this field.</param>
	/// <param name="value">The value of this field, depends on the type.</param>
	/// <returns>A new basic field.</returns>
	public static BasicField<T> Create(string key, MemberType type, T value) {
		return new BasicField<T>(key, type, (creators.First((p) => p.Key == type).Value(value) as ISerializable<T>)!);
	}
	/// <summary>
	/// A utility to create a basic field.
	/// </summary>
	/// <param name="key">The key of the basic field.</param>
	/// <param name="type">The type of this field.</param>
	/// <param name="value">The value of this field, depends on the type.</param>
	/// <param name="parent">The parent of this field.</param>
	/// <returns>A new basic field.</returns>
	public static BasicField<T> Create(string key, IParentNode parent, MemberType type, T value) {
		return new BasicField<T>(key, parent, type, (creators.First((p) => p.Key == type).Value(value) as ISerializable<T>)!);
	}
	private static void WriteType(Serializer s, MemberType type) {
		s.WriteType(type);
	}
	/// <summary>
	/// The underlying serializable.
	/// </summary>
	public ISerializable<T> CastedSerializable { get => (ISerializable<T>)Serializable; }
	/// <inheritdoc/>
	public T Value { get => CastedSerializable.Value; set => CastedSerializable.Value = value; }

	/// <inheritdoc/>
	public BasicField(MemberType type, ISerializable.Info baseInfo) : base(type, baseInfo) { }

	/// <summary>
	/// Creates a serializer-ready serializable field.
	/// </summary>
	/// <param name="type">The type of this field.</param>
	/// <param name="key">The key of this field.</param>
	/// <param name="baseSerializable">The underlying serializable.</param>
	public BasicField(string key, MemberType type, ISerializable<T> baseSerializable) : base(key, type, baseSerializable) { }

	/// <summary>
	/// Creates a serializer- and node-ready serializable field.
	/// </summary>
	/// <param name="type">The type of this field.</param>
	/// <param name="key">The key of this field.</param>
	/// <param name="baseSerializable">The underlying serializable.</param>
	/// <param name="parent">The parent of this node.</param>
	public BasicField(string key, IParentNode parent, MemberType type, ISerializable<T> baseSerializable) : base(key, parent, type, baseSerializable) { }
}