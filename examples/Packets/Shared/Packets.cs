using GAIL.Networking;
using GAIL.Networking.Parser;

namespace examples.Packets.Shared;

public class MessagePacket : Packet {
    private static readonly List<Type> format = [typeof(string)];
    public string message;
    static MessagePacket() {
        PacketParser.RegisterPacket(new MessagePacket());
    }
    public MessagePacket() {
        message = "";
    }
    public MessagePacket(string message) {
        this.message = message;
    }
    public override byte[] Format() {
        return PacketParser.Format([new StringField(message)]);
        // return PacketParser.Format([PacketParser.CreateFieldFromType<string>(message)]);
    }
    public override void Parse(byte[] data) {
        message = (string)PacketParser.Parse(data, format)[0].Value;
    }
}