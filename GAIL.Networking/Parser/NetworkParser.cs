using GAIL.Serializing;

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