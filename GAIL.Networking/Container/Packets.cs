using GAIL.Networking.Parser;
using GAIL.Serializing;

namespace GAIL.Networking;

/// <summary>
/// A DisconnectPacket used byt <see cref="Client.ClientContainer"/> and <see cref="Server.ServerContainer"/>
/// </summary>
public class DisconnectPacket : Packet {
    /// <inheritdoc/>
    public override SerializableInfo[] Format { get => [BytesSerializable.Info]; }

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
    public DisconnectPacket(List<ISerializable> fields) : base(fields)  { }

    /// <inheritdoc/>
    public override List<ISerializable> GetFields() {
        return [new BytesSerializable(AdditionalData)];
    }
    /// <inheritdoc/>
    public override void Parse(List<ISerializable> fields) {
        AdditionalData = (fields[0] as BytesSerializable)!.Value;
    }
}