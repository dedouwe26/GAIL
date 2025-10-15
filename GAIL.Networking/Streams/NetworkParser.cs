using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using LambdaKit.Logging;

namespace GAIL.Networking.Streams;

/// <summary>
/// A parser that can parse the network format (opposite of: <see cref="NetworkSerializer"/>).
/// </summary>
public class NetworkParser : Parser {
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
    /// Creates a new network parser.
    /// </summary>
    /// <inheritdoc/>
    public NetworkParser(Stream input, bool shouldCloseStream = false) : base(input, shouldCloseStream) { }
    /// <summary>
    /// Creates a new network parser.
    /// </summary>
    /// <inheritdoc/>
    public NetworkParser(byte[] input, bool shouldCloseStream = true) : base(input, shouldCloseStream) { }

    /// <summary>
    /// Reads a packet from the stream.
    /// </summary>
    /// <param name="formatter">The global formatter used for decoding.</param>
    /// <returns>The packet parsed from the stream.</returns>
    public Packet ReadPacket(IFormatter? formatter = null) {
        if (formatter != null) {
            Packet packet;
            using (Parser parser = new(Read(null, formatter))) {
                uint packetID = parser.ReadUInt();
                packet = NetworkRegister.GetPacketInfo(packetID).Create(parser, null);
            }
            return packet;
        } else {
            uint packetID = ReadUInt();
            return NetworkRegister.GetPacketInfo(packetID).Create(this, null);
        }
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