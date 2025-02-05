using System.Runtime.InteropServices;

namespace GAIL.Serializing;

/// <summary>
/// An attribute to mark the info of a serializable. Must be on a static field or property.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class SerializableInfoAttribute : Attribute { }