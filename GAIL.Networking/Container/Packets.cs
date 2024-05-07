using GAIL.Networking.Parser;

namespace GAIL.Networking;

/// <summary>
/// A DisconnectPacket used byt <see cref="Client.ClientContainer"/> and <see cref="Server.ServerContainer"/>
/// </summary>
public class DisconnectPacket : Packet {
    static DisconnectPacket() {
        PacketParser.RegisterField(new BytesField());
        PacketParser.RegisterPacket(new DisconnectPacket());
    }

    /// <inheritdoc/>
    public override Type[] Format { get => [typeof(byte[])]; }

    /// <summary>
    /// The optional additional data.
    /// </summary>
    public byte[] AdditionalData;

    /// <inheritdoc/>
    public DisconnectPacket() {
        AdditionalData = [];
    }
    /// <inheritdoc/>
    public DisconnectPacket(byte[] additionalData) {
        AdditionalData = additionalData;
    }
    /// <inheritdoc/>
    public DisconnectPacket(List<Field> fields) : base(fields)  { AdditionalData = []; }

    /// <inheritdoc/>
    public override List<Field> GetFields() {
        return [new BytesField(AdditionalData)];
    }
    /// <inheritdoc/>
    public override void Parse(List<Field> fields) {
        AdditionalData = (byte[])fields[0].BaseValue;
    }
}