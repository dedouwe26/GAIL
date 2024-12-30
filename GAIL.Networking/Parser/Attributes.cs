using System.ComponentModel;

namespace GAIL.Networking.Parser;

/// <summary>
/// An attribute that will register a packet.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class PacketAttribute : Attribute { }

/// <summary>
/// An attribute that will register the constructor of the packet.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
public class PacketConstructorAttribute : Attribute { }

/// <summary>
/// An attribute that will register a packet field.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class PacketFieldAttribute : Attribute { }