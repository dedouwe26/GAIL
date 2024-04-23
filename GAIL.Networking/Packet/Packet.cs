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
    public Packet(byte[] data) { Parse(data); }
    /// <summary>
    /// Formats a packet and turns it into raw data.
    /// </summary>
    /// <returns>The raw data that it is turned into.</returns>
    public abstract byte[] Format();
    /// <summary>
    /// Sets all the data in this packet from the raw data.
    /// </summary>
    /// <param name="data">The raw data to convert.</param>
    public abstract void Parse(byte[] data);
    /// <summary>
    /// Gets the ID of this Packet.
    /// </summary>
    /// <returns>The ID of this Packet.</returns>
    public uint GetID() { return PacketParser.GetPacketID(this); }
}