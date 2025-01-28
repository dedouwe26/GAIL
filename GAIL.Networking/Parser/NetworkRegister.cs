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
    private static readonly List<PacketData> Packets = [];

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Removed types don't need to get registered.")]
    private static IEnumerable<Type> GetTypesWithAttribute<T>(Assembly assembly, bool inherit = false) where T : Attribute {
        foreach (Type type in assembly.GetTypes()) {
            if (type.GetCustomAttributes(typeof(T), inherit).Length > 0) {
                yield return type;
            }
        }
    }

    #region Packet Reflection

    private static ConstructorInfo? GetConstructor(Type type) {
        foreach (ConstructorInfo constructor in type.GetConstructors(BindingFlags.Instance)) {
            if (constructor.IsDefined(typeof(PacketConstructorAttribute)) && constructor.GetParameters().Length < 1) {
                return constructor;
            }
        }
        return null;
    }
    private static ConstructorInfo? GetConstructorNonPublic([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type type) {
        foreach (ConstructorInfo constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)) {
            if (constructor.IsDefined(typeof(PacketConstructorAttribute)) && constructor.GetParameters().Length < 1) {
                return constructor;
            }
        }
        return null;
    }
    private static IFormatter? GetFormatter(Type type) {
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance)) {
            if (property.IsDefined(typeof(PacketFormatterAttribute)) && property.PropertyType == typeof(IFormatter)) {
                return property.GetValue(null) as IFormatter;
            }
        }
        return null;
    }
    private static IFormatter? GetFormatterNonPublic([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicProperties)] Type type) {
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)) {
            if (property.IsDefined(typeof(PacketFormatterAttribute)) && property.PropertyType == typeof(IFormatter)) {
                return property.GetValue(null) as IFormatter;
            }
        }
        return null;
    }
    private static SerializableInfo[] GetFormat(Type type) {
        List<SerializableInfo> format = [];
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance)) {
            PacketFieldAttribute? attribute = property.GetCustomAttribute<PacketFieldAttribute>();
            if (attribute != null) {
                if (!typeof(ISerializable).IsAssignableFrom(property.PropertyType)) {
                    throw new ArgumentException($"Property {property.Name} in {type.Name} is not an ISerializable");
                }
                format.Add(attribute.Info);
            }
        }
        return [.. format];
    }
    private static SerializableInfo[] GetFormatNonPublic([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicProperties)] Type type) {
        List<SerializableInfo> format = [];
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)) {
            PacketFieldAttribute? attribute = property.GetCustomAttribute<PacketFieldAttribute>();
            if (attribute != null) {
                if (!typeof(ISerializable).IsAssignableFrom(property.PropertyType)) {
                    throw new ArgumentException($"Property {property.Name} in {type.Name} is not an ISerializable");
                }
                format.Add(attribute.Info);
            }
        }
        return [.. format];
    }

    #endregion Packet Reflection

    #region Packets
    
    /// <summary>
    /// Registers all packets in an assembly.
    /// </summary>
    /// <param name="assembly">The assembly of the packets to register.</param>
    public static void RegisterPackets(Assembly assembly) {
        foreach (Type packetType in GetTypesWithAttribute<PacketAttribute>(assembly)) {
            RegisterPacket(packetType);
        }
    }
    /// <summary>
    /// Registers a packet where only the public parts will be checked.
    /// </summary>
    /// <param name="type">The type of the packet.</param>
    /// <returns>The packet ID.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static uint RegisterPacket(Type type) {
        ConstructorInfo? constructor;
        try {
            constructor = GetConstructor(type);
        } catch (Exception e) {
            Logger.LogError($"Failed to get constructor of packet ({type.Name}):");
            Logger.LogException(e);
            throw new ArgumentException($"Failed to get constructor of packet ({type.Name})", e);
        }
        if (constructor == null) {
            Logger.LogError($"Could not find constructor of packet ({type.Name})");
            throw new ArgumentException($"Could not find constructor of packet ({type.Name})");
        }

        SerializableInfo[] format;
        try {
            format = GetFormat(type);
        } catch (Exception e) {
            Logger.LogError($"Failed to get format of packet ({type.Name}):");
            Logger.LogException(e);
            throw new ArgumentException($"Failed to get format of packet ({type.Name})", e);
        }

        IFormatter? formatter;
        try {
            formatter = GetFormatter(type);
        } catch (Exception e) {
            Logger.LogError($"Failed to get formatter of packet ({type.Name}):");
            Logger.LogException(e);
            throw new ArgumentException($"Failed to get formatter of packet", e);
        }
        if (formatter == null) {
            Logger.LogError($"Could not find formatter of packet ({type.Name})");
            throw new ArgumentException($"Could not find formatter of packet ({type.Name})");
        }

        PacketData packetData = new(constructor, format, formatter);

        foreach (PacketData packet in Packets) {
            if (packet.Equals(packetData)) {
                throw new InvalidOperationException("Packet is already registered");
            }
        }
        Packets.Add(packetData);
        return (uint)(Packets.Count - 1);
    }
    /// <summary>
    /// Registers a packet where the non-public parts will be checked too.
    /// </summary>
    /// <param name="type">The type of the packet.</param>
    /// <returns>The packet ID.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static uint RegisterPacketNonPublic([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicProperties|DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type type) {
        ConstructorInfo? constructor;
        try {
            constructor = GetConstructorNonPublic(type);
        } catch (Exception e) {
            Logger.LogError($"Failed to get constructor of packet ({type.Name}):");
            Logger.LogException(e);
            throw new ArgumentException($"Failed to get constructor of packet ({type.Name})", e);
        }
        if (constructor == null) {
            Logger.LogError($"Could not find constructor of packet ({type.Name})");
            throw new ArgumentException($"Could not find constructor of packet ({type.Name})");
        }

        SerializableInfo[] format;
        try {
            format = GetFormatNonPublic(type);
        } catch (Exception e) {
            Logger.LogError($"Failed to get format of packet ({type.Name}):");
            Logger.LogException(e);
            throw new ArgumentException($"Failed to get format of packet ({type.Name})", e);
        }

        IFormatter? formatter;
        try {
            formatter = GetFormatterNonPublic(type);
        } catch (Exception e) {
            Logger.LogError($"Failed to get formatter of packet ({type.Name}):");
            Logger.LogException(e);
            throw new ArgumentException($"Failed to get formatter of packet", e);
        }
        if (formatter == null) {
            Logger.LogError($"Could not find formatter of packet ({type.Name})");
            throw new ArgumentException($"Could not find formatter of packet ({type.Name})");
        }

        PacketData packetData = new(constructor, format, formatter);

        foreach (PacketData packet in Packets) {
            if (packet.Equals(packetData)) {
                throw new InvalidOperationException("Packet is already registered");
            }
        }
        Packets.Add(packetData);
        return (uint)(Packets.Count - 1);
    }
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
        return RegisterPacketNonPublic(packet.GetType());
    }
    /// <summary>
    /// Gets the ID of the packet.
    /// </summary>
    /// <param name="packet">The packet to get the ID of.</param>
    /// <returns>The ID of the first packet that matches.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static uint? GetPacketID(Packet packet) {
        for (int i = 0; i < Packets.Count; i++) {
            PacketData packetData = Packets[i];
            if (packetData.Constructor.Equals(GetConstructor(packet.GetType()))) {
                return (uint)i;
            }
        }
        return null;
    }
    /// <summary>
    /// Creates a packet from the ID and the raw data.
    /// </summary>
    /// <param name="packetID">The ID of the packet to create.</param>
    /// <param name="fields">All the fields of this packet.</param>
    /// <returns>The parsed packet.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Packet CreatePacket(uint packetID, List<ISerializable> fields) {
        if (Packets.Count <= packetID) {
            Logger.LogError($"Invalid packet ID: {packetID}, is it registered?");
            throw new ArgumentOutOfRangeException($"Invalid packet ID: {packetID}, is it registered?");
        }
        PacketData packetData = Packets[(int)packetID];
        return (packetData.Constructor.Invoke([fields]) as Packet)!;
    }

    /// <summary>
    /// Gets the format of the packet.
    /// </summary>
    /// <param name="packetID">The ID of the packet.</param>
    /// <returns>The format of the packet.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static SerializableInfo[] GetPacketFormat(uint packetID) {
        if (Packets.Count <= packetID) {
            Logger.LogError($"Invalid packet ID: {packetID}, is it registered?");
            throw new ArgumentOutOfRangeException($"Invalid packet ID: {packetID}, is it registered?");
        }
        return Packets[(int)packetID].Format;
    }
    
    /// <summary>
    /// Gets the formatter of the packet.
    /// </summary>
    /// <param name="packetID">The ID of the packet.</param>
    /// <returns>The formatter of the packet.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IFormatter GetPacketFormatter(uint packetID) {
        if (Packets.Count <= packetID) {
            Logger.LogError($"Invalid packet ID: {packetID}, is it registered?");
            throw new ArgumentOutOfRangeException($"Invalid packet ID: {packetID}, is it registered?");
        }
        return Packets[(int)packetID].Formatter;
    }

    #endregion Packets
}