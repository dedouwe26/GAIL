using GAIL.Serializing;
using GAIL.Serializing.Formatters;

namespace GAIL.Networking.Parser;

/// <summary>
/// A parser that can parse the network format (opposite of: <see cref="NetworkSerializer"/>).
/// </summary>
public class NetworkParser : Serializing.Streams.Parser {
    /// <summary>
    /// Creates a new network parser.
    /// </summary>
    /// <inheritdoc/>
    public NetworkParser(Stream input, bool shouldCloseStream = false) : base(input, shouldCloseStream) { }
    /// <summary>
    /// Creates a new network parser.
    /// </summary>
    /// <inheritdoc/>
    public NetworkParser(byte[] input, bool shouldCloseStream = false) : base(input, shouldCloseStream) { }

    private List<ISerializable> ReadFields(SerializableInfo[] format) {
        List<ISerializable> fields = [];
        int formatIndex = 0;

        while (true) {
            try {
                fields.Add(ReadSerializable(format[formatIndex]));
            } catch (EndOfStreamException) {
                return fields;
            }
            
            if (format.Length <= (++formatIndex)) {
                return fields;
            }
        }
    }

    /// <summary>
    /// Applies all the formatters to make it parsable (should call at the beginning).
    /// </summary>
    /// <param name="globalFormatter">The formatter used for global purposes (multiple packets).</param>
    /// <param name="packetFormatter">The formatter used for this specific packet.</param>
    public void Decode(IFormatter globalFormatter, IFormatter packetFormatter) {
        byte[] raw = new byte[4];
        InStream.Read(raw);
        IntSerializable @int = new(default);
        @int.Parse(raw);
        raw = new byte[@int.Value];
        InStream.Read(raw);
        BaseStream = new MemoryStream(formatter.Decode(raw));
    }

    /// <summary>
    /// Reads a packet from the stream.
    /// </summary>
    /// <returns>The packet parsed from the stream.</returns>
    public Packet ReadPacket() {
        uint packetID = ReadUInt();
        SerializableInfo[] format = NetworkRegister.GetPacketFormat(packetID);
        if (format.Length <= 0) {
            return NetworkRegister.CreatePacket(packetID, []);
        }

        return NetworkRegister.CreatePacket(packetID, ReadFields(format));
    }

    /// <summary>
    /// Parses the stream for packets.
    /// </summary>
    /// <param name="isClosed">If it should stop and return.</param>
    /// <param name="onPacket">The callback for when a packet has been received. Returns true if it should stop.</param>
    /// <returns>True if it was successful, otherwise false.</returns>
    public bool Parse(Func<bool> isClosed, Func<Packet, bool> onPacket) {
        while (!isClosed()) {
            try {
                onPacket(ReadPacket());
            } catch (EndOfStreamException) {
                return false;
            }
            
        }

        return true;
    }
}