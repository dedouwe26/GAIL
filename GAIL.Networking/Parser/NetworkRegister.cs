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
        RegisterPacket<DisconnectPacket>();
    }
    private record class PacketFieldInfo(PropertyInfo Property, SerializableInfo Info);
    private record class PacketInfo(string FullyQualifiedName, Func<ISerializable[], Packet> Creator, SerializableInfo[] Format, IFormatter Formatter, Func<Packet, ISerializable[]> Destructor);
    private static readonly List<PacketInfo> Packets = [];

    #region Packet Reflection

    private static ConstructorInfo? GetConstructor(ConstructorInfo[] constructors) {
        foreach (ConstructorInfo constructor in constructors) {
            if (constructor.IsDefined(typeof(PacketConstructorAttribute))) {
                if (constructor.GetParameters().Length > 0) {
                    throw new ArgumentException($"Constructor {constructor.Name} in {constructor.ReflectedType?.Name ?? "packet"} cannot have parameters");
                }
                return constructor;
            }
        }
        return null;
    }
    private static PacketFieldInfo[] GetFields(PropertyInfo[] properties, object instance) {
        List<PacketFieldInfo> f = [];
        foreach (PropertyInfo property in properties) {
            PacketFieldAttribute? attribute = property.GetCustomAttribute<PacketFieldAttribute>();
            if (attribute != null) {
                if (!typeof(ISerializable).IsAssignableFrom(property.PropertyType)) {
                    throw new ArgumentException($"Property {property.Name} in {property.ReflectedType?.Name ?? "packet"} is not a serializable");
                }
                ISerializable? serializable = property.GetValue(instance) as ISerializable
                    ?? throw new ArgumentException($"Property {property.Name} in {property.ReflectedType?.Name ?? "packet"} is not a serializable");
                f.Add(new PacketFieldInfo(property, serializable.Info));
            }
        }
        return [.. f];
    }
    private static MethodInfo? GetSerializeMethod(MethodInfo[] methods) {
        foreach (MethodInfo method in methods) {
            if (method.IsDefined(typeof(PacketSerializeAttribute))) {
                if (method.GetParameters().Length > 0) {
                    throw new ArgumentException($"Method {method.Name} in {method.ReflectedType?.Name ?? "packet"} cannot have parameters");
                }
                return method;
            }
        }
        return null;
    }
    private static MethodInfo? GetParseMethod(MethodInfo[] methods) {
        foreach (MethodInfo method in methods) {
            if (method.IsDefined(typeof(PacketParseAttribute))) {
                if (method.GetParameters().Length > 0) {
                    throw new ArgumentException($"Method {method.Name} in {method.ReflectedType?.Name ?? "packet"} cannot have parameters");
                }
                return method;
            }
        }
        return null;
    }

    #endregion Packet Reflection

    #region Packets

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
        Type type = packet.GetType();
        string name = type.Name;

        PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        ConstructorInfo? constructor;
        try {
            constructor = GetConstructor(type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
        } catch (Exception e) {
            Logger.LogError($"Failed at getting the constructor of packet ({name}):");
            Logger.LogException(e);
            throw new ArgumentException($"Failed at the getting constructor of packet ({name})", e);
        }
        if (constructor == null) {
            Logger.LogError($"Could not find the constructor of packet ({name})");
            throw new ArgumentException($"Could not find the constructor of packet ({name})");
        }

        PacketFieldInfo[] fields;
        try {
            fields = GetFields(properties, packet);
        } catch (Exception e) {
            Logger.LogError($"Failed at getting the format of packet ({name}):");
            Logger.LogException(e);
            throw new ArgumentException($"Failed at getting the format of packet ({name})", e);
        }
        
        IFormatter formatter = packet.Formatter;

        MethodInfo? serializeMethod;
        MethodInfo? parseMethod;
        try {
            serializeMethod = GetSerializeMethod(methods);
            parseMethod = GetParseMethod(methods);
        } catch (Exception e) {
            Logger.LogError($"Failed at getting the serialize- and parse-methods of packet ({name}):");
            Logger.LogException(e);
            throw new ArgumentException($"Failed at getting the serialize and parse methods of packet ({name})", e);
        }

        Packet CreatePacket(ISerializable[] serializables) {
            if (constructor.Invoke(null) is not Packet packet) {
                throw new InvalidOperationException($"Failed at creating packet ({name})");
            }

            for (int i = 0; i < fields.Length; i++) {
                try {
                fields[i].Property.SetValue(packet, serializables[i]);
                } catch (Exception e) {
                    Logger.LogError($"Failed at setting field {fields[i].Property.Name} in packet ({name}):");
                    Logger.LogException(e);
                    throw new InvalidOperationException($"Failed at setting field {fields[i].Property.Name} in packet ({name})", e);
                }
            }

            parseMethod?.Invoke(packet, null);

            return packet;
        }
        ISerializable[] DestructPacket(Packet packet) {
            serializeMethod?.Invoke(packet, null);

            List<ISerializable> result = [];
            
            foreach (PacketFieldInfo field in fields) {
                object? gainedValue;
                try {
                    gainedValue = field.Property.GetValue(packet);
                } catch (Exception e) {
                    Logger.LogError($"Failed at getting field {field.Property.Name} in packet ({name}):");
                    Logger.LogException(e);
                    throw new InvalidOperationException($"Failed at getting field {field.Property.Name} in packet ({name})", e);
                }
                if (gainedValue is not ISerializable serializable) {
                    throw new InvalidOperationException($"Field {field.Property.Name} in {name} is not a serializable");
                }
                result.Add(serializable);
            }
            return [.. result];
        }

        PacketInfo packetData = new(type.AssemblyQualifiedName!, CreatePacket, [.. fields.Select(f => f.Info)], formatter, DestructPacket);

        foreach (PacketInfo p in Packets) {
            if (p.Equals(packetData)) {
                throw new InvalidOperationException("Packet is already registered");
            }
        }
        Packets.Add(packetData);
        return (uint)(Packets.Count - 1);
    }
    /// <summary>
    /// Gets the ID of the packet.
    /// </summary>
    /// <param name="packet">The packet to get the ID of.</param>
    /// <returns>The ID of the first packet that matches.</returns>
    public static uint? GetPacketID(Packet packet) {
        for (int i = 0; i < Packets.Count; i++) {
            PacketInfo packetData = Packets[i];
            if (packetData.FullyQualifiedName == packet.GetType().AssemblyQualifiedName!) {
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
    public static Packet CreatePacket(uint packetID, ISerializable[] fields) {
        if (Packets.Count <= packetID) {
            Logger.LogError($"Invalid packet ID: {packetID}, is it registered?");
            throw new ArgumentOutOfRangeException(nameof(packetID), $"Invalid packet ID: {packetID}, is it registered?");
        }
        PacketInfo packetData = Packets[(int)packetID];
        return packetData.Creator(fields);
    }

    /// <summary>
    /// Destructs a packet from its original state.
    /// </summary>
    /// <param name="packet">The packet to destruct.</param>
    /// <returns>The fields of the packet.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static ISerializable[] DestructPacket(Packet packet) {
        if (packet.ID >= Packets.Count || packet.ID < 0) {
            throw new InvalidOperationException($"Invalid packet ID: {packet.ID} of packet {packet.GetType().Name}");
        }
        PacketInfo info = Packets[(int)packet.ID];
        return info.Destructor(packet);
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
            throw new ArgumentOutOfRangeException(nameof(packetID), $"Invalid packet ID: {packetID}, is it registered?");
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
            throw new ArgumentOutOfRangeException(nameof(packetID), $"Invalid packet ID: {packetID}, is it registered?");
        }
        return Packets[(int)packetID].Formatter;
    }

    #endregion Packets
}