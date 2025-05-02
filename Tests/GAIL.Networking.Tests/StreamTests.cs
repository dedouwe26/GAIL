using GAIL.Networking.Streams;
using GAIL.Serializing.Formatters;

namespace GAIL.Networking.Tests;

public class StreamTests {
    [Fact]
    public void SerializingAndParsing() {
        Packets.RegisterPackets();

        TestPacket original = new();

        NetworkSerializer serializer = new(true);
        serializer.WritePacket(original);

        serializer.OutStream.Position = 0;

        NetworkParser parser = new(serializer.OutStream, false);
        Packet read = parser.ReadPacket();

        parser.Dispose();
        serializer.Dispose();

        Assert.True(original.Equals(read));
    }
    [Fact]
    public void SerializingAndParsingWithGlobalFormatter() {
        Packets.RegisterPackets();

        AESFormatter formatter = new(
            [.. new byte[32].Select((_, index) => Convert.ToByte(index))], [.. new byte[16].Select((_, index) => Convert.ToByte(index))]
        );
        TestPacket original = new();

        NetworkSerializer serializer = new(true);
        serializer.WritePacket(original, formatter);

        serializer.OutStream.Position = 0;

        NetworkParser parser = new(serializer.OutStream, false);
        Packet read = parser.ReadPacket(formatter);

        parser.Dispose();
        serializer.Dispose();

        Assert.True(original.Equals(read));
    }
}