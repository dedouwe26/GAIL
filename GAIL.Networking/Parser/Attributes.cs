using GAIL.Serializing;

namespace GAIL.Networking.Parser;

/// <summary>
/// An attribute that will register a packet.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class PacketAttribute : Attribute { }

/// <summary>
/// An attribute that will register the constructor of the packet (must be empty).
/// </summary>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
public class PacketConstructorAttribute : Attribute { }

/// <summary>
/// An attribute that will register a packet field.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class PacketFieldAttribute : Attribute {
    /// <summary>
    /// The serializable info of the packet field.
    /// </summary>
    public SerializableInfo Info { get; private set; }
    /// <summary>
    /// An attribute that will register a packet field.
    /// </summary>
    /// <param name="info">The serializable info of the packet field.</param>
    public PacketFieldAttribute(SerializableInfo info) {
        Info = info;
    }
}

/// <summary>
/// An attribute that will set the formatter of the packet.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class PacketFormatterAttribute : Attribute { }