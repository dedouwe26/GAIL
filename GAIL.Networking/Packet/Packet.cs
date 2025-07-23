using System.Reflection;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Serializing.Streams;

namespace GAIL.Networking;

/// <summary>
/// Packet is an abstract class for parsing and formatting a packet.
/// </summary>
public abstract class Packet : IReducer {
    /// <summary>
    /// Describes a field in a packet.
    /// </summary>
    /// <param name="Property">The property of the field.</param>
    /// <param name="Info">The serializable info of the field.</param>
    public record class FieldInfo(PropertyInfo Property, IRawSerializable.Info Info);
    /// <summary>
    /// Describes a packet.
    /// </summary>
    public class Info {
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
        private static FieldInfo[] GetFields(PropertyInfo[] properties, object instance) {
            List<FieldInfo> f = [];
            foreach (PropertyInfo property in properties) {
                PacketFieldAttribute? attribute = property.GetCustomAttribute<PacketFieldAttribute>();
                if (attribute != null) {
                    if (!typeof(IRawSerializable).IsAssignableFrom(property.PropertyType)) {
                        throw new ArgumentException($"Property {property.Name} in {property.ReflectedType?.Name ?? "packet"} is not a serializable");
                    }
                    IRawSerializable? serializable = property.GetValue(instance) as IRawSerializable
                        ?? throw new ArgumentException($"Property {property.Name} in {property.ReflectedType?.Name ?? "packet"} is not a serializable");

                    f.Add(new FieldInfo(property, IRawSerializable.TryGetInfo(serializable) ?? throw new InvalidOperationException($"Serializable of packet field {property.Name} in {property.ReflectedType?.Name ?? "packet"} does not have a serializable info")));
                }
            }
            return [.. f];
        }
        internal readonly string fullyQualifiedName;

        internal readonly ConstructorInfo constructor;

        internal readonly FieldInfo[] fields;

        /// <summary>
        /// The used formatter.
        /// </summary>
        public readonly IFormatter? formatter;

        internal Info(Packet packet) {
            Type type = packet.GetType();
            string name = type.Name;
            
            ConstructorInfo? c;
            try {
                c = GetConstructor(
                    type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                ));
            } catch (Exception e) {
                throw new ArgumentException($"Failed at the getting constructor of packet ({name})", e);
            }
            if (c == null) {
                throw new ArgumentException($"Could not find the constructor of packet ({name})");
            }

            FieldInfo[] fields;
            try {
                fields = GetFields(
                    type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
                    packet
                );
            } catch (Exception e) {
                throw new ArgumentException($"Failed at getting the format of packet ({name})", e);
            }
            
            constructor = c;
            this.fields = fields;
            formatter = packet.Formatter;
            fullyQualifiedName = type.AssemblyQualifiedName!;
        }

        private IRawSerializable.Info[]? format;
        /// <summary>
        /// The format of this packet.
        /// </summary>
        public IRawSerializable.Info[] Format { get {
            format ??= [.. fields.Select(f => f.Info)];
            return format;
        } }
        /// <summary>
        /// Creates a packet from the parser (the creator of a packet).
        /// </summary>
        /// <param name="parser">The parser from which to read the packet.</param>
        /// <returns>The parsed packet.</returns>
        public Packet Create(Parser parser) {
            if (constructor.Invoke(null) is not Packet packet) {
                throw new InvalidOperationException($"Failed at creating packet ({fullyQualifiedName})");
            }
            packet.Parse(parser);
            return packet;
        }
    }
    private Info? packetInfo;
    /// <summary>
    /// The info of this packet.
    /// </summary>
    public Info PacketInfo { get {
        packetInfo ??= new(this);
        return packetInfo;
    } }
    private readonly IReducer.Info reducerInfo;
    /// <summary>
    /// The info of the underlying reducer.
    /// </summary>
    [SerializingInfo]
    public IReducer.Info ReducerInfo => reducerInfo;

    /// <summary>
    /// The formatter used for encoding / decoding this packet.
    /// </summary>
    public virtual IFormatter? Formatter => null;
    /// <summary>
    /// Creates a packet (add own data here).
    /// </summary>
    public Packet() {
        reducerInfo = new(PacketInfo.Create);
    }

    private uint? id;
    /// <summary>
    /// The ID of this packet.
    /// </summary>
    /// <exception cref="InvalidOperationException"/>
    public uint ID { get {
        id ??= NetworkRegister.GetPacketID(this) ?? throw new InvalidOperationException($"Packet {GetType().Name} is not registered");
        return id.Value;
    } }

    /// <summary>
    /// Gets called before serializing the packet.
    /// </summary>
    protected virtual void OnSerialize() { }
    /// <summary>
    /// Gets called after parsing the packet.
    /// </summary>
    protected virtual void OnParse() { }

    /// <inheritdoc/>
    public virtual void Serialize(Serializer serializer) {
        OnSerialize();
        foreach (FieldInfo field in PacketInfo.fields) {
            object? gainedValue;
            try {
                gainedValue = field.Property.GetValue(this);
            } catch (Exception e) {
                throw new InvalidOperationException($"Failed at getting field {field.Property.Name} in packet ({PacketInfo.fullyQualifiedName})", e);
            }
            if (gainedValue is not IRawSerializable serializable) {
                throw new InvalidOperationException($"Field {field.Property.Name} in {PacketInfo.fullyQualifiedName} is not a serializable");
            }

            serializer.WriteSerializable(serializable);
        }
    }

    /// <inheritdoc/>
    public virtual void Parse(Parser parser) {
        for (int i = 0; i < PacketInfo.fields.Length; i++) {
            FieldInfo field = PacketInfo.fields[i];
            try {
                field.Property.SetValue(this, parser.ReadSerializable(field.Info));
            } catch (Exception e) {
                throw new InvalidOperationException($"Failed at setting field {field.Property.Name} in packet ({PacketInfo.fullyQualifiedName})", e);
            }
        }
        OnParse();
    }
}