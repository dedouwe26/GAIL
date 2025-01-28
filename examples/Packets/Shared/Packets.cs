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
    // The format of this packet (baseplate fields).
    public override SerializableInfo[] Format => [StringSerializable.Info];
    // Assign default values (for empty constructor).
    public string message = "";

    // Empty constructor for registering.
    public MessagePacket() { }
    public MessagePacket(string message) {
        this.message = message;
    }
    // You can call Parse method directly.
    public MessagePacket(List<ISerializable> fields)  { Parse(fields); }

    public override List<ISerializable> GetFields() { // Create the fields (in the format above).
        return [new StringSerializable(message)];
    }

    public override void Parse(List<ISerializable> fields) { // Assign the values back.
        message = (fields[0] as StringSerializable)!.Value;
    }
}
public class NameMessagePacket : Packet {
    // Applies a AES formatter to this specific packet.
    public override IFormatter Formatter => new AESFormatter(new byte[32].Select((_, index) => Convert.ToByte(index)).ToArray(), new byte[16].Select((_, index) => Convert.ToByte(index)).ToArray());

    public override SerializableInfo[] Format => [StringSerializable.Info, StringSerializable.Info];

    public string message = "";
    public string name = "";
    public NameMessagePacket() { }
    public NameMessagePacket(string name, string message) {
        this.name = name;
        this.message = message;
    }
    // You can call the base constructor.
    public NameMessagePacket(List<ISerializable> fields) : base(fields)  { }

    public override List<ISerializable> GetFields() {
        return [new StringSerializable(name), new StringSerializable(message)];
    }

    public override void Parse(List<ISerializable> fields) {
        name = (fields[0] as StringSerializable)!.Value;
        message = (fields[1] as StringSerializable)!.Value;
    }
}
public class RegisterPacket : Packet {
    public override SerializableInfo[] Format => [StringSerializable.Info];

    public string name = "";

    public RegisterPacket() { }
    public RegisterPacket(string name) {
        this.name = name;
    }
    public RegisterPacket(List<ISerializable> fields) { Parse(fields); }
    public override List<ISerializable> GetFields() {
        return [new StringSerializable(name)];
    }

    public override void Parse(List<ISerializable> fields) {
        name = (fields[0] as StringSerializable)!.Value;
    }
}