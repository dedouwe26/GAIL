using System.Reflection;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;
using LambdaKit.Logging;

namespace GAIL.Networking;

/// <summary>
/// A register that registers and creates packets for the network parser and serializer.
/// </summary>
public static class NetworkRegister {
    /// <summary>
    /// The ID of the network register logger.
    /// </summary>
    public const string LoggerID = "GAIL.Networking.NetworkRegister";
    private static Logger? logger;
    /// <summary>
    /// The logger for the network register.
    /// </summary>
    public static Logger Logger { get {
        if (logger == null) {
            try {
                logger = new Logger("Network Register", LoggerID);
            } catch (ArgumentException) {
                logger = Loggers.Get(LoggerID) ?? new Logger("Network Register");
            }
        }
        return logger;
    } }
    static NetworkRegister() {
        // All built-in packets.
        RegisterPacket<DisconnectPacket>();
    }
    private static readonly List<Packet.Info> Packets = [];

    #region Packet Registering

    /// <summary>
    /// Registers a packet where the non-public parts will be checked.
    /// </summary>
    /// <typeparam name="T">The type of the packet.</typeparam>
    /// <returns>The packet ID.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static uint RegisterPacket<T>() where T : Packet, new() {
        return RegisterPacket(new T());
    }
    /// <summary>
    /// Registers a packet where the non-public parts will be checked.
    /// </summary>
    /// <param name="packet">The packet to register.</param>
    /// <returns>The packet ID.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static uint RegisterPacket(Packet packet) {
        Packets.Add(packet.PacketInfo);
        return (uint)(Packets.Count - 1);
    }

    /// <summary>
    /// Checks if a packet is registered.
    /// </summary>
    /// <param name="packetType">The type of the packet.</param>
    /// <returns>True if the packet is registered.</returns>
    public static bool IsPacketRegistered(Type packetType) {
        return Packets.Any((packet) => packet.fullyQualifiedName == packetType.AssemblyQualifiedName);
    }
    /// <summary>
    /// Checks if a packet is registered.
    /// </summary>
    /// <typeparam name="T">The type of the packet.</typeparam>
    /// <returns>True if the packet is registered.</returns>
    public static bool IsPacketRegistered<T>() where T : Packet {
        return IsPacketRegistered(typeof(T));
    }
    
    #endregion Packet Registering

    #region Packet Accessing

    /// <summary>
    /// Gets the ID of the packet.
    /// </summary>
    /// <param name="packet">The packet to get the ID of.</param>
    /// <returns>The ID of the first packet that matches.</returns>
    public static uint? GetPacketID(Packet packet) {
        for (int i = 0; i < Packets.Count; i++) {
            Packet.Info packetInfo = Packets[i];
            if (packetInfo.fullyQualifiedName == packet.GetType().AssemblyQualifiedName!) {
                return (uint)i;
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the packet info of the desired packet ID.
    /// </summary>
    /// <param name="packetID">The packet's id.</param>
    /// <returns>The desired packet info.</returns>
    public static Packet.Info GetPacketInfo(uint packetID) {
        if (Packets.Count <= packetID) {
            Logger.LogError($"Invalid packet ID: {packetID}, is it registered?");
            throw new ArgumentOutOfRangeException(nameof(packetID), $"Invalid packet ID: {packetID}, is it registered?");
        }
        return Packets[(int)packetID];
    }

    /// <summary>
    /// Gets the format of the packet.
    /// </summary>
    /// <param name="packetID">The ID of the packet.</param>
    /// <returns>The format of the packet.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IRawSerializable.Info[] GetPacketFormat(uint packetID) {
        return GetPacketInfo(packetID).Format;
    }
    
    /// <summary>
    /// Gets the formatter of the packet.
    /// </summary>
    /// <param name="packetID">The ID of the packet.</param>
    /// <returns>The formatter of the packet.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IFormatter? GetPacketFormatter(uint packetID) {
        return GetPacketInfo(packetID).formatter;
    }

    #endregion Packet Accessing

    /// <summary>
    /// Creates a packet.
    /// </summary>
    /// <param name="packetID">The id of the packet to create.</param>
    /// <param name="parser">The parser of the packet data</param>
    /// <returns>The created packet.</returns>
    public static Packet CreatePacket(uint packetID, Parser parser) {
        return GetPacketInfo(packetID).Create(parser);
    }
}