using GAIL.Networking.Parser;

namespace GAIL.Networking;

/// <summary>
/// Packet is an abstract class for parsing and formatting a packet.
/// </summary>
public abstract class Packet {
    /// <summary>
    /// Creates a packet (add own data here).
    /// </summary>
    public Packet() {}
    /// <summary>
    /// Creates a packet from the raw bytes.
    /// </summary>
    /// <param name="data">The bytes to parse.</param>
    public Packet(List<Field> data) { Parse(data); }
    /// <summary>
    /// The format of this packets (field baseplates).
    /// </summary>
    public abstract Field[] Format { get; }
    /// <summary>
    /// Gets all the fields of this packet.
    /// </summary>
    /// <returns>All the fields of this packet.</returns>
    public abstract List<Field> GetFields();
    /// <summary>
    /// Creates this packet from all the fields.
    /// </summary>
    /// <param name="fields">The fields to create the packet from.</param>
    public abstract void Parse(List<Field> fields);
    /// <summary>
    /// The ID of this packet.
    /// </summary>
    public uint ID { get => PacketParser.GetPacketID(this); }
}