using GAIL.Serializing;
using GAIL.Serializing.Formatters;

namespace GAIL.Networking.Tests;

public class RegisterTests {
    [Fact]
    public void CheckForIDs() {
        uint original = NetworkRegister.RegisterPacket<TestPacket>();
        uint? id = NetworkRegister.GetPacketID(new TestPacket());
        
        Assert.NotNull(id);
        Assert.Equal(id, original);
    }
    [Fact]
    public void IsPacketRegistered() {
        NetworkRegister.RegisterPacket<TestPacket>();

        Assert.True(NetworkRegister.IsPacketRegistered<TestPacket>());
    }
    [Fact]
    public void CheckForFormat() {
        uint id = NetworkRegister.RegisterPacket<TestPacket>();

        IRawSerializable.Info[] format = NetworkRegister.GetPacketFormat(id);

        Assert.True(
            format.SequenceEqual(
                [StringSerializable.Info, IntSerializable.Info, BoolSerializable.Info],
                EqualityComparer<IRawSerializable.Info>.Create((info1, info2) => info1!.FixedSize == info2!.FixedSize)
            )
        );
    }
    [Fact]
    public void CheckForFormatter() {
        uint id = NetworkRegister.RegisterPacket<FormattedPacket>();

        IFormatter? formatter = NetworkRegister.GetPacketFormatter(id);

        Assert.IsType<GZipFormatter>(formatter);
    }
}