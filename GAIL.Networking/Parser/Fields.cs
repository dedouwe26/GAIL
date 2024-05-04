using System.Text;

namespace GAIL.Networking.Parser;

/// <summary>
/// A float field for Packets, please use <see cref="ByteField"/> if there are multiple booleans in the packet.
/// </summary>
public class BoolField : Field<bool> {
    static BoolField() {
        PacketParser.RegisterField(new BoolField());
    }
    /// <inheritdoc/>
    public BoolField() {}
    /// <inheritdoc/>
    public BoolField(bool value) : base(value) { }
    /// <inheritdoc/>
    public BoolField(byte[] data) : base(data) { }

    /// <inheritdoc/>
    public override uint? FixedSize => 1;

    /// <inheritdoc/>
    public override byte[] Format() {
        return [Value ? (byte)1 : (byte)0];
    }

    /// <inheritdoc/>
    public override bool Parse(byte[] data) {
        if (data[0]!=0||data[0]!=1) {
            throw new FormatException("byte isn't 0 or 1.");
        }
        return data[0]==1;
    }
}
/// <summary>
/// A float field for Packets.
/// </summary>
public class FloatField : Field<float> {
    static FloatField() {
        PacketParser.RegisterField(new FloatField());
    }
    /// <inheritdoc/>
    public FloatField() {}
    /// <inheritdoc/>
    public FloatField(float value) : base(value) { }
    /// <inheritdoc/>
    public FloatField(byte[] data) : base(data) { }

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
    static DoubleField() {
        PacketParser.RegisterField(new DoubleField());
    }
    /// <inheritdoc/>
    public DoubleField() {}
    /// <inheritdoc/>
    public DoubleField(double value) : base(value) { }
    /// <inheritdoc/>
    public DoubleField(byte[] data) : base(data) { }
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
    static ByteField() {
        PacketParser.RegisterField(new ByteField());
    }
    /// <inheritdoc/>
    public ByteField() {}
    /// <inheritdoc/>
    public ByteField(byte value) : base(value) { }
    /// <inheritdoc/>
    public ByteField(byte[] data) : base(data) { }
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
    static ShortField() {
        PacketParser.RegisterField(new ShortField());
    }
    /// <inheritdoc/>
    public ShortField() {}
    /// <inheritdoc/>
    public ShortField(short value) : base(value) { }
    /// <inheritdoc/>
    public ShortField(byte[] data) : base(data) { }
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
    static IntField() {
        PacketParser.RegisterField(new IntField());
    }
    /// <inheritdoc/>
    public IntField() {}
    /// <inheritdoc/>
    public IntField(int value) : base(value) { }
    /// <inheritdoc/>
    public IntField(byte[] data) : base(data) { }
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
    static LongField() {
        PacketParser.RegisterField(new LongField());
    }
    /// <inheritdoc/>
    public LongField() {}
    /// <inheritdoc/>
    public LongField(long value) : base(value) { }
    /// <inheritdoc/>
    public LongField(byte[] data) : base(data) { }
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
    static SByteField() {
        PacketParser.RegisterField(new SByteField());
    }
    /// <inheritdoc/>
    public SByteField() {}
    /// <inheritdoc/>
    public SByteField(sbyte value) : base(value) { }
    /// <inheritdoc/>
    public SByteField(byte[] data) : base(data) { }
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
    static UShortField() {
        PacketParser.RegisterField(new UShortField());
    }
    /// <inheritdoc/>
    public UShortField() {}
    /// <inheritdoc/>
    public UShortField(ushort value) : base(value) { }
    /// <inheritdoc/>
    public UShortField(byte[] data) : base(data) { }
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
    static UIntField() {
        PacketParser.RegisterField(new UIntField());
    }
    /// <inheritdoc/>
    public UIntField() {}
    /// <inheritdoc/>
    public UIntField(uint value) : base(value) { }
    /// <inheritdoc/>
    public UIntField(byte[] data) : base(data) { }
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
    static ULongField() {
        PacketParser.RegisterField(new ULongField());
    }
    /// <inheritdoc/>
    public ULongField() {}
    /// <inheritdoc/>
    public ULongField(ulong value) : base(value) { }
    /// <inheritdoc/>
    public ULongField(byte[] data) : base(data) { }
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
/// A string field for Packets.
/// </summary>
public class StringField : Field<string> {
    static StringField() {
        PacketParser.RegisterField(new StringField());
    }
    /// <inheritdoc/>
    public StringField() {}
    /// <inheritdoc/>
    public StringField(string value) : base(value) { }
    /// <inheritdoc/>
    public StringField(byte[] data) : base(data) { }
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

/// <summary>
/// A list field for Packets.
/// </summary>
public class ListField : Field<List<Field<object>>> {
    static ListField() {
        PacketParser.RegisterField(new ListField());
    }
    /// <inheritdoc/>
    public ListField() {}
    /// <inheritdoc/>
    public ListField(List<Field<object>> value) : base(value) { }
    /// <inheritdoc/>
    public ListField(byte[] data) : base(data) { }

    /// <inheritdoc/>
    public override uint? FixedSize => null;

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException"/>
    public override byte[] Format() {
        return PacketParser.Format(Value);
    }

    /// <inheritdoc/>
    public override List<Field<object>> Parse(byte[] data) {
        Type type = GetType().GetGenericArguments()[0].GetGenericArguments()[0];
        List<Field<object>> list = [];
        
        int dataIndex = 0;
        foreach (byte b in data) {
            uint? fixedSize = PacketParser.GetFixedSize(type);

            uint size;
            if (fixedSize == null) {
                size = BitConverter.ToUInt32(data.Skip(dataIndex).Take(4).ToArray());
                dataIndex += 4;
            } else {
                size = fixedSize.Value;
            }

            list.Add(PacketParser.Decode(data.Skip(dataIndex).Take(checked((int)size)).ToArray(), type));
        }

        return list;
    }
}