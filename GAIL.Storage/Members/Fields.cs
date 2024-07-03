using GAIL.Serializing;

namespace GAIL.Storage.Members;

/// <summary>
/// A field with an underlying serializable.
/// </summary>
public abstract class SerializableField<T> : Field where T : ISerializable {
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

// TODO: Add better docs.

/// <inheritdoc/>
public sealed class BoolField : SerializableField<BoolSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new BoolField(key, default));
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
    public override MemberType Type => MemberType.Bool;

    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="b1">The first boolean (msb).</param>
    /// <param name="b2">The second boolean.</param>
    /// <param name="b3">The third boolean.</param>
    /// <param name="b4">The fourth boolean.</param>
    /// <param name="b5">The fifth boolean.</param>
    /// <param name="b6">The sixth boolean.</param>
    /// <param name="b7">The seventh boolean.</param>
    /// <param name="b8">The first boolean (lsb).</param>
    public BoolField(string key, bool b1, bool b2=false, bool b3=false, bool b4=false, bool b5=false, bool b6=false, bool b7=false, bool b8=false) : base(key, new (b1, b2, b3, b4, b5, b6, b7, b8)) { }
}

/// <inheritdoc/>
public sealed class FloatField : SerializableField<FloatSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new FloatField(key, default));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public float Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.Float;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public FloatField(string key, float value) : base(key, new (value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public FloatField(string key, float value, IParentNode parent) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public sealed class DoubleField : SerializableField<DoubleSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new DoubleField(key, default));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public double Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.Double;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public DoubleField(string key, double value) : base(key, new (value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public DoubleField(string key, double value, IParentNode parent) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public sealed class ByteField : SerializableField<ByteSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new ByteField(key, default));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public byte Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.Byte;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public ByteField(string key, byte value) : base(key, new (value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public ByteField(string key, byte value, IParentNode parent) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public sealed class ShortField : SerializableField<ShortSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new ShortField(key, default));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public short Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.Short;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public ShortField(string key, short value) : base(key, new (value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public ShortField(string key, short value, IParentNode parent) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public sealed class IntField : SerializableField<IntSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new IntField(key, default));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public int Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.Int;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public IntField(string key, int value) : base(key, new (value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public IntField(string key, int value, IParentNode parent) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public sealed class LongField : SerializableField<LongSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new LongField(key, default));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public long Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.Long;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public LongField(string key, long value) : base(key, new (value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public LongField(string key, long value, IParentNode parent) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public sealed class SByteField : SerializableField<SByteSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new SByteField(key, default));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public sbyte Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.SByte;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public SByteField(string key, sbyte value) : base(key, new (value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public SByteField(string key, sbyte value, IParentNode parent) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public sealed class UShortField : SerializableField<UShortSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new UShortField(key, default));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public ushort Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.UShort;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public UShortField(string key, ushort value) : base(key, new (value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public UShortField(string key, ushort value, IParentNode parent) : base(key, parent, new (value)) { }
}

/// <inheritdoc/>
public sealed class UIntField : SerializableField<UIntSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new UIntField(key, default));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public uint Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.UInt;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public UIntField(string key, uint value) : base(key, new (value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public UIntField(string key, uint value, IParentNode parent) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public sealed class ULongField : SerializableField<ULongSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new ULongField(key, default));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public ulong Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.ULong;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public ULongField(string key, ulong value) : base(key, new (value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public ULongField(string key, ulong value, IParentNode parent) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public sealed class BytesField : SerializableField<BytesSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new BytesField(key, []));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public byte[] Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.Bytes;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public BytesField(string key, byte[] value) : base(key, new (value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public BytesField(string key, byte[] value, IParentNode parent) : base(key, parent, new (value)) { }
}
/// <inheritdoc/>
public sealed class StringField : SerializableField<StringSerializable> {
    /// <summary>
    /// The member info of this member.
    /// </summary>
    public static readonly FieldInfo Info = CreateFieldInfo(key => new StringField(key, string.Empty));
    /// <summary>
    /// The value of this field.
    /// </summary>
    public string Value => Serializable.Value;
    /// <inheritdoc/>
    public override MemberType Type => MemberType.String;
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="value">The value of this field.</param>
    public StringField(string key, string value) : base(key, new StringSerializable(value)) { }
    /// <summary>
    /// Creates a new field.
    /// </summary>
    /// <param name="key">The key of this field.</param>
    /// <param name="parent">The parent of this field.</param>
    /// <param name="value">The value of this field.</param>
    public StringField(string key, string value, IParentNode parent) : base(key, parent, new (value)) { }
}