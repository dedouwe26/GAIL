using System.Reflection;

namespace GAIL.Networking.Parser;

public record FormatData(byte[] Data, bool IsObject);



public static class PacketParser {
    private record struct FieldCreator(ConstructorInfo fromT, ConstructorInfo fromData);
    public static readonly byte Seperator = 0x1C;
    public static readonly byte Allocator = 0x1D;
    public static readonly byte Scope = 0x1E;
    public static readonly byte NewPacket = 0x1F;
    public static readonly byte[] IllegalCharacters = [Seperator, Allocator, Scope, NewPacket];
    private static readonly Dictionary<Type, FieldCreator> Fields = [];
    private static readonly Dictionary<uint, ConstructorInfo> Packets = [];
    private static ConstructorInfo GetConstructor(Packet p) {
        return p.GetType().GetConstructor([typeof(byte[])])!;
    }
    public static uint RegisterPacket(Packet p) {
        ConstructorInfo constructor = GetConstructor(p);
        if (Packets.ContainsValue(constructor)) {
            throw new InvalidOperationException("Packet is already registered");
        }
        uint id = (uint)Packets.Count;
        Packets.Add(id, constructor);
        return id;
    }
    public static uint GetPacketID(byte[] bytes) {
        return BitConverter.IsLittleEndian ? BitConverter.ToUInt32(bytes) : BitConverter.ToUInt32(bytes.Reverse().ToArray());
    }
    public static uint GetPacketID(Packet packet) {
        foreach (KeyValuePair<uint, ConstructorInfo> packetData in Packets) {
            if (packetData.Value == GetConstructor(packet)) {
                return packetData.Key;
            }
        }
        throw new ArgumentException("No ID was found, is it registered?", nameof(packet));
    }
    public static byte[] GetBytesFromPacketID(uint packetID) {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(packetID) : BitConverter.GetBytes(packetID).Reverse().ToArray();
    }
    public static Packet CreatePacket(uint packetID, byte[] data) {
        if (Packets.Count <= packetID) {
            throw new InvalidOperationException("Invalid packet ID: "+packetID);
        }
        return (Packets[packetID].Invoke([data]) as Packet)!;
    }
    public static bool ContainsFieldType(Type type) {
        return Fields.ContainsKey(type);
    }
    public static bool ContainsFieldType<T>() {
        return ContainsFieldType(typeof(T));
    }
    public static Field<object> CreateFieldFromType(Type type, object data) {
        if (!Fields.TryGetValue(type, out FieldCreator ctor)) {
            throw new ArgumentException("No field found for type: "+type.Name, nameof(type));
        }
        object obj = ctor.fromT.Invoke([data]);
        if (obj is not Field<object>) {
            throw new InvalidOperationException("Constructor of field is not for field");
        }
        return (obj as Field<object>)!;
    }
    /// <summary>
    /// Creates a field from 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Field<T> CreateFieldFromType<T>(object data) where T : notnull {
        return (CreateFieldFromType(typeof(T), data) as Field<T>)!;
    }
    /// <summary>
    /// Creates a field from an type.
    /// </summary>
    /// <param name="type">The value type of the field.</param>
    /// <param name="data">The raw data for the field.</param>
    /// <returns>The created field.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static Field<object> CreateFieldFromType(Type type, byte[] data) {
        if (!Fields.TryGetValue(type, out FieldCreator ctor)) {
            throw new ArgumentException("No field found for type: "+type.Name, nameof(type));
        }
        object obj = ctor.fromData.Invoke([data]);
        if (obj is not Field<object>) {
            throw new InvalidOperationException("Constructor of field is not for field");
        }
        return (obj as Field<object>)!;
    }
    /// <summary>
    /// Creates a field from an type.
    /// </summary>
    /// <typeparam name="T">The value type of the field.</typeparam>
    /// <param name="data">The raw data for the field.</param>
    /// <returns>The created field.</returns>
    public static Field<T> CreateFieldFromType<T>(byte[] data) where T : notnull {
        return (CreateFieldFromType(typeof(T), data) as Field<T>)!;
    }
    /// <summary>
    /// Registers a field.
    /// </summary>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <typeparam name="TType">The type of the field type.</typeparam>
    /// <param name="field">An instance of the Field, only used for getting a constructor</param>
    /// <returns></returns>
    public static bool RegisterField<TField, TType>(TField field) where TField : Field<TType> where TType : notnull {
        if (field == null) {return false;}
        if (Fields.ContainsKey(typeof(TType))) {return false;}
        Fields.Add(typeof(TType), new (field.GetType().GetConstructor([typeof(TType)])!, field.GetType().GetConstructor([typeof(byte[])])!));
        return true;
    }
    public static List<Field<object>> Parse(byte[] data, List<Type> format) {
        List<Field<object>> fields = [];
        
        byte previousByte = 0;
        bool isInObject = false;
        for (int i = 0; i < data.Length; i++) {
            byte b = data[i];

            
            // TODO: read data

            previousByte = b;
        }

        return fields;
    }
    public static byte[] Format(List<Field<object>> fields) {
        List<byte> rawData = [];

        for (int i = 0; i < fields.Count; i++) {
            
        }

        return [.. rawData];
    }
    public static byte[] Encode(Field<object> field) {
        (byte[] data, bool isObject) = field.Format();
        List<byte> newData = [];
        if (isObject) newData.Add(Scope);
        foreach (byte b in data) {
            newData.Add(b);
            if ((IllegalCharacters.Contains(b)&&!isObject)||b==NewPacket) newData.Add(b);
        }
        if (isObject) newData.Add(Scope);
        return [.. newData];
    }
    public static byte[] Encode<T>(Field<T> field) where T : notnull {
        return Encode(field);
    }
    public static Field<object> Decode(byte[] data, Type type) {
        byte? previousByte = null;
        List<byte> newData = [];
        bool isObject = false;
        foreach (byte b in data) {
            if (previousByte == null && b == Scope && data[^1] == Scope) {
                isObject = true;
                continue;
            } 
            if (IllegalCharacters.Contains(b)) {
                if (previousByte == b||isObject) {
                    newData.Add(b);
                } else {
                    continue;
                }
            } else {
                newData.Add(b);
            }
            previousByte = b;
        }
        return CreateFieldFromType(type, [.. newData]);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Field<T> Decode<T>(byte[] data) where T : notnull {
        return (Decode(data, typeof(T)) as Field<T>)!;
    }

}