using GAIL.Networking;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using OxDED.Terminal.Logging;

namespace examples.Packets.Shared;

public static class Packets {
    private static Logger? logger;
    internal static Logger Logger { get {
        logger ??= new Logger("Shared Logger", severity: Severity.Trace);
        return logger;
    } }
    public static void RegisterPackets() {
        // Registers packets.
        NetworkRegister.RegisterPacket<MessagePacket>();
        NetworkRegister.RegisterPacket<NameMessagePacket>();
        NetworkRegister.RegisterPacket<RegisterPacket>();
    }
}

public class MessagePacket : Packet {
    // Assign default values (for empty constructor).
    public string message = "";

    // This defines an entry in the serialized packet.
    [PacketField]
    // The packet field must be an ISerializable and must be gettable and settable.
    private StringSerializable MessageField { get => new(message); set => message = value.Value; } 

    // Defines which constructor to use for registering.
    [PacketConstructor]
    // Empty constructor for registering.
    public MessagePacket() { }
    public MessagePacket(string message) {
        this.message = message;
    }
}
public class NameMessagePacket : Packet {
    // Before sending this packet, the AES formatter is going to be applied.
    // Just like before parsing. This adds another layer of security or compression.
    // Applies a AES formatter to this specific packet.
    public override IFormatter Formatter => new AESFormatter(
        // Some AES specific stuff.
        [.. new byte[32].Select((_, index) => Convert.ToByte(index))], [.. new byte[16].Select((_, index) => Convert.ToByte(index))]
    );

    public string message = "";
    
    // Some more fields, nothing special.
    [PacketField]
    private StringSerializable MessageField { get => new(message); set => message = value.Value; }

    public string name = "";
    [PacketField]
    private StringSerializable NameField { get => new(name); set => name = value.Value; }

    [PacketConstructor]
    public NameMessagePacket() { }
    public NameMessagePacket(string name, string message) {
        this.name = name;
        this.message = message;
    }
}
public class RegisterPacket : Packet {
    public string name = "";
    [PacketField]
    private StringSerializable NameField { get => new(name); set => name = value.Value; }

    [PacketConstructor]
    public RegisterPacket() { }
    public RegisterPacket(string name) {
        this.name = name;
    }
    // This method is called before the packet is going to be serialized.
    protected override void OnSerialize() {
        Packets.Logger.LogMessage("RegisterPacket just got serialized (probably sent to the server)");
    }

    // This method is called after the packet has been parsed.
    protected override void OnParse() {
        Packets.Logger.LogMessage("RegisterPacket just got parsed (probably received by the server)");
    }
}