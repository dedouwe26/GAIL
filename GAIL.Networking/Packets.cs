using GAIL.Networking.Parser;
using GAIL.Serializing;
using OxDED.Terminal.Logging;

namespace GAIL.Networking;

/// <summary>
/// A DisconnectPacket used byt <see cref="Client.ClientContainer"/> and <see cref="Server.ServerContainer"/>
/// </summary>
public class DisconnectPacket : Packet {
    /// <inheritdoc/>
    public override SerializableInfo[] Format => [BytesSerializable.Info];

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

public class LogPacket : Packet {
    public override SerializableInfo[] Format => [ByteSerializable.Info, LongSerializable.Info, StringSerializable.Info, StringSerializable.Info, StringSerializable.Info];
    
    public Severity Severity { get; private set; }
    public DateTime Time { get; private set; }
    public string ID { get; private set; }
    public string Name { get; private set; }
    public string Text { get; private set; }

    public LogPacket(Severity severity, DateTime time, string id, string name, string text) {
        Severity = severity;
        Time = time;
        ID = id;
        Name = name;
        Text = text;
    }

    public override List<ISerializable> GetFields() {
        return [
            new ByteSerializable((byte)Severity),
            new LongSerializable(Time.ToBinary()),
            new StringSerializable(ID),
            new StringSerializable(Name),
            new StringSerializable(Text)
        ];
    }

    public override void Parse(List<ISerializable> fields) {
        Severity = (Severity)(fields[0] as ByteSerializable)!.Value;
        Time = DateTime.FromBinary((fields[1] as LongSerializable)!.Value);
        ID = (fields[2] as StringSerializable)!.Value;
        Name = (fields[3] as StringSerializable)!.Value;
        Text = (fields[4] as StringSerializable)!.Value;
    }
}