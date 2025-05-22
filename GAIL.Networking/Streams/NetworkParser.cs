using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using LambdaKit.Logging;

namespace GAIL.Networking.Streams;

/// <summary>
/// A parser that can parse the network format (opposite of: <see cref="NetworkSerializer"/>).
/// </summary>
public class NetworkParser : Serializing.Streams.Parser {
    /// <summary>
    /// The ID of the network parser logger.
    /// </summary>
    public const string LoggerID = "GAIL.Networking.Parser.NetworkParser";
    private static Logger? logger;
    /// <summary>
    /// The logger for the network parser.
    /// </summary>
    public static Logger Logger { get {
        if (logger == null) {
            try {
                logger = new Logger("Network Parser", LoggerID);
            } catch (ArgumentException) {
                logger = Loggers.Get(LoggerID) ?? new Logger("Network Parser");
            }
        }
        return logger;
    } }
    /// <summary>
    /// The stream to read from while formatting.
    /// </summary>
    public Stream InStream { get; private set; }
    /// <summary>
    /// Creates a new network parser.
    /// </summary>
    /// <inheritdoc/>
    public NetworkParser(Stream input, bool shouldCloseStream = false) : base(new MemoryStream(), shouldCloseStream) { InStream = input; }
    /// <summary>
    /// Creates a new network parser.
    /// </summary>
    /// <inheritdoc/>
    public NetworkParser(byte[] input, bool shouldCloseStream = false) : base(new MemoryStream(), shouldCloseStream) { InStream = new MemoryStream(input); }

    private void Decode(IFormatter? formatter = null) {
        BaseStream.Dispose();

        byte[] buffer = new byte[4];
        InStream.Read(buffer);
        IntSerializable size = (IntSerializable)IntSerializable.Info.Creator(buffer);

        buffer = new byte[size.Value];
        InStream.Read(buffer);

        if (formatter != null) buffer = formatter.Decode(buffer);

        BaseStream = new MemoryStream(buffer);
    }

    /// <summary>
    /// Reads a packet from the stream.
    /// </summary>
    /// <param name="formatter">The global formatter used for decoding.</param>
    /// <returns>The packet parsed from the stream.</returns>
    public Packet ReadPacket(IFormatter? formatter = null) {
        Decode(formatter);
        uint packetID = ReadUInt();
        
        SerializableInfo[] format = NetworkRegister.GetPacketFormat(packetID);

        Packet packet = (Packet)ReadReducer(
            new(format, (fields)=>NetworkRegister.CreatePacket(packetID, fields)),
            NetworkRegister.GetPacketFormatter(packetID)
        );
        return packet;
    }

    /// <summary>
    /// Parses the stream for packets.
    /// </summary>
    /// <param name="globalFormatter">The global formatter used for decoding.</param>
    /// <param name="isClosed">If it should stop and return.</param>
    /// <param name="onPacket">The callback for when a packet has been received. Returns true if it should stop.</param>
    /// <returns>True if it was successful, otherwise false.</returns>
    public bool Parse(IFormatter? globalFormatter, Func<bool> isClosed, Func<Packet, bool> onPacket) {
        Logger.LogDebug("Starting to parse the stream for packets...");
        while (!isClosed()) {
            try {
                if (onPacket(ReadPacket(globalFormatter))) {
                    return true;
                }
            } catch (EndOfStreamException e) {
                if (isClosed()) {
                    return true;
                }
                Logger.LogError("End of stream while parsing:");
                Logger.LogException(e, Severity.Error);
                return false;
            } catch (Exception e) {
                if (isClosed()) {
                    return true;
                }
                Logger.LogError("Exception while parsing:");
                Logger.LogException(e, Severity.Error);
                return false;
            }

        }
        Logger.LogDebug("Finished parsing the stream for packets.");
        return true;
    }
}