using GAIL.Serializing;
using GAIL.Serializing.Formatters;

namespace GAIL.Networking.Tests;

public class TestPacket : Packet, IEquatable<TestPacket> {
    [PacketConstructor]
    public TestPacket() { name = "Admin"; id = 6; isAdmin = true; }

    public string name;
    [PacketField]
    private StringSerializable NameField { get => new(name); set => name = value.Value; }
    public int id;
    [PacketField]
    private IntSerializable IDField { get => new(id); set => id = value.Value; }
    public bool isAdmin;
    [PacketField]
    private BoolSerializable IsAdminField { get => new(isAdmin); set => isAdmin = value.B1; }

    /// <inheritdoc/>
    public bool Equals(TestPacket? other) {
        if (other is null) { return false; }
        if (ReferenceEquals(this, other)) { return true; }
        if (GetType() != other.GetType()) { return false; }
        
        return name==other.name && id==other.id && isAdmin==other.isAdmin;
    }
    /// <inheritdoc/>
	public override bool Equals(object? obj) {
		return Equals(obj as TestPacket);
	}
    public override int GetHashCode() {
        return name.GetHashCode() ^ id.GetHashCode() ^ isAdmin.GetHashCode();
    }
}

public class FormattedPacket : Packet {
    public override IFormatter? Formatter => new GZipFormatter();
    [PacketConstructor]
    public FormattedPacket() { message = "AAaaBBbb1234"; }
    
    public string message;
    [PacketField]
    private StringSerializable MessageField { get => new(message); set => message = value.Value; }
}

public static class Packets {
    public static void RegisterPackets() {
        if (!NetworkRegister.IsPacketRegistered<TestPacket>()) {
            NetworkRegister.RegisterPacket<TestPacket>();
            NetworkRegister.RegisterPacket<FormattedPacket>();
        }
    }
}