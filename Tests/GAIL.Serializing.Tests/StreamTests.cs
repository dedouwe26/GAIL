using GAIL.Serializing.Streams;

namespace GAIL.Serializing.Tests;

public class StreamTests {
    [Fact]
    public void SerializingAndParsing() {
        Serializer serializer = new();

        string a1 = "A string!";
        serializer.WriteString(a1);

        uint a2 = 0xBEEF;
        serializer.WriteUInt(a2);

        bool a3 = true;
        bool a4 = false;
        serializer.WriteSerializable(new BoolSerializable(a3, a4));

        serializer.BaseStream.Position = 0;

        Parser parser = new(serializer.BaseStream, false);

        string b1 = parser.ReadString();
        uint b2 = parser.ReadUInt();
        BoolSerializable b3 = parser.ReadSerializable<BoolSerializable>(BoolSerializable.Info);

        Assert.Equal(a1, b1);
        Assert.Equal(a2, b2);
        Assert.Equal(a3, b3.B1);
        Assert.Equal(a4, b3.B2);

        parser.Dispose();
        serializer.Dispose();
    }
}