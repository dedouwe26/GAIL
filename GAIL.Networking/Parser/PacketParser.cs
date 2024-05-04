using System.Reflection;

namespace GAIL.Networking.Parser;

/// <summary>
/// Can convert data, for the GAIL Networking.
/// </summary>
public static class PacketParser {
    private record struct FieldData(ConstructorInfo FromT, ConstructorInfo FromData, uint? FixedSize);
    private static readonly Dictionary<Type, FieldData> Fields = [];
    private static readonly Dictionary<uint, ConstructorInfo> Packets = [];


    #region Constructors
    private static ConstructorInfo GetConstructor(Packet p) {
        return p.GetType().GetConstructor([typeof(byte[])])!;
    }
    private static ConstructorInfo? GetTypeConstructor<T>(Field<T> field) where T : notnull {
        return field.GetType().GetConstructor([typeof(T)]);
    }
    private static ConstructorInfo? GetRawConstructor<T>(Field<T> field) where T : notnull {
        return field.GetType().GetConstructor([typeof(byte[])]);
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
        ConstructorInfo constructor = GetConstructor(p);
        if (Packets.ContainsValue(constructor)) {
            throw new InvalidOperationException("Packet is already registered");
        }
        uint id = (uint)Packets.Count;
        Packets.Add(id, constructor);
        return id;
    }
    /// <summary>
    /// Gets the packet ID from the byte list.
    /// </summary>
    /// <param name="bytes">The bytes of the ID.</param>
    /// <returns>The ID of the packet.</returns>
    public static uint GetPacketID(byte[] bytes) {
        return BitConverter.IsLittleEndian ? BitConverter.ToUInt32(bytes) : BitConverter.ToUInt32(bytes.Reverse().ToArray());
    }
    /// <summary>
    /// Gets the ID of the packet.
    /// </summary>
    /// <param name="packet">The packet to get the ID of.</param>
    /// <returns>The ID of the packet.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static uint GetPacketID(Packet packet) {
        foreach (KeyValuePair<uint, ConstructorInfo> packetData in Packets) {
            if (packetData.Value == GetConstructor(packet)) {
                return packetData.Key;
            }
        }
        throw new ArgumentException("No ID was found, is it registered?", nameof(packet));
    }
    /// <summary>
    /// Gets the bytes from an ID.
    /// </summary>
    /// <param name="packetID">The ID of the packet.</param>
    /// <returns>The bytes of the packet ID.</returns>
    public static byte[] GetBytesFromPacketID(uint packetID) {
        return BitConverter.IsLittleEndian ? BitConverter.GetBytes(packetID) : BitConverter.GetBytes(packetID).Reverse().ToArray();
    }
    /// <summary>
    /// Creates a packet from the ID and the raw data.
    /// </summary>
    /// <param name="packetID">The ID of the packet to create.</param>
    /// <param name="data">The raw data of the packet.</param>
    /// <returns>The parsed packet.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Packet CreatePacket(uint packetID, byte[] data) {
        if (Packets.Count <= packetID) {
            throw new InvalidOperationException("Invalid packet ID: "+packetID);
        }
        return (Packets[packetID].Invoke([data]) as Packet)!;
    }
    #endregion Packets


    #region Fields

    #region   Create
    /// <summary>
    /// Creates a field from the data.
    /// </summary>
    /// <param name="type">The type of the data.</param>
    /// <param name="data">The data.</param>
    /// <returns>The created field.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static Field<object> CreateFieldFromType(Type type, object data) {
        if (!Fields.TryGetValue(type, out FieldData ctor)) {
            throw new ArgumentException("No field found for type: "+type.Name, nameof(type));
        }
        object obj = ctor.FromT.Invoke([data]);
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
        if (!Fields.TryGetValue(type, out FieldData ctor)) {
            throw new ArgumentException("No field found for type: "+type.Name, nameof(type));
        }
        object obj = ctor.FromData.Invoke([data]);
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
    #endregion   Create

    /// <summary>
    /// Registers a field.
    /// </summary>
    /// <typeparam name="T">The type of the field type.</typeparam>
    /// <param name="field">An instance of the Field, only used for getting a constructor and the fixed size.</param>
    /// <returns>True if it was a success.</returns>
    /// <exception cref="InvalidOperationException"/>
    public static bool RegisterField<T>(Field<T> field) where T : notnull {
        if (field == null) {return false;}
        if (Fields.ContainsKey(typeof(T))) {return false;}
        Fields.Add(typeof(T), new (
            GetTypeConstructor(field) ?? throw new InvalidOperationException("The field has no constructor for creating a field from "+typeof(T).Name), 
            GetRawConstructor(field) ?? throw new InvalidOperationException("The field has no constructor for creating a field from byte[] (raw data)"),
            field.FixedSize
        ));
        return true;
    }

    /// <summary>
    /// Gets the fixed size of a field if it has one.
    /// </summary>
    /// <param name="type">The type of the field type.</param>
    /// <returns>The fixed size.</returns>
    /// <exception cref="InvalidOperationException" />
    public static uint? GetFixedSize(Type type) {
        if (!Fields.TryGetValue(type, out FieldData fieldData)) {
            throw new InvalidOperationException("Could not find a field with type: "+type.Name);
        }
        return fieldData.FixedSize;
    }
    /// <summary>
    /// Checks if there is a field for type <paramref name="type"/>
    /// </summary>
    /// <param name="type">The type of the field type.</param>
    /// <returns>True if there is one.</returns>
    public static bool ContainsFieldType(Type type) {
        return Fields.ContainsKey(type);
    }
    /// <summary>
    /// Checks if there is a field for type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of the field type.</typeparam>
    /// <returns>True if there is one.</returns>
    public static bool ContainsFieldType<T>() where T : notnull {
        return ContainsFieldType(typeof(T));
    }

    #endregion Fields

    
    #region Parser
    
    #region Obsolete
    #if false
    /// <summary>
    /// Reads the length of the next field (see: <see cref="WriteLength"/>).
    /// </summary>
    /// <param name="currentByte">The current byte in an array.</param>
    /// <param name="nextByte">The function to get the next byte.</param>
    /// <returns>The length of the next field.</returns>
    public static uint ReadLength(byte currentByte, Func<byte> nextByte) {
        byte data = (byte)(0b_1111_1110 & currentByte);
        if ((0b_0000_0001 & currentByte) == 1) {
            return (uint)(data >>> 1);
        } else {
            return ((uint)((data >>> 1) << 7)) | ReadLength(nextByte(), nextByte);
        }
    }
    #endif
    #endregion Obsolete
    
    /// <summary>
    /// Parses the raw data into fields.
    /// </summary>
    /// <param name="data">The raw data to parse.</param>
    /// <param name="format">The format of the fields.</param>
    /// <returns>All the parsed fields.</returns>
    public static List<Field<object>> Parse(byte[] data, List<Type> format) {
        List<Field<object>> fields = [];
        
        int dataIndex = 0;
        foreach (Type type in format) {
            uint? fixedSize = GetFixedSize(type);
            uint size;
            if (fixedSize == null) {
                size = BitConverter.ToUInt32(data.Skip(dataIndex).Take(4).ToArray());
                dataIndex += 4;
            } else {
                size = fixedSize.Value;
            }
            fields.Add(Decode(data.Skip(dataIndex).Take(checked((int)size)).ToArray(), type));
        }

        return fields;
    }
    /// <summary>
    /// Formats all the fields.
    /// </summary>
    /// <param name="fields">The fields to format.</param>
    /// <returns>The formatted raw data.</returns>
    public static byte[] Format(List<Field<object>> fields) {
        List<byte> rawData = [];
        foreach (Field<object> field in fields) {
            rawData.AddRange(Encode(field));
        }
        return [.. rawData];
    }
    /// <summary>
    /// Encodes a field, with fixed and non-fixed sizes.
    /// </summary>
    /// <param name="field">The field to encode.</param>
    /// <returns>The raw data, with the non-fixed size if there is one.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static byte[] Encode(Field<object> field) {
        List<byte> data = [.. field.Format()];
        if (!field.HasFixedSize) {
            data.InsertRange(0, BitConverter.GetBytes(data.Count));
        } else {
            if (data.Count != field.FixedSize) {
                throw new InvalidOperationException("Size of raw data does not equal to the fixed size");
            }
        }
        return [.. data];
    }
    /// <summary>
    /// Encodes a field, with fixed and non-fixed sizes.
    /// </summary>
    /// <param name="field">The field to encode.</param>
    /// <typeparam name="T">The type of the field type.</typeparam>
    /// <returns>The raw data.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static byte[] Encode<T>(Field<T> field) where T : notnull {
        return Encode(field);
    }
    /// <summary>
    /// Decodes a field, without the non-fixed uint size.
    /// </summary>
    /// <param name="data">The raw data.</param>
    /// <param name="type">The type of the field type.</param>
    /// <returns>The decoded field.</returns>
    public static Field<object> Decode(byte[] data, Type type) {
        return CreateFieldFromType(type, data);
    }
    /// <summary>
    /// Decodes a field, without the non-fixed uint size.
    /// </summary>
    /// <typeparam name="T">The type of the field type.</typeparam>
    /// <param name="data">The raw data.</param>
    /// <returns>The decoded field.</returns>
    public static Field<T> Decode<T>(byte[] data) where T : notnull {
        return (Decode(data, typeof(T)) as Field<T>)!;
    }

    #endregion Parser
}