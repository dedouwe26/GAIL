using GAIL.Networking.Parser;

namespace GAIL.Networking;

/// <summary>
/// A DisconnectPacket used byt <see cref="Client.ClientContainer"/> and <see cref="Server.ServerContainer"/>
/// </summary>
public class DisconnectPacket : Packet {
    /// <inheritdoc/>
    public override Field[] Format { get => [new BytesField()]; }

    /// <summary>
    /// The optional additional data.
    /// </summary>
    public byte[] AdditionalData = [];

    /// <summary>
    /// Creates an empty disconnect packet.
    /// </summary>
    public DisconnectPacket() { }
    /// <summary>
    /// Creates a disconnect packet with additional data.
    /// </summary>
    /// <param name="additionalData">The additional data to send.</param>
    public DisconnectPacket(byte[] additionalData) { AdditionalData = additionalData; }
    /// <inheritdoc/>
    public DisconnectPacket(List<Field> fields) : base(fields)  { }

    /// <inheritdoc/>
    public override List<Field> GetFields() {
        return [new BytesField(AdditionalData)];
    }
    /// <inheritdoc/>
    public override void Parse(List<Field> fields) {
        AdditionalData = (byte[])fields[0].BaseValue;
    }
}