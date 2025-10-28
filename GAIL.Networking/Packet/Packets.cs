using GAIL.Serializing;
using GAIL.Serializing.Streams;
using LambdaKit.Logging;

namespace GAIL.Networking;

/// <summary>
/// A disconnect packet used by the <see cref="Client.ClientContainer"/> and <see cref="Server.ServerContainer"/> to notify the other of a disconnection.
/// </summary>
public class DisconnectPacket : Packet {
	/// <summary>
	/// Gets the packet info of this packet.
	/// </summary>
	public static new Info Info => Info.Create<DisconnectPacket>();
	/// <summary>
	/// The optional additional data.
	/// </summary>
	public byte[] additionalData;
	/// <summary>
	/// Creates an empty parser-ready disconnect packet.
	/// </summary>
	public DisconnectPacket() { additionalData = []; }
	/// <summary>
	/// Creates a disconnect packet with additional data.
	/// </summary>
	/// <param name="additionalData">The additional data to send.</param>
	public DisconnectPacket(byte[] additionalData) { this.additionalData = additionalData; }

	/// <inheritdoc/>
	public override void Parse(Parser parser) {
		additionalData = parser.ReadValue<byte[]>(BytesSerializable.Info);
	}
	/// <inheritdoc/>
	public override void Serialize(Serializer serializer) {
		serializer.WriteSerializable(new BytesSerializable(additionalData));
	}
}

/// <summary>
/// A packet that sends a log.
/// </summary>
public class LogPacket : Packet {
	/// <summary>
	/// Gets the packet info of this packet.
	/// </summary>
	public static new Info Info => Info.Create<LogPacket>();
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
	/// Creates an empty parser-ready log packet.
	/// </summary>
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

	/// <inheritdoc/>
	public override void Parse(Parser parser) {
		Severity = (Severity)parser.ReadByte();
		Time = DateTime.FromBinary(parser.ReadValue<long>(LongSerializable.Info));
		LoggerID = parser.ReadString();
		Name = parser.ReadString();
		Text = parser.ReadString();
	}
	/// <inheritdoc/>
	public override void Serialize(Serializer serializer) {
		serializer.WriteByte((byte)Severity);
		serializer.WriteSerializable(new LongSerializable(Time.ToBinary()));
		serializer.WriteString(LoggerID);
		serializer.WriteString(Name);
		serializer.WriteString(Text);
	}
}