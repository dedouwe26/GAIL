using System.Text;

namespace GAIL.Networking.Parser;

/// <summary>
/// A float field for Packets, please use <see cref="ByteField"/> if there are multiple booleans in the packet.
/// </summary>
public class BoolField : Field<bool> {
    /// <inheritdoc/>
    public BoolField() {}
    /// <inheritdoc/>
    public BoolField(bool value) : base(value) { }
    /// <inheritdoc/>
    public BoolField(RawData data) : base(data) { }

    /// <inheritdoc/>
    public override uint? FixedSize => 1;

    /// <inheritdoc/>
    public override byte[] Format() {
        return [Value ? (byte)1 : (byte)0];
    }

    /// <inheritdoc/>
    public override bool Parse(byte[] data) {
        return data[0]!=0;
    }
}
/// <summary>
/// A float field for Packets.
/// </summary>
public class FloatField : Field<float> {
    /// <inheritdoc/>
    public FloatField() {}
    /// <inheritdoc/>
    public FloatField(float value) : base(value) { }
    /// <inheritdoc/>
    public FloatField(RawData data) : base(data) { }

    /// <inheritdoc/>
    public override uint? FixedSize => 4;

    /// <inheritdoc/>
    public override byte[] Format() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public override float Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToSingle(data) : BitConverter.ToSingle(data.Reverse().ToArray());
    }
}
/// <summary>
/// A list field for Packets.
/// </summary>
public class DoubleField : Field<double> {
    /// <inheritdoc/>
    public DoubleField() {}
    /// <inheritdoc/>
    public DoubleField(double value) : base(value) { }
    /// <inheritdoc/>
    public DoubleField(RawData data) : base(data) { }
    /// <inheritdoc/>
    public override uint? FixedSize => 8;
    /// <inheritdoc/>
    public override byte[] Format() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }

    /// <inheritdoc/>
    public override double Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToDouble(data) : BitConverter.ToDouble(data.Reverse().ToArray());
    }
}
/// <summary>
/// A byte field for Packets.
/// </summary>
public class ByteField : Field<byte> {
    /// <inheritdoc/>
    public ByteField() {}
    /// <inheritdoc/>
    public ByteField(byte value) : base(value) { }
    /// <inheritdoc/>
    public ByteField(RawData data) : base(data) { }
    /// <inheritdoc/>
    public override uint? FixedSize => 1;
    /// <inheritdoc/>
    public override byte[] Format() {
        return [Value];
    }

    /// <inheritdoc/>
    public override byte Parse(byte[] data) {
        return data[0];
    }
}
/// <summary>
/// A short field for Packets.
/// </summary>
public class ShortField : Field<short> {
    /// <inheritdoc/>
    public ShortField() {}
    /// <inheritdoc/>
    public ShortField(short value) : base(value) { }
    /// <inheritdoc/>
    public ShortField(RawData data) : base(data) { }
    /// <inheritdoc/>
    public override uint? FixedSize => 2;
    /// <inheritdoc/>
    public override byte[] Format() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public override short Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToInt16(data) : BitConverter.ToInt16(data.Reverse().ToArray());
    }
}
/// <summary>
/// A int field for Packets.
/// </summary>
public class IntField : Field<int> {
    /// <inheritdoc/>
    public IntField() {}
    /// <inheritdoc/>
    public IntField(int value) : base(value) { }
    /// <inheritdoc/>
    public IntField(RawData data) : base(data) { }
    /// <inheritdoc/>
    public override uint? FixedSize => 4;
    /// <inheritdoc/>
    public override byte[] Format() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public override int Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToInt32(data) : BitConverter.ToInt32(data.Reverse().ToArray());
    }
}
/// <summary>
/// A long field for Packets.
/// </summary>
public class LongField : Field<long> {
    /// <inheritdoc/>
    public LongField() {}
    /// <inheritdoc/>
    public LongField(long value) : base(value) { }
    /// <inheritdoc/>
    public LongField(RawData data) : base(data) { }
    /// <inheritdoc/>
    public override uint? FixedSize => 8;
    /// <inheritdoc/>
    public override byte[] Format() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public override long Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToInt64(data) : BitConverter.ToInt64(data.Reverse().ToArray());
    }
}
/// <summary>
/// A signed byte field for Packets.
/// </summary>
public class SByteField : Field<sbyte> {
    /// <inheritdoc/>
    public SByteField() {}
    /// <inheritdoc/>
    public SByteField(sbyte value) : base(value) { }
    /// <inheritdoc/>
    public SByteField(RawData data) : base(data) { }
    /// <inheritdoc/>
    public override uint? FixedSize => 1;
    /// <inheritdoc/>
    public override byte[] Format() {
        return [(byte)Value];
    }
    /// <inheritdoc/>
    public override sbyte Parse(byte[] data) {
        return (sbyte)data[0];
    }
}
/// <summary>
/// A unsigned short field for Packets.
/// </summary>
public class UShortField : Field<ushort> {
    /// <inheritdoc/>
    public UShortField() {}
    /// <inheritdoc/>
    public UShortField(ushort value) : base(value) { }
    /// <inheritdoc/>
    public UShortField(RawData data) : base(data) { }
    /// <inheritdoc/>
    public override uint? FixedSize => 2;
    /// <inheritdoc/>
    public override byte[] Format() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public override ushort Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToUInt16(data) : BitConverter.ToUInt16(data.Reverse().ToArray());
    }
}
/// <summary>
/// A unsigned int field for Packets.
/// </summary>
public class UIntField : Field<uint> {
    /// <inheritdoc/>
    public UIntField() {}
    /// <inheritdoc/>
    public UIntField(uint value) : base(value) { }
    /// <inheritdoc/>
    public UIntField(RawData data) : base(data) { }
    /// <inheritdoc/>
    public override uint? FixedSize => 4;
    /// <inheritdoc/>
    public override byte[] Format() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public override uint Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToUInt32(data) : BitConverter.ToUInt32(data.Reverse().ToArray());
    }
}
/// <summary>
/// A unsigned long field for Packets.
/// </summary>
public class ULongField : Field<ulong> {
    /// <inheritdoc/>
    public ULongField() {}
    /// <inheritdoc/>
    public ULongField(ulong value) : base(value) { }
    /// <inheritdoc/>
    public ULongField(RawData data) : base(data) { }
    /// <inheritdoc/>
    public override uint? FixedSize => 8;
    /// <inheritdoc/>
    public override byte[] Format() {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public override ulong Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToUInt64(data) : BitConverter.ToUInt64(data.Reverse().ToArray());
    }
}
/// <summary>
/// A byte array field for Packets.
/// </summary>
public class BytesField : Field<byte[]> {
    /// <inheritdoc/>
    public BytesField() {}
    /// <inheritdoc/>
    public BytesField(byte[] value) : base(value) { }
    /// <inheritdoc/>
    public BytesField(RawData data) : base(data) { }

    /// <inheritdoc/>
    public override uint? FixedSize => null;

    /// <inheritdoc/>
    public override byte[] Format() {
        return BitConverter.IsLittleEndian ? Value : [.. Value.Reverse()];
    }
    /// <inheritdoc/>
    public override byte[] Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? data : [.. data.Reverse()];
    }
}
/// <summary>
/// A string field for Packets.
/// </summary>
public class StringField : Field<string> {
    /// <inheritdoc/>
    public StringField() {}
    /// <inheritdoc/>
    public StringField(string value) : base(value) { }
    /// <inheritdoc/>
    public StringField(RawData data) : base(data) { }
    /// <inheritdoc/>
    public override uint? FixedSize => null;
    /// <inheritdoc/>
    public override byte[] Format() {
        return BitConverter.IsLittleEndian ? Encoding.UTF8.GetBytes(Value) : Encoding.UTF8.GetBytes(Value).Reverse().ToArray();
    }
    /// <inheritdoc/>
    public override string Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? Encoding.UTF8.GetString(data) : Encoding.UTF8.GetString(data.Reverse().ToArray());
    }
}