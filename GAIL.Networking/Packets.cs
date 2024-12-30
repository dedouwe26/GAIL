using GAIL.Networking.Parser;
using GAIL.Serializing;
using OxDED.Terminal.Logging;

namespace GAIL.Networking;

/// <summary>
/// A DisconnectPacket used byt <see cref="Client.ClientContainer"/> and <see cref="Server.ServerContainer"/>
/// </summary>
[Packet]
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

/// <summary>
/// A packet that sends a log.
/// </summary>
// [Packet] : Should only be registered if it is used in the connections.
public class LogPacket : Packet {
    /// <inheritdoc/>
    public override SerializableInfo[] Format => [ByteSerializable.Info, LongSerializable.Info, StringSerializable.Info, StringSerializable.Info, StringSerializable.Info];
    
    /// <summary>
    /// The severity of the log.
    /// </summary>
    public Severity Severity { get; private set; }
    /// <summary>
    /// The time the log was created.
    /// </summary>
    public DateTime Time { get; private set; }
    /// <summary>
    /// The ID of the logger.
    /// </summary>
    public string LoggerID { get; private set; }
    /// <summary>
    /// The name of the logger.
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// The content of the log.
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// Creates a new log packet.
    /// </summary>
    /// <param name="severity">The severity of the log.</param>
    /// <param name="time">The time the log was created.</param>
    /// <param name="id">The ID of the logger.</param>
    /// <param name="name">The name of the logger.</param>
    /// <param name="text">The content of the log.</param>
    public LogPacket(Severity severity, DateTime time, string id, string name, string text) {
        Severity = severity;
        Time = time;
        LoggerID = id;
        Name = name;
        Text = text;
    }

    /// <inheritdoc/>
    public override List<ISerializable> GetFields() {
        return [
            new ByteSerializable((byte)Severity),
            new LongSerializable(Time.ToBinary()),
            new StringSerializable(LoggerID),
            new StringSerializable(Name),
            new StringSerializable(Text)
        ];
    }

    /// <inheritdoc/>
    public override void Parse(List<ISerializable> fields) {
        Severity = (Severity)(fields[0] as ByteSerializable)!.Value;
        Time = DateTime.FromBinary((fields[1] as LongSerializable)!.Value);
        LoggerID = (fields[2] as StringSerializable)!.Value;
        Name = (fields[3] as StringSerializable)!.Value;
        Text = (fields[4] as StringSerializable)!.Value;
    }
}