using System.Reflection;
using GAIL.Serializing;
using GAIL.Serializing.Streams;

namespace GAIL.Networking.Parser;

/// <summary>
/// Can convert data, for the GAIL Networking.
/// </summary>
public static class NetworkParser {
    static NetworkParser() {
        // All built-in packets.
        RegisterPacket(new DisconnectPacket());
    }
    private record struct PacketData(ConstructorInfo Constructor, SerializableInfo[] Format);
    private static readonly Dictionary<uint, PacketData> Packets = [];


    #region Constructors
    private static ConstructorInfo GetConstructor(Packet p) {
        return p.GetType().GetConstructor([typeof(List<ISerializable>)]) ?? throw new InvalidOperationException($"Packet ({p.GetType().Name}) does not contain a List<ISerializable> constructor. Add this: 'public {p.GetType().Name}(List<ISerializable> fields) : base(fields)  {'{'} {'}'}'");
    }
    #endregion Constructors


    #region Packets
    /// <summary>
    /// Registers a packet.
    /// </summary>
    /// <param name="p">The packet instance, only used for getting the constructors.</param>
    /// <returns>The packet ID.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static uint RegisterPacket(Packet p) {
        PacketData packetData = new (GetConstructor(p), p.Format);
        foreach (KeyValuePair<uint, PacketData> packet in Packets) {
            if (packet.Value.Equals(packetData)) {
                throw new InvalidOperationException("Packet is already registered");
            }
        }
        uint id = (uint)Packets.Count;
        Packets.Add(id, packetData);
        return id;
    }
    /// <summary>
    /// Gets the ID of the packet.
    /// </summary>
    /// <param name="packet">The packet to get the ID of.</param>
    /// <returns>The ID of the first packet that matches.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static uint GetPacketID(Packet packet) {
        foreach (KeyValuePair<uint, PacketData> packetData in Packets) {
            if (packetData.Value.Constructor.Equals(GetConstructor(packet))) {
                return packetData.Key;
            }
        }
        throw new ArgumentException("No ID was found, is it registered?", nameof(packet));
    }
    /// <summary>
    /// Creates a packet from the ID and the raw data.
    /// </summary>
    /// <param name="packetID">The ID of the packet to create.</param>
    /// <param name="fields">All the fields of this packet.</param>
    /// <returns>The parsed packet.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Packet CreatePacket(uint packetID, List<ISerializable> fields) {
        if (Packets.Count <= packetID) {
            throw new InvalidOperationException($"Invalid packet ID: {packetID}, is it registered?");
        }
        PacketData packetData = Packets[packetID];
        return (packetData.Constructor.Invoke([fields]) as Packet)!;
    }

    /// <summary>
    /// Gets the format of the packet.
    /// </summary>
    /// <param name="packetID">The ID of the packet.</param>
    /// <returns>The format of the packet.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static SerializableInfo[] GetPacketFormat(uint packetID) {
        if (!Packets.TryGetValue(packetID, out PacketData data)) {
            throw new InvalidOperationException($"Invalid packet ID: {packetID}, is it registered?");
        }
        return data.Format;
    }

    #endregion Packets

    
    #region Parser

    /// <summary>
    /// Serializes a packet into raw data.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="packet">The packet to format.</param>
    public static void Serialize(Stream stream, Packet packet) {
        Serializer serializer = new(stream);
        serializer.WriteUInt(GetPacketID(packet));

        foreach (ISerializable field in packet.GetFields()) {
            serializer.WriteSerializable(field);
        }
    }

    /// <summary>
    /// Parses the stream for packets.
    /// </summary>
    /// <param name="stream">The stream to read the raw data from to parse.</param>
    /// <param name="isClosed">If it should stop and return.</param>
    /// <param name="onPacket">The callback for when a packet has been received. Returns true if it should stop.</param>
    /// <returns>True if it was successfull, otherwise false.</returns>
    public static bool Parse(Stream stream, Func<bool> isClosed, Func<Packet, bool> onPacket) {
        Serializing.Streams.Parser parser;
        try {
            parser = new(stream);
        } catch (InvalidOperationException) {
            return false;
        }

        bool isInPacket = false;
        uint packetID = 0;
        List<ISerializable> fields = [];
        SerializableInfo[] format = [];
        int formatIndex = 0;

        while (!isClosed()) {
            if (!isInPacket) {
                try {
                    packetID = parser.ReadUInt();
                } catch (IndexOutOfRangeException) {
                    continue;
                }
                
                format = GetPacketFormat(packetID);
                if (format.Length <= 0) {
                    if (onPacket(CreatePacket(packetID, fields))) { break; }
                    packetID = 0;
                    format = [];
                    fields = [];
                    formatIndex = 0;
                }
                isInPacket = true;
                continue;
            }

            try {
                fields.Add(parser.ReadSerializable(format[formatIndex]));
            } catch (IndexOutOfRangeException) {
                continue;
            }

            if (format.Length <= (++formatIndex)) {
                if (onPacket(CreatePacket(packetID, fields))) { break; }
                packetID = 0;
                isInPacket = false;
                format = [];
                fields = [];
                formatIndex = 0;
            }
        }
        return true;
    }
    #endregion Parser
}