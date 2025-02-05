using GAIL.Networking;
using GAIL.Networking.Parser;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;

namespace examples.Packets.Shared;

public static class Packets {
    public static void RegisterPackets() {
        // Registers packets.
        NetworkRegister.RegisterPacket<MessagePacket>();
        NetworkRegister.RegisterPacket<NameMessagePacket>();
        NetworkRegister.RegisterPacket<RegisterPacket>();
    }
}

public class MessagePacket : Packet {
    [PacketField(StringSerializable.Info)] 
    public StringSerializable messageField { get; set; }

    // Assign default values (for empty constructor).
    public string message = "";

    // Empty constructor for registering.
    public MessagePacket() { }
    public MessagePacket(string message) {
        this.message = message;
    }
}
public class NameMessagePacket : Packet {
    // Applies a AES formatter to this specific packet.
    public override IFormatter Formatter => new AESFormatter([.. new byte[32].Select((_, index) => Convert.ToByte(index))], [.. new byte[16].Select((_, index) => Convert.ToByte(index))]);

    public string message = "";
    public string name = "";
    public NameMessagePacket() { }
    public NameMessagePacket(string name, string message) {
        this.name = name;
        this.message = message;
    }
}
public class RegisterPacket : Packet {

    public string name = "";

    public RegisterPacket() { }
    public RegisterPacket(string name) {
        this.name = name;
    }
}