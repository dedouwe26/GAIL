using System.Text;

namespace GAIL.Networking.Parser;

public class BoolParser : IParser<bool> {
    static BoolParser() {
        PacketParser.RegisterFieldParser(new BoolParser());
    }
    public FormatData Format(bool data) {
        return new ([data ? (byte)1 : (byte)0], false);
    }

    public bool Parse(byte[] data) {
        if (data[0]!=0||data[0]!=1) {
            throw new FormatException("byte isn't 0 or 1.");
        }
        return data[0]==1;
    }
}

public class FloatParser : IParser<float> {
    static FloatParser() {
        PacketParser.RegisterFieldParser(new FloatParser());
    }
    public FormatData Format(float data) {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(data) : BitConverter.GetBytes(data).Reverse().ToArray(), false);
    }

    public float Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToSingle(data) : BitConverter.ToSingle(data.Reverse().ToArray());
    }
}
public class DoubleParser : IParser<double> {
    static DoubleParser() {
        PacketParser.RegisterFieldParser(new DoubleParser());
    }
    public FormatData Format(double data) {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(data) : BitConverter.GetBytes(data).Reverse().ToArray(), false);
    }

    public double Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToDouble(data) : BitConverter.ToDouble(data.Reverse().ToArray());
    }
}
public class ByteParser : IParser<byte> {
    static ByteParser() {
        PacketParser.RegisterFieldParser(new ByteParser());
    }
    public FormatData Format(byte data) {
        return new ([data], false);
    }

    public byte Parse(byte[] data) {
        return data[0];
    }
}
public class ShortParser : IParser<short> {
    static ShortParser() {
        PacketParser.RegisterFieldParser(new ShortParser());
    }
    public FormatData Format(short data) {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(data) : BitConverter.GetBytes(data).Reverse().ToArray(), false);
    }

    public short Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToInt16(data) : BitConverter.ToInt16(data.Reverse().ToArray());
    }
}
public class IntParser : IParser<int> {
    static IntParser() {
        PacketParser.RegisterFieldParser(new IntParser());
    }
    public FormatData Format(int data) {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(data) : BitConverter.GetBytes(data).Reverse().ToArray(), false);
    }

    public int Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToInt32(data) : BitConverter.ToInt32(data.Reverse().ToArray());
    }
}
public class LongParser : IParser<long> {
    static LongParser() {
        PacketParser.RegisterFieldParser(new LongParser());
    }
    public FormatData Format(long data) {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(data) : BitConverter.GetBytes(data).Reverse().ToArray(), false);
    }

    public long Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToInt64(data) : BitConverter.ToInt64(data.Reverse().ToArray());
    }
}
public class SByteParser : IParser<sbyte> {
    static SByteParser() {
        PacketParser.RegisterFieldParser(new SByteParser());
    }
    public FormatData Format(sbyte data) {
        return new ([(byte)data], false);
    }

    public sbyte Parse(byte[] data) {
        return (sbyte)data[0];
    }
}
public class UShortParser : IParser<ushort> {
    static UShortParser() {
        PacketParser.RegisterFieldParser(new UShortParser());
    }
    public FormatData Format(ushort data) {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(data) : BitConverter.GetBytes(data).Reverse().ToArray(), false);
    }

    public ushort Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToUInt16(data) : BitConverter.ToUInt16(data.Reverse().ToArray());
    }
}
public class UIntParser : IParser<uint> {
    static UIntParser() {
        PacketParser.RegisterFieldParser(new UIntParser());
    }
    public FormatData Format(uint data) {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(data) : BitConverter.GetBytes(data).Reverse().ToArray(), false);
    }

    public uint Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToUInt32(data) : BitConverter.ToUInt32(data.Reverse().ToArray());
    }
}
public class ULongParser : IParser<ulong> {
    static ULongParser() {
        PacketParser.RegisterFieldParser(new ULongParser());
    }
    public FormatData Format(ulong data) {
        return new (BitConverter.IsLittleEndian ? BitConverter.GetBytes(data) : BitConverter.GetBytes(data).Reverse().ToArray(), false);
    }

    public ulong Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? BitConverter.ToUInt64(data) : BitConverter.ToUInt64(data.Reverse().ToArray());
    }
}

public class StringParser : IParser<string> {
    static StringParser() {
        PacketParser.RegisterFieldParser(new StringParser());
    }
    public FormatData Format(string data) {
        return new (BitConverter.IsLittleEndian ? Encoding.UTF8.GetBytes(data) : Encoding.UTF8.GetBytes(data).Reverse().ToArray(), false);
    }

    public string Parse(byte[] data) {
        return BitConverter.IsLittleEndian ? Encoding.UTF8.GetString(data) : Encoding.UTF8.GetString(data.Reverse().ToArray());
    }
}

public class ListParser : Field<List<object>> {
    static ListParser() {
        PacketParser.RegisterFieldParser(new ListParser());
    }
    public FormatData Format(List<object> data) {
        Type listType = data.GetType().GetGenericArguments()[0].GetType();
        if (!PacketParser.ContainsFieldParser(listType)) {
            throw new ArgumentException("Could not find parser for type: "+listType.Name, nameof(data));
        }
        List<byte> raw = [];
        foreach (object obj in data) {
            PacketParser.Encode(new IField<object>(obj), );
        }
        return new ([.. raw], true);
    }

    public List<object> Parse(byte[] data) {

    }
}