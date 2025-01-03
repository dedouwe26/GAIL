namespace GAIL.Serializing;

using System.Text;
using GAIL.Serializing.Streams;

/// <summary>
/// A bool serializable.
/// </summary>
public class BoolSerializable : ByteSerializer {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new ByteSerializable(default);});
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public byte Value;
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
    public BoolSerializable(bool b1, bool b2=false, bool b3=false, bool b4=false, bool b5=false, bool b6=false, bool b7=false, bool b8=false) {
        B1 = b1;
        B2 = b2;
        B3 = b3;
        B4 = b4;
        B5 = b5;
        B6 = b6;
        B7 = b7;
        B8 = b8;
    }
    /// <inheritdoc/>
    public uint? FixedSize => 1;

    /// <inheritdoc/>
    public byte[] Serialize() {
        return [Value];
    }

    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = data[0];
    }
}
/// <summary>
/// A float serializable.
/// </summary>
public class FloatSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new FloatSerializable(default);});
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public float Value;
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public FloatSerializable(float value) {
        Value = value;
    }

    /// <inheritdoc/>
    public uint? FixedSize => 4;

    /// <inheritdoc/>
    public byte[] Serialize() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = BitConverter.IsLittleEndian ? BitConverter.ToSingle(data) : BitConverter.ToSingle(data.Reverse().ToArray());
    }
}
/// <summary>
/// A double serializable.
/// </summary>
public class DoubleSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new DoubleSerializable(default);});
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public double Value;
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public DoubleSerializable(double value) {
        Value = value;
    }
    /// <inheritdoc/>
    public uint? FixedSize => 8;
    /// <inheritdoc/>
    public byte[] Serialize() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }

    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = BitConverter.IsLittleEndian ? BitConverter.ToDouble(data) : BitConverter.ToDouble(data.Reverse().ToArray());
    }
}
/// <summary>
/// A byte serializable.
/// </summary>
public class ByteSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new ByteSerializable(default);});
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public byte Value;
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public ByteSerializable(byte value) {
        Value = value;
    }
    /// <inheritdoc/>
    public uint? FixedSize => 1;
    /// <inheritdoc/>
    public byte[] Serialize() {
        return [Value];
    }

    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = data[0];
    }
}
/// <summary>
/// A short serializable.
/// </summary>
public class ShortSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new ShortSerializable(default);});
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public short Value;
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public ShortSerializable(short value) {
        Value = value;
    }
    /// <inheritdoc/>
    public uint? FixedSize => 2;
    /// <inheritdoc/>
    public byte[] Serialize() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = BitConverter.IsLittleEndian ? BitConverter.ToInt16(data) : BitConverter.ToInt16(data.Reverse().ToArray());
    }
}
/// <summary>
/// A int serializable.
/// </summary>
public class IntSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new IntSerializable(default);});
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public int Value;
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public IntSerializable(int value) {
        Value = value;
    }
    /// <inheritdoc/>
    public uint? FixedSize => 4;
    /// <inheritdoc/>
    public byte[] Serialize() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = BitConverter.IsLittleEndian ? BitConverter.ToInt32(data) : BitConverter.ToInt32(data.Reverse().ToArray());
    }
}
/// <summary>
/// A long serializable.
/// </summary>
public class LongSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new LongSerializable(default);});
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public LongSerializable(long value) {
        Value = value;
    }
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public long Value;
    /// <inheritdoc/>
    public uint? FixedSize => 8;
    /// <inheritdoc/>
    public byte[] Serialize() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = BitConverter.IsLittleEndian ? BitConverter.ToInt64(data) : BitConverter.ToInt64(data.Reverse().ToArray());
    }
}
/// <summary>
/// A signed byte serializable.
/// </summary>
public class SByteSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new SByteSerializable(default);});
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public SByteSerializable(sbyte value) {
        Value = value;
    }
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public sbyte Value;
    /// <inheritdoc/>
    public uint? FixedSize => 1;
    /// <inheritdoc/>
    public byte[] Serialize() {
        return [(byte)Value];
    }
    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = (sbyte)data[0];
    }
}
/// <summary>
/// An unsigned short serializable.
/// </summary>
public class UShortSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new UShortSerializable(default);});
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public UShortSerializable(ushort value) {
        Value = value;
    }
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public ushort Value;
    /// <inheritdoc/>
    public uint? FixedSize => 2;
    /// <inheritdoc/>
    public byte[] Serialize() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = BitConverter.IsLittleEndian ? BitConverter.ToUInt16(data) : BitConverter.ToUInt16(data.Reverse().ToArray());
    }
}
/// <summary>
/// An unsigned int serializable.
/// </summary>
public class UIntSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new UIntSerializable(default);});
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public UIntSerializable(uint value) {
        Value = value;
    }
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public uint Value;
    /// <inheritdoc/>
    public uint? FixedSize => 4;
    /// <inheritdoc/>
    public byte[] Serialize() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = BitConverter.IsLittleEndian ? BitConverter.ToUInt32(data) : BitConverter.ToUInt32(data.Reverse().ToArray());
    }
}
/// <summary>
/// An unsigned long serializable.
/// </summary>
public class ULongSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new ULongSerializable(default);});
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public ULongSerializable(ulong value) {
        Value = value;
    }
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public ulong Value;
    /// <inheritdoc/>
    public uint? FixedSize => 8;
    /// <inheritdoc/>
    public byte[] Serialize() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = BitConverter.IsLittleEndian ? BitConverter.ToUInt64(data) : BitConverter.ToUInt64(data.Reverse().ToArray());
    }
}
/// <summary>
/// A byte array serializable.
/// </summary>
public class BytesSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new BytesSerializable([]);});
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public BytesSerializable(byte[] value) {
        Value = value;
    }
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public byte[] Value;

    /// <inheritdoc/>
    public uint? FixedSize => null;

    /// <inheritdoc/>
    public byte[] Serialize() {
        return BitConverter.IsLittleEndian ? Value : [.. Value.Reverse()];
    }
    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = BitConverter.IsLittleEndian ? data : [.. data.Reverse()];
    }
}
/// <summary>
/// A string serializable (UTF-8).
/// </summary>
public class StringSerializable : ISerializer<T> {
    /// <summary>
    /// Information on how to read and create this serializable.
    /// </summary>
    public readonly static SerializableInfo Info = ISerializable.CreateInfo(() => {return new StringSerializable(string.Empty);});
    /// <summary>
    /// Creates a new serializable.
    /// </summary>
    /// <param name="value">The value of this serializable.</param>
    public StringSerializable(string value) {
        Value = value;
    }
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public string Value;
    /// <inheritdoc/>
    public uint? FixedSize => null;
    /// <inheritdoc/>
    public byte[] Serialize() {
        return BitConverter.IsLittleEndian ? Encoding.UTF8.GetBytes(Value) : Encoding.UTF8.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value = BitConverter.IsLittleEndian ? Encoding.UTF8.GetString(data) : Encoding.UTF8.GetString(data.Reverse().ToArray());
    }
}

/// <summary>
/// A list serializable.
/// </summary>
/// <typeparam name="T">The serializable list type.</typeparam>
public class ListSerializable<T> : ISerializer<T> where T : ISerializer<T> {
    /// <summary>
    /// Creates the info for a list.
    /// </summary>
    /// <param name="info">The info for type <typeparamref name="T"/>.</param>
    /// <returns>A new list serializable info.</returns>
    public static SerializableInfo CreateInfo(SerializableInfo info) {
        return new (null, (byte[] raw) => {
            ListSerializable<T> list = new([], info);
            list.Parse(raw);
            return list;
        });
    }
    /// <summary>
    /// Creates a new serializable list.
    /// </summary>
    /// <param name="value">The list of the serializables.</param>
    /// <param name="info">The serializable info for the type of the list.</param>
    public ListSerializable(List<T> value, SerializableInfo info) {
        Info = info;
        Value = value;
    }
    /// <summary>
    /// The list itself.
    /// </summary>
    public List<T> Value;
    private readonly SerializableInfo Info;
    /// <inheritdoc/>
    public uint? FixedSize => null;
    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value.Clear();

        Parser parser = new(data);

        while (parser.BaseStream.Length > parser.BaseStream.Position) {
            Value.Add(parser.ReadSerializable<T>(Info));
        }
    }

    /// <inheritdoc/>
    public byte[] Serialize() {
        Serializer serializer = new();

        foreach (T serializable in Value) {
            serializer.WriteSerializable(serializable);
        }

        return (serializer.BaseStream as MemoryStream)!.ToArray();
    }
}

/// <summary>
/// A dictionary serializable.
/// </summary>
/// <typeparam name="TKey">The serializable dictionary key type.</typeparam>
/// <typeparam name="TValue">The serializable dictionary value type.</typeparam>
public class DictionarySerializable<TKey, TValue> : ISerializer<T> where TKey : ISerializer<T> where TValue : ISerializer<T> {
    /// <summary>
    /// Creates the info for a dictionary.
    /// </summary>
    /// <param name="keyInfo">The info for type <typeparamref name="TKey"/>.</param>
    /// <param name="valueInfo">The info for type <typeparamref name="TValue"/>.</param>
    /// <returns>A new dictionary serializable info.</returns>
    public static SerializableInfo CreateInfo(SerializableInfo keyInfo, SerializableInfo valueInfo) {
        return new (null, (byte[] raw) => {
            DictionarySerializable<TKey, TValue> dict = new([], keyInfo, valueInfo);
            dict.Parse(raw);
            return dict;
        });
    }
    /// <summary>
    /// Creates a new serializable dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary where the key and the value are serializables.</param>
    /// <param name="keyInfo">The serializable info for the key type of the dictionary.</param>
    /// <param name="valueInfo">The serializable info for the value type of the dictionary.</param>
    public DictionarySerializable(Dictionary<TKey, TValue> dictionary, SerializableInfo keyInfo, SerializableInfo valueInfo) {
        Value = dictionary;
        KeyInfo = keyInfo;
        ValueInfo = valueInfo;
    }

    private readonly SerializableInfo KeyInfo;
    private readonly SerializableInfo ValueInfo;

    /// <summary>
    /// The dictionary itself.
    /// </summary>
    public Dictionary<TKey, TValue> Value;

    /// <inheritdoc/>
    public uint? FixedSize => throw new NotImplementedException();

    /// <inheritdoc/>
    public byte[] Serialize() {
        Serializer serializer = new();

        foreach (KeyValuePair<TKey, TValue> kvp in Value) {
            serializer.WriteSerializable(kvp.Key);
            serializer.WriteSerializable(kvp.Value);
        }

        return (serializer.BaseStream as MemoryStream)!.ToArray();
    }

    /// <inheritdoc/>
    public void Parse(byte[] data) {
        Value.Clear();

        Parser parser = new(data);

        while (parser.BaseStream.Length > parser.BaseStream.Position) {
            Value.Add(parser.ReadSerializable<TKey>(KeyInfo), parser.ReadSerializable<TValue>(ValueInfo));
        }
    }

    
}