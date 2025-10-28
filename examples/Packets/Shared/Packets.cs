using GAIL.Networking;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using LambdaKit.Logging;

namespace examples.Packets.Shared;

public static class Packets {
    private static Logger? logger;
    internal static Logger Logger { get {
        logger ??= new Logger("Shared Logger", severity: Severity.Trace);
        return logger;
    } }
    public static void RegisterPackets() {
        // Registers packets.
        NetworkRegister.RegisterPacket(MessagePacket.Info);
        NetworkRegister.RegisterPacket(NameMessagePacket.Info);
        NetworkRegister.RegisterPacket(RegisterPacket.Info);
    }
}

public class MessagePacket : Packet {
    // The following line creates a packet info for this packet.
    // This is not neccessary, but it reduces code cluttering
    // when registering your packet.
	public static new Info Info => Info.Create<MessagePacket>();
    // Assign default values (for an empty parser-ready constructor).
    public string message = "";

    // Empty constructor for parsing.
    public MessagePacket() { }
    // Constructor for serializing.
    public MessagePacket(string message) {
        this.message = message;
    }
    // In this method you read all the data you have written
    // in Serialize(Serializer) and convert that to your fields.
    public override void Parse(Parser parser) {
        message = parser.ReadString();
    }
    // In this method you write all your data to the serializer.
    public override void Serialize(Serializer serializer) {
        serializer.WriteString(message);
    }
    
}
public class NameMessagePacket : Packet {
	public static new Info Info => Info.Create<NameMessagePacket>();
    // Before sending this packet, the AES formatter is going to be applied.
    // This adds another layer of security or compression.
    // The following lines will add that functionality.
    public override IFormatter Formatter => new AESFormatter(
        // Some AES specific stuff.
        [.. new byte[32].Select((_, index) => (byte)index)], [.. new byte[16].Select((_, index) => (byte)index)]
    );

    public string message = "";
    public string name = "";

    public NameMessagePacket() { }
    public NameMessagePacket(string name, string message) {
        this.name = name;
        this.message = message;
    }

    public override void Parse(Parser parser) {
        message = parser.ReadString();
        name = parser.ReadString();
    }
    public override void Serialize(Serializer serializer) {
        serializer.WriteString(message);
        serializer.WriteString(name);
    }
}
public class RegisterPacket : Packet {
	public static new Info Info => Info.Create<RegisterPacket>();
    public string name = "";

    public RegisterPacket() { }
    public RegisterPacket(string name) {
        this.name = name;
    }
    public override void Parse(Parser parser) {
        name = parser.ReadString();
        // You can also do other stuff in the Parse and Serialize methods.
        Packets.Logger.LogMessage("RegisterPacket just got parsed (probably received by the server)");
    }
    public override void Serialize(Serializer serializer) {
        Packets.Logger.LogMessage("RegisterPacket is serializing (probably sent to the server)");
        serializer.WriteString(name);
    }
}