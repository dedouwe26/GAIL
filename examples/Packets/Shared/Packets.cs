using GAIL.Networking;
using GAIL.Networking.Parser;

namespace examples.Packets.Shared;

public class MessagePacket : Packet {
    public override Type[] Format => [typeof(string)];

    public string message;
    static MessagePacket() {
        PacketParser.RegisterField(new StringField());
        PacketParser.RegisterPacket(new MessagePacket());
    }
    public MessagePacket() {
        message = "";
    }
    public MessagePacket(string message) {
        this.message = message;
    }
    
    public MessagePacket(List<Field> fields) : base(fields)  { message = ""; }

    public override List<Field> GetFields() {
        return [new StringField(message)];
    }

    public override void Parse(List<Field> fields) {
        message = (string)fields[0].BaseValue;
    }
}
public class NameMessagePacket : Packet {
    public override Type[] Format => [typeof(string), typeof(string)];

    public string message;
    public string name;
    static NameMessagePacket() {
        PacketParser.RegisterField(new StringField());
        PacketParser.RegisterPacket(new MessagePacket());
    }
    public NameMessagePacket() {
        name = "";
        message = "";
    }
    public NameMessagePacket(string name, string message) {
        this.name = name;
        this.message = message;
    }

    public NameMessagePacket(List<Field> fields) : base(fields)  { name = ""; message = ""; }

    public override List<Field> GetFields() {
        return [new StringField(name), new StringField(message)];
    }

    public override void Parse(List<Field> fields) {
        name = (string)fields[0].BaseValue;
        message = (string)fields[1].BaseValue;
    }
}
public class RegisterPacket : Packet {
    public override Type[] Format => [typeof(string)];

    public string name;
    static RegisterPacket() {
        PacketParser.RegisterField(new StringField());
        PacketParser.RegisterPacket(new RegisterPacket());
    }
    public RegisterPacket() {
        name = "";
    }
    public RegisterPacket(string name) {
        this.name = name;
    }
    public RegisterPacket(List<Field> fields) : base(fields)  { name = ""; }
    public override List<Field> GetFields() {
        return [new StringField(name)];
    }

    public override void Parse(List<Field> fields) {
        name = (string)fields[0].BaseValue;
    }
}