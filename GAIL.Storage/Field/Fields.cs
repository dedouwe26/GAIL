using GAIL.Serializing;

namespace GAIL.Storage;

/// <summary>
/// A field with an underlying serializable.
/// </summary>
public class SerializableField<T> : Field where T : ISerializable {
    /// <summary>
    /// The underlying serializable.
    /// </summary>
    public T Serializable { get; private set; }
    /// <summary>
    /// Creates a serializable field from a key and a serializable.
    /// </summary>
    /// <param name="key">The key for the child node.</param>
    /// <param name="serializable">The serializable that the field will use.</param>
    public SerializableField(string key, T serializable) : base(key) {
        Serializable = serializable;
    }
    /// <summary>
    /// Creates a serializable field from a key and a serializable.
    /// </summary>
    /// <param name="key">The key for the child node.</param>
    /// <param name="parent">The parent of the child node.</param>
    /// <param name="serializable">The serializable that the field will use.</param>
    /// <exception cref="ArgumentException">Parent is not a container or a storage node.</exception>
    public SerializableField(string key, IParentNode parent, T serializable) : base(key, parent) {
        Serializable = serializable;
    }
    /// <inheritdoc/>
    public override uint? FixedSize => Serializable.FixedSize;
    /// <inheritdoc/>
    public override void Parse(byte[] data) {
        Serializable.Parse(data);
    }
    /// <inheritdoc/>
    public override byte[] Serialize() {
        return Serializable.Serialize();
    }
}

/// <summary>
/// A field with an underlying serializable.
/// </summary>
public class SimpleField : SerializableField<ISerializable> {
    /// <inheritdoc/>
    public SimpleField(string key, ISerializable serializable) : base(key, serializable) { }
    /// <inheritdoc/>
    public SimpleField(string key, IParentNode parent, ISerializable serializable) : base(key, parent, serializable) { }
}

/// <inheritdoc/>
public class BoolField : SerializableField<BoolSerializable> {
    /// <summary>
    /// The value of this serializable.
    /// </summary>
    public byte Value { get => Serializable.Value; set => Serializable.Value = value; }
    /// <summary>
    /// The first boolean (msb).
    /// </summary>
    public bool B1 { get => Serializable.B1; set => Serializable.B1 = value; }
    /// <summary>
    /// The second boolean.
    /// </summary>
    public bool B2 { get => Serializable.B2; set => Serializable.B2 = value; }
    /// <summary>
    /// The third boolean.
    /// </summary>
    public bool B3 { get => Serializable.B3; set => Serializable.B3 = value; }
    /// <summary>
    /// The fourth boolean.
    /// </summary>
    public bool B4 { get => Serializable.B4; set => Serializable.B4 = value; }
    /// <summary>
    /// The fifth boolean.
    /// </summary>
    public bool B5 { get => Serializable.B5; set => Serializable.B5 = value; }
    /// <summary>
    /// The sixth boolean.
    /// </summary>
    public bool B6 { get => Serializable.B6; set => Serializable.B6 = value; }
    /// <summary>
    /// The seventh boolean.
    /// </summary>
    public bool B7 { get => Serializable.B7; set => Serializable.B7 = value; }
    /// <summary>
    /// The eighth boolean (lsb).
    /// </summary>
    public bool B8 { get => Serializable.B8; set => Serializable.B8 = value; }
    /// <inheritdoc/>
    public BoolField(string key, bool b1, bool b2=false, bool b3=false, bool b4=false, bool b5=false, bool b6=false, bool b7=false, bool b8=false) : base(key, new (b1, b2, b3, b4, b5, b6, b7, b8)) { }
    /// <inheritdoc/>
    public BoolField(string key, IParentNode parent, bool b1, bool b2=false, bool b3=false, bool b4=false, bool b5=false, bool b6=false, bool b7=false, bool b8=false) : base(key, parent, new (b1, b2, b3, b4, b5, b6, b7, b8)) { }
}

/// <inheritdoc/>
public class FloatField : SerializableField<FloatSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public float Value => Serializable.Value;
    ///
    public FloatField(string key, float value) : base(key, new (value)) { }
    ///
    public FloatField(string key, IParentNode parent, float value) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public class DoubleField : SerializableField<DoubleSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public double Value => Serializable.Value;
    /// <inheritdoc/>
    public DoubleField(string key, double value) : base(key, new (value)) { }
    /// <inheritdoc/>
    public DoubleField(string key, IParentNode parent, double value) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public class ByteField : SerializableField<ByteSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public byte Value => Serializable.Value;
    /// <inheritdoc/>
    public ByteField(string key, byte value) : base(key, new (value)) { }
    /// <inheritdoc/>
    public ByteField(string key, IParentNode parent, byte value) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public class ShortField : SerializableField<ShortSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public short Value => Serializable.Value;
    /// <inheritdoc/>
    public ShortField(string key, short value) : base(key, new (value)) { }
    /// <inheritdoc/>
    public ShortField(string key, IParentNode parent, short value) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public class IntField : SerializableField<IntSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public int Value => Serializable.Value;
    /// <inheritdoc/>
    public IntField(string key, int value) : base(key, new (value)) { }
    /// <inheritdoc/>
    public IntField(string key, IParentNode parent, int value) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public class LongField : SerializableField<LongSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public long Value => Serializable.Value;
    /// <inheritdoc/>
    public LongField(string key, long value) : base(key, new (value)) { }
    /// <inheritdoc/>
    public LongField(string key, IParentNode parent, long value) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public class SByteField : SerializableField<SByteSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public sbyte Value => Serializable.Value;
    /// <inheritdoc/>
    public SByteField(string key, sbyte value) : base(key, new (value)) { }
    /// <inheritdoc/>
    public SByteField(string key, IParentNode parent, sbyte value) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public class UShortField : SerializableField<UShortSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public ushort Value => Serializable.Value;
    /// <inheritdoc/>
    public UShortField(string key, ushort value) : base(key, new (value)) { }
    /// <inheritdoc/>
    public UShortField(string key, IParentNode parent, ushort value) : base(key, parent, new (value)) { }
}

/// <inheritdoc/>
public class UIntField : SerializableField<UIntSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public uint Value => Serializable.Value;
    /// <inheritdoc/>
    public UIntField(string key, uint value) : base(key, new (value)) { }
    /// <inheritdoc/>
    public UIntField(string key, IParentNode parent, uint value) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public class ULongField : SerializableField<ULongSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public ulong Value => Serializable.Value;
    /// <inheritdoc/>
    public ULongField(string key, ulong value) : base(key, new (value)) { }
    /// <inheritdoc/>
    public ULongField(string key, IParentNode parent, ulong value) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public class BytesField : SerializableField<BytesSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public byte[] Value => Serializable.Value;
    /// <inheritdoc/>
    public BytesField(string key, byte[] value) : base(key, new (value)) { }
    /// <inheritdoc/>
    public BytesField(string key, IParentNode parent, byte[] value) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public class StringField : SerializableField<StringSerializable> {
    /// <summary>
    /// The value of this field.
    /// </summary>
    public string Value => Serializable.Value;
    /// <inheritdoc/>
    public StringField(string key, string value) : base(key, new StringSerializable(value)) { }
    /// <inheritdoc/>
    public StringField(string key, IParentNode parent, string value) : base(key, parent, new (value)) { }
}