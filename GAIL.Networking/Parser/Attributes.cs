using GAIL.Serializing;

namespace GAIL.Networking.Parser;

/// <summary>
/// An attribute that will register the constructor of the packet (must be empty).
/// </summary>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
public sealed class PacketConstructorAttribute : Attribute { }

/// <summary>
/// An attribute that will register a packet field.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PacketFieldAttribute : Attribute {
    /// <summary>
    /// The serializable info of the packet field.
    /// </summary>
    public SerializableInfo Info { get; private set; }
    /// <summary>
    /// Creates an attribute that will register a packet field.
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
public sealed class PacketFormatterAttribute : Attribute { }

/// <summary>
/// Defines a method that will be called before serializing the packet.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class PacketSerializeAttribute : Attribute { }

/// <summary>
/// Defines a method that will be called after parsing the packet.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class PacketParseAttribute : Attribute { }