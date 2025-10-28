using System.Text;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Serializing;

/// <summary>
/// A bool serializable.
/// </summary>
public class BoolSerializable : ByteSerializable {
	private static ISerializable.Info<BoolSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static new ISerializable.Info<BoolSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<BoolSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// The first boolean (msb).
	/// </summary>
	public bool B1 { get => (Value & 0x80) != 0; set { if (value) { Value |= 0x80; } else { Value &= 0x7f; } } }
	/// <summary>
	/// The second boolean.
	/// </summary>
	public bool B2 { get => (Value & 0x40) != 0; set { if (value) { Value |= 0x40; } else { Value &= 0xbf; } } }
	/// <summary>
	/// The third boolean.
	/// </summary>
	public bool B3 { get => (Value & 0x20) != 0; set { if (value) { Value |= 0x20; } else { Value &= 0xdf; } } }
	/// <summary>
	/// The fourth boolean.
	/// </summary>
	public bool B4 { get => (Value & 0x10) != 0; set { if (value) { Value |= 0x10; } else { Value &= 0xef; } } }
	/// <summary>
	/// The fifth boolean.
	/// </summary>
	public bool B5 { get => (Value & 0x08) != 0; set { if (value) { Value |= 0x08; } else { Value &= 0xf7; } } }
	/// <summary>
	/// The sixth boolean.
	/// </summary>
	public bool B6 { get => (Value & 0x04) != 0; set { if (value) { Value |= 0x04; } else { Value &= 0xfb; } } }
	/// <summary>
	/// The seventh boolean.
	/// </summary>
	public bool B7 { get => (Value & 0x02) != 0; set { if (value) { Value |= 0x02; } else { Value &= 0xfd; } } }
	/// <summary>
	/// The eighth boolean (lsb).
	/// </summary>
	public bool B8 { get => (Value & 0x01) != 0; set { if (value) { Value |= 0x01; } else { Value &= 0xfe; } } }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public BoolSerializable() : this(default) { }
	/// <summary>
	/// Creates a new serializable (stores 8 bools in 1 byte).
	/// </summary>
	/// <param name="b1">The first boolean (msb).</param>
	/// <param name="b2">The second boolean.</param>
	/// <param name="b3">The third boolean.</param>
	/// <param name="b4">The fourth boolean.</param>
	/// <param name="b5">The fifth boolean.</param>
	/// <param name="b6">The sixth boolean.</param>
	/// <param name="b7">The seventh boolean.</param>
	/// <param name="b8">The eighth boolean (lsb).</param>
	public BoolSerializable(bool b1,
		bool b2=default, bool b3=default, bool b4=default, bool b5=default, bool b6=default, bool b7=default, bool b8=default) : base((byte)(
			(b1 ? 0x80 : 0x00)|(b2 ? 0x40 : 0x00)|
			(b3 ? 0x20 : 0x00)|(b4 ? 0x10 : 0x00)|
			(b5 ? 0x08 : 0x00)|(b6 ? 0x04 : 0x00)|
			(b7 ? 0x02 : 0x00)|(b8 ? 0x01 : 0x00)
		)
	) { }
}
/// <summary>
/// A float serializable.
/// </summary>
public class FloatSerializable : RawSerializable<float> {
	private static ISerializable.Info<FloatSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<FloatSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<FloatSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public FloatSerializable() : this(default) { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public FloatSerializable(float value) : base(value) { }

	/// <inheritdoc/>
	public override uint? FixedSize => 4;

	/// <inheritdoc/>
	public override byte[] Serialize() {
		return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
	}
	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = BitConverter.IsLittleEndian ? BitConverter.ToSingle(data) : BitConverter.ToSingle(data.Reverse().ToArray());
	}
}
/// <summary>
/// A double serializable.
/// </summary>
public class DoubleSerializable : RawSerializable<double> {
	private static ISerializable.Info<DoubleSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<DoubleSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<DoubleSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public DoubleSerializable() : this(default) { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public DoubleSerializable(double value) : base(value) { }
	/// <inheritdoc/>
	public override uint? FixedSize => 8;
	/// <inheritdoc/>
	public override byte[] Serialize() {
		return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
	}

	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = BitConverter.IsLittleEndian ? BitConverter.ToDouble(data) : BitConverter.ToDouble(data.Reverse().ToArray());
	}
}
/// <summary>
/// A byte serializable.
/// </summary>
public class ByteSerializable : RawSerializable<byte> {
	private static ISerializable.Info<ByteSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<ByteSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<ByteSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public ByteSerializable() : this(default) { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public ByteSerializable(byte value) : base(value) { }
	/// <inheritdoc/>
	public override uint? FixedSize => 1;
	/// <inheritdoc/>
	public override byte[] Serialize() {
		return [Value];
	}

	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = data[0];
	}
}
/// <summary>
/// A short serializable.
/// </summary>
public class ShortSerializable : RawSerializable<short> {
	private static ISerializable.Info<ShortSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<ShortSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<ShortSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public ShortSerializable() : this(default) { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public ShortSerializable(short value) : base(value) { }
	/// <inheritdoc/>
	public override uint? FixedSize => 2;
	/// <inheritdoc/>
	public override byte[] Serialize() {
		return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
	}
	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = BitConverter.IsLittleEndian ? BitConverter.ToInt16(data) : BitConverter.ToInt16(data.Reverse().ToArray());
	}
}
/// <summary>
/// A int serializable.
/// </summary>
public class IntSerializable : RawSerializable<int> {
	private static ISerializable.Info<IntSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<IntSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<IntSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public IntSerializable() : this(default) { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public IntSerializable(int value) : base(value) { }
	/// <inheritdoc/>
	public override uint? FixedSize => 4;
	/// <inheritdoc/>
	public override byte[] Serialize() {
		return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
	}
	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = BitConverter.IsLittleEndian ? BitConverter.ToInt32(data) : BitConverter.ToInt32(data.Reverse().ToArray());
	}
}
/// <summary>
/// A long serializable.
/// </summary>
public class LongSerializable : RawSerializable<long> {
	private static ISerializable.Info<LongSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<LongSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<LongSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public LongSerializable() : this(default) { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public LongSerializable(long value) : base(value) { }
	/// <inheritdoc/>
	public override uint? FixedSize => 8;
	/// <inheritdoc/>
	public override byte[] Serialize() {
		return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
	}
	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = BitConverter.IsLittleEndian ? BitConverter.ToInt64(data) : BitConverter.ToInt64(data.Reverse().ToArray());
	}
}
/// <summary>
/// A signed byte serializable.
/// </summary>
public class SByteSerializable : RawSerializable<sbyte> {
	private static ISerializable.Info<SByteSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<SByteSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<SByteSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public SByteSerializable() : this(default) { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public SByteSerializable(sbyte value) : base(value) { }
	/// <inheritdoc/>
	public override uint? FixedSize => 1;
	/// <inheritdoc/>
	public override byte[] Serialize() {
		return [(byte)Value];
	}
	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = (sbyte)data[0];
	}
}
/// <summary>
/// An unsigned short serializable.
/// </summary>
public class UShortSerializable : RawSerializable<ushort> {
	private static ISerializable.Info<UShortSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<UShortSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<UShortSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public UShortSerializable() : this(default) { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public UShortSerializable(ushort value) : base(value) { }
	/// <inheritdoc/>
	public override uint? FixedSize => 2;
	/// <inheritdoc/>
	public override byte[] Serialize() {
		return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
	}
	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = BitConverter.IsLittleEndian ? BitConverter.ToUInt16(data) : BitConverter.ToUInt16(data.Reverse().ToArray());
	}
}
/// <summary>
/// An unsigned int serializable.
/// </summary>
public class UIntSerializable : RawSerializable<uint> {
	private static ISerializable.Info<UIntSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<UIntSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<UIntSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public UIntSerializable() : this(default) { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public UIntSerializable(uint value) : base(value) { }
	/// <inheritdoc/>
	public override uint? FixedSize => 4;

	/// <inheritdoc/>
	public override byte[] Serialize() {
		return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
	}
	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = BitConverter.IsLittleEndian ? BitConverter.ToUInt32(data) : BitConverter.ToUInt32(data.Reverse().ToArray());
	}
}
/// <summary>
/// An unsigned long serializable.
/// </summary>
public class ULongSerializable : RawSerializable<ulong> {
	private static ISerializable.Info<ULongSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<ULongSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<ULongSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public ULongSerializable() : this(default) { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public ULongSerializable(ulong value) : base(value) { }
	/// <inheritdoc/>
	public override uint? FixedSize => 8;
	/// <inheritdoc/>
	public override byte[] Serialize() {
		return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
	}
	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = BitConverter.IsLittleEndian ? BitConverter.ToUInt64(data) : BitConverter.ToUInt64(data.Reverse().ToArray());
	}
}
/// <summary>
/// A byte array serializable.
/// </summary>
public class BytesSerializable : RawSerializable<byte[]> {
	private static ISerializable.Info<BytesSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<BytesSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<BytesSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public BytesSerializable() : this([]) { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public BytesSerializable(byte[] value) : base(value) { }

	/// <inheritdoc/>
	public override uint? FixedSize => null;

	/// <inheritdoc/>
	public override byte[] Serialize() {
		return BitConverter.IsLittleEndian ? Value : [.. Value.Reverse()];
	}
	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = BitConverter.IsLittleEndian ? data : [.. data.Reverse()];
	}
}
/// <summary>
/// A string serializable (UTF-8).
/// </summary>
public class StringSerializable : RawSerializable<string> {
	private static ISerializable.Info<StringSerializable>? info;
	/// <summary>
	/// Information on how to read and create this serializable.
	/// </summary>
	public static ISerializable.Info<StringSerializable> Info { get {
		if (info == null) {
			info = ISerializable.CreateInfo<StringSerializable>();
		}
		return info;
	} }
	/// <summary>
	/// Creates an empty serializable.
	/// </summary>
	public StringSerializable() : this("") { }
	/// <summary>
	/// Creates a new serializable.
	/// </summary>
	/// <param name="value">The value of this serializable.</param>
	public StringSerializable(string value) : base(value) { }
	/// <inheritdoc/>
	public override uint? FixedSize => null;
	/// <inheritdoc/>
	public override byte[] Serialize() {
		return BitConverter.IsLittleEndian ? Encoding.UTF8.GetBytes(Value) : Encoding.UTF8.GetBytes(Value).Reverse().ToArray();
	}
	/// <inheritdoc/>
	public override void Parse(byte[] data) {
		Value = BitConverter.IsLittleEndian ? Encoding.UTF8.GetString(data) : Encoding.UTF8.GetString(data.Reverse().ToArray());
	}
}

/// <summary>
/// A list serializable.
/// </summary>
/// <typeparam name="T">The serializable list type.</typeparam>
public class ListSerializable<T> : ISerializable<IEnumerable<T>> where T : ISerializable {
	/// <summary>
	/// Creates the info for a list.
	/// </summary>
	/// <param name="typeInfo">The serializable info of type <typeparamref name="T"/>.</param>
	/// <returns>A new list serializable info.</returns>
	public static ISerializable.Info<ListSerializable<T>> CreateInfo(ISerializable.Info typeInfo) {
		return new((p, f) => {
			ListSerializable<T> list = new(typeInfo);
			list.Parse(p, f);
			return list;
		});
	}

	private IEnumerable<T>? value;
	/// <inheritdoc/>
	public IEnumerable<T> Value { get => value ?? throw new InvalidOperationException("This list serializable is not serializer- and usage-ready"); set => this.value = value; }

	/// <summary>
	/// Creates a parser-ready list serializable.
	/// </summary>
	/// <param name="valueInfo">The serializable info of type <typeparamref name="T"/>.</param>
	public ListSerializable(ISerializable.Info valueInfo) {
		this.valueInfo = valueInfo;
	}
	/// <summary>
	/// Creates a serializer-ready list serializable.
	/// </summary>
	/// <param name="value">The list of the serializables.</param>
	public ListSerializable(IEnumerable<T> value) {
		Value = value;
	}
	private readonly ISerializable.Info? valueInfo;
	/// <summary>
	/// The serializable info of the values.
	/// </summary>
	public ISerializable.Info ValueInfo => valueInfo ?? throw new InvalidOperationException("This list serializable is not parser-ready");
	/// <inheritdoc/>
	public void Serialize(Serializer serializer, IFormatter? formatter = null) {
		if (formatter != null) {
			serializer.Encode((s) => {
				SerializeWithoutCount(s, null);
			}, formatter);
		} else {
			serializer.WriteUInt((uint)Value.Count());
			foreach (T item in Value) {
				serializer.WriteSerializable(item);
			}
		}
		
	}
	/// <summary>
	/// Serializes the dictionary without an count.
	/// </summary>
	/// <param name="serializer">The serializer to write to.</param>
	/// <param name="formatter">The formatter to use.</param>
	public void SerializeWithoutCount(Serializer serializer, IFormatter? formatter = null) {
		if (formatter != null) {
			serializer.Encode((s) => {
				SerializeWithoutCount(s, null);
			}, formatter);
		} else {
			foreach (T item in Value) {
				serializer.WriteSerializable(item);
			}
		}
	}

	/// <inheritdoc/>
	public void Parse(Parser parser, IFormatter? formatter = null) {
		if (formatter != null) {
			parser.Decode((p) => {
				ParseWithoutCount(p, null);
			}, formatter);
		} else {
			uint count = parser.ReadUInt();
			Value = new T[count];
			uint actualCount = 0;
			while (actualCount < count) {
				((T[])Value)[actualCount++] = parser.ReadSerializable<T>(ValueInfo);
			}
		}
	}
	/// <summary>
	/// Parses the whole stream.
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	/// <param name="formatter">The formatter to use.</param>
	public void ParseWithoutCount(Parser parser, IFormatter? formatter = null) {
		if (formatter != null) {
			parser.Decode((p) => {
				ParseWithoutCount(p, null);
			}, formatter);
		} else {
			uint count = parser.ReadUInt();
			Value = new T[count];
			uint actualCount = 0;
			while (parser.BaseStream.Length > parser.BaseStream.Position) {
				((T[])Value)[actualCount++] = parser.ReadSerializable<T>(ValueInfo);
			}
		}
	}
}

/// <summary>
/// A dictionary serializable.
/// </summary>
/// <typeparam name="TKey">The serializable dictionary key type.</typeparam>
/// <typeparam name="TValue">The serializable dictionary value type.</typeparam>
public class DictionarySerializable<TKey, TValue> : ISerializable<IDictionary<TKey, TValue>> where TKey : ISerializable where TValue : ISerializable {
	/// <summary>
	/// Creates the info for a dictionary.
	/// </summary>
	/// <param name="keyInfo">The info for type <typeparamref name="TKey"/>.</param>
	/// <param name="valueInfo">The info for type <typeparamref name="TValue"/>.</param>
	/// <returns>A new dictionary serializable info.</returns>
	public static ISerializable.Info<DictionarySerializable<TKey, TValue>> CreateInfo(ISerializable.Info keyInfo, ISerializable.Info valueInfo) {
		return new((p, f) => {
			DictionarySerializable<TKey, TValue> dict = new(keyInfo, valueInfo);
			dict.Parse(p, f);
			return dict;
		});
	}
	private IDictionary<TKey, TValue>? value;
	/// <inheritdoc/>
	public IDictionary<TKey, TValue> Value { get => value ?? throw new InvalidOperationException("This list serializable is not serializer- and usage-ready"); set => this.value = value; }

	private readonly ISerializable.Info? keyInfo;
	private readonly ISerializable.Info? valueInfo;
	private ISerializable.Info KeyInfo => keyInfo ?? throw new InvalidOperationException("This dictionary serializable is not parser-ready");
	private ISerializable.Info ValueInfo => valueInfo ?? throw new InvalidOperationException("This dictionary serializable is not parser-ready");
	/// <summary>
	/// Creates a parser-ready dictionary serializable.
	/// </summary>
	/// <param name="keyInfo">The serializable info for the key type of the dictionary.</param>
	/// <param name="valueInfo">The serializable info for the value type of the dictionary.</param>
	public DictionarySerializable(ISerializable.Info keyInfo, ISerializable.Info valueInfo) {
		this.keyInfo = keyInfo;
		this.valueInfo = valueInfo;
	}
	/// <summary>
	/// Creates a serializer-ready dictionary serializable.
	/// </summary>
	/// <param name="dictionary">The dictionary where the key and the value are serializables.</param>
	public DictionarySerializable(Dictionary<TKey, TValue> dictionary) {
		Value = dictionary;
	}
	/// <inheritdoc/>
	public void Serialize(Serializer serializer, IFormatter? formatter = null) {
		if (formatter != null) {
			serializer.Encode((s) => {
				SerializeWithoutCount(s, null);
			}, formatter);
		} else {
			serializer.WriteUInt((uint)Value.Count);
			foreach (KeyValuePair<TKey, TValue> kvp in Value) {
				serializer.WriteSerializable(kvp.Key);
				serializer.WriteSerializable(kvp.Value);
			}
		}
		
	}
	/// <summary>
	/// Serializes the dictionary without an count.
	/// </summary>
	/// <param name="serializer">The serializer to write to.</param>
	/// <param name="formatter">The formatter to use.</param>
	public void SerializeWithoutCount(Serializer serializer, IFormatter? formatter = null) {
		if (formatter != null) {
			serializer.Encode((s) => {
				SerializeWithoutCount(s, null);
			}, formatter);
		} else {
			foreach (KeyValuePair<TKey, TValue> kvp in Value) {
				serializer.WriteSerializable(kvp.Key);
				serializer.WriteSerializable(kvp.Value);
			}
		}
		
		
	}

	/// <inheritdoc/>
	public void Parse(Parser parser, IFormatter? formatter = null) {
		Value.Clear();
		if (formatter != null) {
			parser.Decode((p) => {
				ParseWithoutCount(p, null);
			}, formatter);
		} else {
			uint count = parser.ReadUInt();
			while (Value.Count < count) {
				Value.Add(parser.ReadSerializable<TKey>(KeyInfo), parser.ReadSerializable<TValue>(ValueInfo));
			}
		}
	}
	/// <summary>
	/// Parses the whole stream.
	/// </summary>
	/// <param name="parser">The parser to read from.</param>
	/// <param name="formatter">The formatter to use.</param>
	public void ParseWithoutCount(Parser parser, IFormatter? formatter = null) {
		Value.Clear();
		if (formatter != null) {
			parser.Decode((p) => {
				ParseWithoutCount(p, null);
			}, formatter);
		} else {
			while (parser.BaseStream.Length > parser.BaseStream.Position) {
				Value.Add(parser.ReadSerializable<TKey>(KeyInfo), parser.ReadSerializable<TValue>(ValueInfo));
			}
		}
	}
}