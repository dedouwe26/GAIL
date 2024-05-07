using System.Data;
using System.Reflection;

namespace GAIL.Networking.Parser;

/// <summary>
/// Can convert data, for the GAIL Networking.
/// </summary>
public static class PacketParser {
    private record struct PacketData(ConstructorInfo Constructor, Type[] Format);
    private record struct FieldData(ConstructorInfo FromT, ConstructorInfo FromData, uint? FixedSize);
    private static readonly Dictionary<Type, FieldData> Fields = [];
    private static readonly Dictionary<uint, PacketData> Packets = [];


    #region Constructors
    private static ConstructorInfo GetConstructor(Packet p) {
        return p.GetType().GetConstructor([typeof(List<Field>)]) ?? throw new InvalidOperationException($"Packet ({p.GetType().Name}) does not contain a List<Field> constructor. Add this: '{p.GetType().Name}(List<Field> fields) : base(fields)  {'{'} {'}'}'");
    }
    private static ConstructorInfo GetTypeConstructor<T>(Field<T> field) where T : notnull {
        return field.GetType().GetConstructor([typeof(T)]) ?? throw new InvalidOperationException("The field has no constructor for creating a field from "+typeof(T).Name);
    }
    private static ConstructorInfo GetRawConstructor<T>(Field<T> field) where T : notnull {
        return field.GetType().GetConstructor([typeof(RawData)]) ?? throw new InvalidOperationException("The field has no constructor for creating a field from (raw data)");
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
    /// <param name="fields">All the fields of this packet.</param>
    /// <returns>The parsed packet.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Packet CreatePacket(uint packetID, List<Field> fields) {
        if (Packets.Count <= packetID) {
            throw new InvalidOperationException("Invalid packet ID: "+packetID);
        }
        PacketData packetData = Packets[packetID];
        return (packetData.Constructor.Invoke([fields]) as Packet)!;
    }
    /// <summary>
    /// Formats a packet into raw data.
    /// </summary>
    /// <param name="packet">The packet to format.</param>
    /// <returns>The formatted raw data.</returns>
    public static byte[] FormatPacket(Packet packet) {
        return [.. GetBytesFromPacketID(GetPacketID(packet)), .. Format(packet.GetFields())];
    }
    /// <summary>
    /// Gets the format of the packet.
    /// </summary>
    /// <param name="packetID">The ID of the packet.</param>
    /// <returns>The format of the packet.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Type[] GetPacketFormat(uint packetID) {
        if (!Packets.TryGetValue(packetID, out PacketData data)) {
            throw new InvalidOperationException("Invalid packet ID: "+packetID);
        }
        return data.Format;
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
    public static Field CreateFieldFromType(Type type, object data) {

        if (!Fields.TryGetValue(type, out FieldData ctor)) {
            throw new ArgumentException("No field found for type: "+type.Name, nameof(type));
        }
        if (ctor.FromT == null) {
            throw new ArgumentException("No field found for type: "+type.Name, nameof(type));
        }
        object obj = ctor.FromT!.Invoke([data]);

        return (obj as Field)!;
    }
    /// <summary>
    /// Creates a field from the data.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="data">The data.</param>
    /// <returns>The created field.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static Field<T> CreateFieldFromType<T>(T data) where T : notnull {
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
    public static Field CreateFieldFromType(Type type, RawData data) {
        if (!Fields.TryGetValue(type, out FieldData ctor)) {
            throw new ArgumentException("No field found for type: "+type.Name, nameof(type));
        }
        object obj = ctor.FromData.Invoke([data]);
        if (obj is not Field) {
            throw new InvalidOperationException("Constructor of field is not for field");
        }
        return (obj as Field)!;
    }
    /// <summary>
    /// Creates a field from an type.
    /// </summary>
    /// <typeparam name="T">The value type of the field.</typeparam>
    /// <param name="data">The raw data for the field.</param>
    /// <returns>The created field.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static Field<T> CreateFieldFromType<T>(RawData data) where T : notnull {
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
            GetTypeConstructor(field), 
            GetRawConstructor(field),
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
    public static List<Field> ParseAll(byte[] data, Type[] format) {
        List<Field> fields = [];
        
        if (data.Length <= 0) {
            return fields;
        }

        int dataIndex = 0;
        foreach (Type type in format) {
            uint? fixedSize = GetFixedSize(type);
            uint size;
            if (fixedSize == null) {
                size = BitConverter.IsLittleEndian ? BitConverter.ToUInt32(data.Skip(dataIndex).Take(4).ToArray()) : BitConverter.ToUInt32(data.Skip(dataIndex).Take(4).Reverse().ToArray());
                dataIndex += 4;
            } else {
                size = fixedSize.Value;
            }
            fields.Add(Decode(data.Skip(dataIndex).Take(checked((int)size)).ToArray(), type));
        }

        return fields;
    }

    /// <summary>
    /// Parses the stream for packets.
    /// </summary>
    /// <param name="stream">The stream to read the raw data from to parse.</param>
    /// <param name="isClosed">If it should stop and return.</param>
    /// <param name="onPacket">The callback for when a packet has been received. Returns true if it should stop.</param>
    /// <returns>True if it was successfull, otherwise false.</returns>
    public static bool Parse(Stream stream, Func<bool> isClosed, Func<Packet, bool> onPacket) {
        if (!stream.CanRead) { return false; }
        
        bool isInPacket = false;
        uint packetID = 0;
        List<Field> fields = [];
        Type[] format = [];
        int formatIndex = 0;
        byte[] buffer;
        while (!isClosed()) {
            if (!isInPacket) {
                buffer = new byte[4];
                if (stream.Read(buffer, 0, 4) >= 0) {
                    continue;
                }
                packetID = GetPacketID(buffer);
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
            
            uint? size = GetFixedSize(format[formatIndex]);
            
            if (size == null) {
                buffer = new byte[4];
                if (stream.Read(buffer, 0, 4) >= 0) {
                    continue;
                }
                size = BitConverter.IsLittleEndian ? BitConverter.ToUInt32(buffer) : BitConverter.ToUInt32([.. buffer.Reverse()]);
            }
            buffer = new byte[size.Value];
            if (stream.Read(buffer, 0, checked((int)size)) >= 0) {
                continue;
            }
            fields.Add(Decode(buffer, format[formatIndex]));

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

    /// <summary>
    /// Formats all the fields.
    /// </summary>
    /// <param name="fields">The fields to format.</param>
    /// <returns>The formatted raw data.</returns>
    public static byte[] Format(List<Field> fields) {
        List<byte> rawData = [];
        foreach (Field field in fields) {
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
    public static byte[] Encode(Field field) {
        List<byte> data = [.. field.Format()];
        if (!field.HasFixedSize) {
            data.InsertRange(0, BitConverter.IsLittleEndian ? BitConverter.GetBytes(data.Count) : BitConverter.GetBytes(data.Count).Reverse());
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
    public static Field Decode(byte[] data, Type type) {
        return CreateFieldFromType(type, new RawData{ data = data });
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