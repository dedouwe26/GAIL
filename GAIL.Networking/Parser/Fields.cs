using System.Text;

namespace GAIL.Networking.Parser;

public class BoolField : Field<bool> {
    static BoolField() {
        PacketParser.RegisterFieldType(new BoolField());
    }
    public BoolField() {}
    public BoolField(bool value) : base(value) { }
    public BoolField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new ([Value ? (byte)1 : (byte)0], false);
    }

    public override bool Parse(byte[] data) {
        if (data[0]!=0||data[0]!=1) {
            throw new FormatException("byte isn't 0 or 1.");
        }
        return data[0]==1;
    }
}
public class FloatField : Field<float> {
    static FloatField() {
        PacketParser.RegisterFieldType(new FloatField());
    }
    public FloatField() {}
    public FloatField(float value) : base(value) { }
    public FloatField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray(), false);
    }
    public override float Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToSingle(data) : BitConverter.ToSingle(data.Reverse().ToArray());
    }
}
public class DoubleField : Field<double> {
    static DoubleField() {
        PacketParser.RegisterFieldType(new DoubleField());
    }
    public DoubleField() {}
    public DoubleField(double value) : base(value) { }
    public DoubleField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray(), false);
    }

    public override double Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToDouble(data) : BitConverter.ToDouble(data.Reverse().ToArray());
    }
}
public class ByteField : Field<byte> {
    static ByteField() {
        PacketParser.RegisterFieldType(new ByteField());
    }
    public ByteField() {}
    public ByteField(byte value) : base(value) { }
    public ByteField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new ([Value], false);
    }

    public override byte Parse(byte[] data) {
        return data[0];
    }
}
public class ShortField : Field<short> {
    static ShortField() {
        PacketParser.RegisterFieldType(new ShortField());
    }
    public ShortField() {}
    public ShortField(short value) : base(value) { }
    public ShortField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray(), false);
    }
    public override short Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToInt16(data) : BitConverter.ToInt16(data.Reverse().ToArray());
    }
}
public class IntField : Field<int> {
    static IntField() {
        PacketParser.RegisterFieldType(new IntField());
    }
    public IntField() {}
    public IntField(int value) : base(value) { }
    public IntField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray(), false);
    }
    public override int Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToInt32(data) : BitConverter.ToInt32(data.Reverse().ToArray());
    }
}
public class LongField : Field<long> {
    static LongField() {
        PacketParser.RegisterFieldType(new LongField());
    }
    public LongField() {}
    public LongField(long value) : base(value) { }
    public LongField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray(), false);
    }
    public override long Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToInt64(data) : BitConverter.ToInt64(data.Reverse().ToArray());
    }
}
public class SByteField : Field<sbyte> {
    static SByteField() {
        PacketParser.RegisterFieldType(new SByteField());
    }
    public SByteField() {}
    public SByteField(sbyte value) : base(value) { }
    public SByteField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new ([(byte)Value], false);
    }
    public override sbyte Parse(byte[] data) {
        return (sbyte)data[0];
    }
}
public class UShortField : Field<ushort> {
    static UShortField() {
        PacketParser.RegisterFieldType(new UShortField());
    }
    public UShortField() {}
    public UShortField(ushort value) : base(value) { }
    public UShortField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray(), false);
    }
    public override ushort Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToUInt16(data) : BitConverter.ToUInt16(data.Reverse().ToArray());
    }
}
public class UIntField : Field<uint> {
    static UIntField() {
        PacketParser.RegisterFieldType(new UIntField());
    }
    public UIntField() {}
    public UIntField(uint value) : base(value) { }
    public UIntField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray(), false);
    }
    public override uint Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToUInt32(data) : BitConverter.ToUInt32(data.Reverse().ToArray());
    }
}
public class ULongField : Field<ulong> {
    static ULongField() {
        PacketParser.RegisterFieldType(new ULongField());
    }
    public ULongField() {}
    public ULongField(ulong value) : base(value) { }
    public ULongField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(Value) : BitConverter.GetBytes(Value).Reverse().ToArray(), false);
    }
    public override ulong Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToUInt64(data) : BitConverter.ToUInt64(data.Reverse().ToArray());
    }
}

public class StringField : Field<string> {
    static StringField() {
        PacketParser.RegisterFieldType(new StringField());
    }
    public StringField() {}
    public StringField(string value) : base(value) { }
    public StringField(byte[] data) : base(data) { }
    public override FormatData Format() {
        return new (BitConverter.IsLittleEndian ? Encoding.UTF8.GetBytes(Value) : Encoding.UTF8.GetBytes(Value).Reverse().ToArray(), false);
    }
    public override string Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? Encoding.UTF8.GetString(data) : Encoding.UTF8.GetString(data.Reverse().ToArray());
    }
}

public class ListField : Field<List<Field<object>>> {
    static ListField() {
        PacketParser.RegisterFieldType(new ListField());
    }
    public ListField() {}
    public ListField(List<Field<object>> value) : base(value) { }
    public ListField(byte[] data) : base(data) { }

    public override FormatData Format() {
        Type listType = Value.GetType().GetGenericArguments()[0].GetType();
        if (!PacketParser.ContainsFieldType(listType)) {
            throw new InvalidOperationException("Could not find parser for type: "+listType.Name);
        }
        return new ([.. PacketParser.Format(Value)], true);
    }

    public override List<Field<object>> Parse(byte[] data) {
        
    }
}