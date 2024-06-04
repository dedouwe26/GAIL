using GAIL.Networking;
using GAIL.Networking.Parser;

namespace examples.Packets.Shared;

public static class Packets {
    public static void RegisterPackets() {
        // Registers packets.
        PacketParser.RegisterPacket(new MessagePacket());
        PacketParser.RegisterPacket(new NameMessagePacket());
        PacketParser.RegisterPacket(new RegisterPacket());
    }
}

public class MessagePacket : Packet {
    // The format of this packet (baseplate fields).
    public override Field[] Format => [new StringField()];
    // Assign default values (for empty constructor).
    public string message = "";

    // Empty constructor for registering.
    public MessagePacket() { }
    public MessagePacket(string message) {
        this.message = message;
    }
    // You can call Parse method directly.
    public MessagePacket(List<Field> fields)  { Parse(fields); }

    public override List<Field> GetFields() { // Create the fields (in the format above).
        return [new StringField(message)];
    }

    public override void Parse(List<Field> fields) { // Assign the values back.
        message = (string)fields[0].BaseValue;
    }
}
public class NameMessagePacket : Packet {
    public override Field[] Format => [new StringField(), new StringField()];

    public string message = "";
    public string name = "";
    static NameMessagePacket() {
        PacketParser.RegisterField(new StringField());
    }
    public NameMessagePacket() { }
    public NameMessagePacket(string name, string message) {
        this.name = name;
        this.message = message;
    }
    // You can call the base constructor.
    public NameMessagePacket(List<Field> fields) : base(fields)  { }

    public override List<Field> GetFields() {
        return [new StringField(name), new StringField(message)];
    }

    public override void Parse(List<Field> fields) {
        name = (string)fields[0].BaseValue;
        message = (string)fields[1].BaseValue;
    }
}
public class RegisterPacket : Packet {
    public override Field[] Format => [new StringField()];

    public string name = "";
    static RegisterPacket() {
        PacketParser.RegisterField(new StringField());
    }
    public RegisterPacket() { }
    public RegisterPacket(string name) {
        this.name = name;
    }
    public RegisterPacket(List<Field> fields) { Parse(fields); }
    public override List<Field> GetFields() {
        return [new StringField(name)];
    }

    public override void Parse(List<Field> fields) {
        name = (fields[0] as StringField)!.Value;
    }
}