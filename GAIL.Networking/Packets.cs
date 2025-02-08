using GAIL.Networking.Parser;
using GAIL.Serializing;
using OxDED.Terminal.Logging;

namespace GAIL.Networking;

/// <summary>
/// A disconnect packet used by the <see cref="Client.ClientContainer"/> and <see cref="Server.ServerContainer"/> to notify the other of a disconnection.
/// </summary>
public class DisconnectPacket : Packet {
    /// <summary>
    /// The optional additional data.
    /// </summary>
    public byte[] AdditionalData = [];
    [PacketField]
    private BytesSerializable AdditionalDataField { get => new(AdditionalData); set => AdditionalData = value.Value;}

    /// <summary>
    /// Creates an empty disconnect packet.
    /// </summary>
    [PacketConstructor]
    public DisconnectPacket() { }
    /// <summary>
    /// Creates a disconnect packet with additional data.
    /// </summary>
    /// <param name="additionalData">The additional data to send.</param>
    public DisconnectPacket(byte[] additionalData) { AdditionalData = additionalData; }
}

/// <summary>
/// A packet that sends a log.
/// </summary>
public class LogPacket : Packet {
    /// <summary>
    /// The severity of the log.
    /// </summary>
    public Severity Severity { get; private set; }
    [PacketField]
    private ByteSerializable SeverityField { get => new((byte)Severity); set => Severity = (Severity)value.Value; }
    /// <summary>
    /// The time the log was created.
    /// </summary>
    public DateTime Time { get; private set; }
    [PacketField]
    private LongSerializable TimeField { get => new(Time.ToBinary()); set => Time = DateTime.FromBinary(value.Value); }
    /// <summary>
    /// The ID of the logger.
    /// </summary>
    public string LoggerID { get; private set; }
    [PacketField]
    private StringSerializable IDField { get => new(LoggerID); set => LoggerID = value.Value; }
    /// <summary>
    /// The name of the logger.
    /// </summary>
    public string Name { get; private set; }
    [PacketField]
    private StringSerializable NameField { get => new(Name); set => Name = value.Value; }
    /// <summary>
    /// The content of the log.
    /// </summary>
    public string Text { get; private set; }
    [PacketField]
    private StringSerializable TextField { get => new(Text); set => Text = value.Value; }

    /// <summary>
    /// Creates an empty log packet.
    /// </summary>
    [PacketConstructor]
    public LogPacket() {
        Severity = default;
        Time = default;
        LoggerID = "";
        Name = "";
        Text = "";
    }
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
}