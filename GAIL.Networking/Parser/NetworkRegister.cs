using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using OxDED.Terminal.Logging;

namespace GAIL.Networking.Parser;

/// <summary>
/// A register that registers and creates packets for the network parser and serializer.
/// </summary>
public static class NetworkRegister {
    /// <summary>
    /// The ID of the network register logger.
    /// </summary>
    public const string LoggerID = "GAIL.Networking.Parser.NetworkRegister";
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
        RegisterPackets(typeof(DisconnectPacket).Assembly);
    }
    private record struct PacketData(ConstructorInfo Constructor, SerializableInfo[] Format, IFormatter Formatter);
    private static readonly Dictionary<uint, PacketData> Packets = [];

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Removed types don't need to get registered.")]
    private static IEnumerable<Type> GetTypesWithAttribute<T>(Assembly assembly) where T : Attribute {
        foreach (Type type in assembly.DefinedTypes) {
            if (type.GetCustomAttributes(typeof(Attribute), true).Length > 0) {
                yield return type;
            }
        }
    }
    private static ConstructorInfo? GetConstructor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type) {
        foreach (ConstructorInfo constructor in type.GetConstructors()) {
            if (constructor.GetParameters().Length < 1 && constructor.GetCustomAttributes<PacketConstructorAttribute>() != null) {
                return constructor;
            }
        }
        return null;
    }

    private static SerializableInfo[]? GetFormat(Type type) {
        foreach (PropertyInfo property in type.GetProperties()) {
            if (property.GetCustomAttributes<PacketFieldAttribute>() != null) {
                
            }
        }
    }

    #region Packets
    
    public static void RegisterPackets(Assembly assembly) {
        foreach (Type packetType in GetTypesWithAttribute<PacketAttribute>(assembly)) {
            RegisterPacket(packetType);
        }
    }
    /// <summary>
    /// Registers a packet.
    /// </summary>
    /// <param name="type">The type of the packet.</param>
    /// <returns>The packet ID.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    [SuppressMessage("Trimming", "IL2067:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.", Justification = "Try-catch is used")]
    public static uint RegisterPacket(Type type) {
        ConstructorInfo? constructor;
        try {
            constructor = GetConstructor(type);
        } catch (Exception e) {
            constructor = null;
            Logger.LogError($"Failed to get constructor of packet ({type.Name}): " + e);
            throw new ArgumentException("Failed to get constructor of packet", e);
        }
        if (constructor == null) {
            Logger.LogError($"Failed to get constructor of packet ({type.Name})");
            throw new ArgumentException("Failed to get constructor of packet");
        }

        PacketData packetData = new(constructor, );

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