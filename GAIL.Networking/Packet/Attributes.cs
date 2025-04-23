namespace GAIL.Networking;

/// <summary>
/// An attribute that will register the constructor of the packet (must be empty).
/// </summary>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
public sealed class PacketConstructorAttribute : Attribute { }

/// <summary>
/// An attribute that will register a packet field.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PacketFieldAttribute : Attribute { }