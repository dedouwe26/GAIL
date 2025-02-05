using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using OxDED.Terminal.Logging;

namespace GAIL.Networking.Parser;

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

    private ISerializable[] ReadFields(SerializableInfo[] format) {
        List<ISerializable> fields = [];
        int formatIndex = 0;

        while (true) {
            try {
                fields.Add(ReadSerializable(format[formatIndex]));
            } catch (EndOfStreamException) {
                return [.. fields];
            }
            
            if (format.Length <= (++formatIndex)) {
                return [.. fields];
            }
        }
    }

    /// <summary>
    /// Applies all the formatters to make it parsable (should call at the beginning).
    /// </summary>
    /// <param name="globalFormatter">The formatter used for global purposes (multiple packets).</param>
    /// <param name="packetFormatter">The formatter getter used for this specific packet ID.</param>
    /// <returns>
    /// Returns the ID of the decoded packet.
    /// </returns>
    public uint Decode(IFormatter globalFormatter, Func<uint, IFormatter> packetFormatter) {
        BaseStream.Dispose();

        byte[] raw = new byte[4];
        InStream.Read(raw);

        IntSerializable intSerializable = new(default);
        intSerializable.Parse(raw);
        raw = new byte[intSerializable.Value];

        InStream.Read(raw);
        raw = globalFormatter.Decode(raw);

        UIntSerializable uintSerializable = new(default);
        uintSerializable.Parse(raw.Take(4).ToArray());
        raw = packetFormatter.Invoke(uintSerializable.Value).Decode(raw.Skip(4).ToArray());

        BaseStream = new MemoryStream(raw);

        return uintSerializable.Value;
    }

    /// <summary>
    /// Reads a packet from the stream.
    /// </summary>
    /// <param name="globalFormatter">The global formatter used for decoding.</param>
    /// <returns>The packet parsed from the stream.</returns>
    public Packet ReadPacket(IFormatter globalFormatter) {
        uint packetID = Decode(globalFormatter, NetworkRegister.GetPacketFormatter);
        
        SerializableInfo[] format = NetworkRegister.GetPacketFormat(packetID);
        if (format.Length <= 0) {
            return NetworkRegister.CreatePacket(packetID, []);
        }

        return NetworkRegister.CreatePacket(packetID, ReadFields(format));
    }

    /// <summary>
    /// Parses the stream for packets.
    /// </summary>
    /// <param name="globalFormatter">The global formatter used for decoding.</param>
    /// <param name="isClosed">If it should stop and return.</param>
    /// <param name="onPacket">The callback for when a packet has been received. Returns true if it should stop.</param>
    /// <returns>True if it was successful, otherwise false.</returns>
    public bool Parse(IFormatter globalFormatter, Func<bool> isClosed, Func<Packet, bool> onPacket) {
        Logger.LogDebug("Starting to parse the stream for packets...");
        while (!isClosed()) {
            try {
                onPacket(ReadPacket(globalFormatter));
            } catch (EndOfStreamException e) {
                Logger.LogError("End of stream while parsing: "+e);
                return false;
            }

        }
        Logger.LogDebug("Finished parsing the stream for packets.");
        return true;
    }
}