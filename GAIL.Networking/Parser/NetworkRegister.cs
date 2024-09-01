using System.Reflection;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;

namespace GAIL.Networking.Parser;

/// <summary>
/// A register that registers and creates packets for the network parser and serializer.
/// </summary>
public static class NetworkRegister {
    static NetworkRegister() {
        // All built-in packets.
        RegisterPacket(new DisconnectPacket());
    }
    private record struct PacketData(ConstructorInfo Constructor, SerializableInfo[] Format, IFormatter Formatter);
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
        PacketData packetData = new (GetConstructor(p), p.Format, p.Formatter);
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
    
    /// <summary>
    /// Gets the formatter of the packet.
    /// </summary>
    /// <param name="packetID">The ID of the packet.</param>
    /// <returns>The formatter of the packet.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IFormatter GetPacketFormatter(uint packetID) {
        if (!Packets.TryGetValue(packetID, out PacketData data)) {
            throw new InvalidOperationException($"Invalid packet ID: {packetID}, is it registered?");
        }
        return data.Formatter;
    }

    #endregion Packets
}